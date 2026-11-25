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
    public class HttpClientWithApiKey : HttpClient
    {
        private string _apiKey;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">配置选项</param>
        public HttpClientWithApiKey(HttpClientOptions options, string apiKey) :
            base(options) 
        {
            SetApiKey(apiKey);
        }

        /// <summary>
        /// 构造函数（使用默认配置）
        /// </summary>
        /// <param name="baseUrl">基础URL</param>
        /// <param name="apiKey">API密钥</param>
        public HttpClientWithApiKey(string baseUrl, string apiKey)
            : base(baseUrl)
        {
            SetApiKey(apiKey);
        }

        /// <summary>
        /// 设置API Key
        /// </summary>
        public void SetApiKey(string apiKey)
        {
            _apiKey = apiKey;

            // 清除现有的Authorization头
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            // 设置新的API Key头
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            }

            //if (!string.IsNullOrEmpty(_apiKey))
            //{
            //    _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
            //}
        }
    }
}