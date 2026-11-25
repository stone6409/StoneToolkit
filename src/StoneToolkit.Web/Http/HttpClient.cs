using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StoneToolkit.Web.Http
{
    /// <summary>
    /// HttpClient服务实现
    /// </summary>
    public class HttpClient : IHttpClient
    {
        protected readonly System.Net.Http.HttpClient _httpClient;
        protected readonly JsonSerializerOptions _jsonOptions;
        protected readonly HttpClientOptions _options;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">配置选项</param>
        public HttpClient(HttpClientOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            
            // 创建HttpClient实例
            // BaseUrl 统一补尾斜杠：否则 BaseAddress 末段会被相对路径替换，导致路径前缀（如 /filesync）丢失
            _httpClient = new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri(NormalizeBaseUrl(_options.BaseUrl)),
                Timeout = _options.Timeout
            };

            // 配置JSON序列化选项
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            // 设置默认请求头
            ConfigureDefaultHeaders();
        }

        /// <summary>
        /// 构造函数（使用默认配置）
        /// </summary>
        /// <param name="baseUrl">基础URL</param>
        /// <param name="apiKey">API密钥</param>
        public HttpClient(string baseUrl, string apiKey = null)
            : this(new HttpClientOptions
            {
                BaseUrl = baseUrl
            })
        {
        }

        /// <summary>
        /// 发送GET请求
        /// </summary>
        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, params object[] parameters)
        {
            return await ExecuteAsync<T>(HttpMethod.Get, endpoint, null, parameters);
        }

        /// <summary>
        /// 发送POST请求
        /// </summary>
        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
        {
            return await ExecuteAsync<T>(HttpMethod.Post, endpoint, data);
        }

        /// <summary>
        /// 发送DELETE请求
        /// </summary>
        public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, params object[] parameters)
        {
            return await ExecuteAsync<T>(HttpMethod.Delete, endpoint, null, parameters);
        }

        /// <summary>
        /// 发送PATCH请求
        /// </summary>
        public async Task<ApiResponse<T>> PatchAsync<T>(string endpoint, object data)
        {
            return await ExecuteAsync<T>(new HttpMethod("PATCH"), endpoint, data);
        }

        /// <summary>
        /// 获取内部的HttpClient实例
        /// </summary>
        public System.Net.Http.HttpClient GetHttpClient()
        {
            return _httpClient;
        }

        /// <summary>
        /// 把相对端点解析为绝对 URL。
        /// 供直接使用 GetHttpClient() 的调用方使用：BaseUrl 带路径前缀（如 https://host/filesync/）时，
        /// 相对路径若以 '/' 开头会丢弃前缀，必须经此方法拼接。
        /// </summary>
        public string ResolveUrl(string endpoint)
        {
            var baseAddress = _httpClient.BaseAddress;
            if (baseAddress == null)
                return endpoint;

            return new Uri(baseAddress, (endpoint ?? string.Empty).TrimStart('/')).ToString();
        }

        /// <summary>
        /// 归一化 BaseUrl：去掉首尾空白，并保证以 '/' 结尾。
        /// </summary>
        private static string NormalizeBaseUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("BaseUrl 不能为空", nameof(baseUrl));

            var normalized = baseUrl.Trim();
            return normalized.EndsWith("/", StringComparison.Ordinal) ? normalized : normalized + "/";
        }

        #region 私有方法

        /// <summary>
        /// 执行HTTP请求
        /// </summary>
        protected virtual async Task<ApiResponse<T>> ExecuteAsync<T>(
            HttpMethod method,
            string endpoint,
            object? data = null,
            params object[] parameters)
        {
            try
            {
                // 构建URL
                var url = BuildUrl(endpoint, parameters);

                // 发送请求
                HttpResponseMessage response;
                if (_options.EnableRetry && _options.MaxRetries > 0)
                {
                    // 每次重试都创建全新的请求消息，避免复用已发送的HttpRequestMessage导致
                    // "The request message was already sent"异常
                    response = await ExecuteWithRetryAsync(async () =>
                    {
                        using var request = CreateRequest(method, url, data);
                        return await _httpClient.SendAsync(request);
                    });
                }
                else
                {
                    using var request = CreateRequest(method, url, data);
                    response = await _httpClient.SendAsync(request);
                }

                // 处理响应
                return await ProcessResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                return HandleException<T>(ex);
            }
        }

        /// <summary>
        /// 构建URL
        /// </summary>
        protected virtual string BuildUrl(string endpoint, object[] parameters)
        {
            // 去掉前导 '/'：否则 BaseAddress 的路径前缀（如 /filesync）会被丢弃
            var url = (endpoint ?? string.Empty).TrimStart('/');

            // 处理URL参数
            if (parameters != null && parameters.Length > 0)
            {
                // 对参数进行URL编码
                var encodedParams = parameters.Select(p => System.Web.HttpUtility.UrlEncode(p?.ToString() ?? ""));
                url = string.Format(url, encodedParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// 创建请求消息
        /// </summary>
        private HttpRequestMessage CreateRequest(HttpMethod method, string url, object? data)
        {
            var request = new HttpRequestMessage(method, url);

            // 添加请求内容（如果是POST/PATCH）
            if (data != null && (method == HttpMethod.Post || method.Method == "PATCH"))
            {
                var jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            }

            return request;
        }

        /// <summary>
        /// 处理HTTP响应
        /// </summary>
        protected virtual async Task<ApiResponse<T>> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            try
            {
                T? data = default;
                
                // 如果响应成功且有内容，尝试反序列化
                if (response.IsSuccessStatusCode && response.Content.Headers.ContentLength > 0)
                {
                    try
                    {
                        // 使用System.Net.Http.Json进行反序列化
                        data = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
                    }
                    catch (JsonException)
                    {
                        // 如果反序列化失败，但响应是成功的，可能返回的是简单类型（如string）
                        if (typeof(T) == typeof(string))
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            data = (T)(object)content;
                        }
                    }
                }

                // 创建ApiResponse
                return ApiResponse<T>.FromHttpResponse(response, data);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.ErrorResponse($"处理响应时发生错误: {ex.Message}", (int)response.StatusCode);
            }
        }

        /// <summary>
        /// 执行带重试的请求
        /// </summary>
        protected virtual async Task<HttpResponseMessage> ExecuteWithRetryAsync(Func<Task<HttpResponseMessage>> requestFunc)
        {
            int retryCount = 0;
            Exception? lastException = null;

            while (retryCount <= _options.MaxRetries)
            {
                try
                {
                    return await requestFunc();
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    retryCount++;

                    if (retryCount > _options.MaxRetries)
                        break;

                    // 等待一段时间后重试
                    await Task.Delay(_options.RetryDelayMs * retryCount);
                }
                catch (TaskCanceledException ex)
                {
                    lastException = ex;
                    retryCount++;

                    if (retryCount > _options.MaxRetries)
                        break;

                    // 等待一段时间后重试
                    await Task.Delay(_options.RetryDelayMs * retryCount);
                }
            }

            throw lastException ?? new HttpRequestException("请求失败，重试次数已达上限");
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        protected virtual ApiResponse<T> HandleException<T>(Exception ex)
        {
            string errorMessage;
            int statusCode = 500;

            switch (ex)
            {
                case HttpRequestException httpEx:
                    errorMessage = $"HTTP请求失败: {httpEx.Message}";
                    break;
                case TaskCanceledException:
                    errorMessage = "请求超时";
                    statusCode = 408; // Request Timeout
                    break;
                case JsonException jsonEx:
                    errorMessage = $"JSON序列化/反序列化失败: {jsonEx.Message}";
                    break;
                default:
                    errorMessage = $"请求执行失败: {ex.Message}";
                    break;
            }

            return ApiResponse<T>.ErrorResponse(errorMessage, statusCode);
        }

        /// <summary>
        /// 配置默认请求头
        /// </summary>
        protected virtual void ConfigureDefaultHeaders()
        {
            // 添加其他默认头
            foreach (var header in _options.DefaultHeaders)
            {
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            // 设置Accept头
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion
    }
}