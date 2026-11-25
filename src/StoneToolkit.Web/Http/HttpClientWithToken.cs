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
    /// 认证专用的HttpClient服务实现
    /// 包含Token管理和认证相关的HTTP请求功能
    /// </summary>
    public class HttpClientWithToken : HttpClient
    {
        private string _accessToken;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">配置选项</param>
        public HttpClientWithToken(HttpClientOptions options) : base(options)
        {
        }

        /// <summary>
        /// 构造函数（使用默认配置）
        /// </summary>
        /// <param name="baseUrl">基础URL</param>
        public HttpClientWithToken(string baseUrl = "http://localhost:5045")
            : base(baseUrl, null)
        {
        }

        /// <summary>
        /// 设置Token
        /// </summary>
        public void SetTokens(string accessToken)
        {
            _accessToken = accessToken;
        }

        /// <summary>
        /// 重写ExecuteAsync方法，添加Token处理逻辑
        /// </summary>
        protected override async Task<ApiResponse<T>> ExecuteAsync<T>(
            HttpMethod method,
            string endpoint,
            object? data = null,
            params object[] parameters)
        {
            try
            {
                // 构建URL
                var url = BuildUrl(endpoint, parameters);

                // 发送请求（调用基类的重试逻辑）
                HttpResponseMessage response;
                if (_options.EnableRetry && _options.MaxRetries > 0)
                {
                    // 每次重试都创建全新的请求消息，避免复用已发送的HttpRequestMessage导致
                    // "The request message was already sent"异常
                    response = await ExecuteWithRetryAsync(async () =>
                    {
                        using var request = CreateRequestWithToken(method, url, data);
                        return await _httpClient.SendAsync(request);
                    });
                }
                else
                {
                    using var request = CreateRequestWithToken(method, url, data);
                    response = await _httpClient.SendAsync(request);
                }

                // 处理响应（调用基类的响应处理逻辑）
                return await ProcessResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                return HandleException<T>(ex);
            }
        }

        /// <summary>
        /// 创建请求消息（包含Token认证头）
        /// </summary>
        private HttpRequestMessage CreateRequestWithToken(HttpMethod method, string url, object? data)
        {
            var request = new HttpRequestMessage(method, url);

            // 设置Authorization Header（认证特有的功能）
            if (!string.IsNullOrEmpty(_accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }

            // 添加请求内容（如果是POST/PATCH）
            if (data != null && (method == HttpMethod.Post || method.Method == "PATCH"))
            {
                var jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            }

            return request;
        }
    }
}