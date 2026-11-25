using System;
using System.Collections.Generic;

namespace StoneToolkit.Web.Http
{
    /// <summary>
    /// HttpClient配置选项
    /// </summary>
    public class HttpClientOptions
    {
        /// <summary>
        /// 基础URL
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// 请求超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// 是否启用重试
        /// </summary>
        public bool EnableRetry { get; set; } = false;

        /// <summary>
        /// 重试延迟（毫秒）
        /// </summary>
        public int RetryDelayMs { get; set; } = 1000;

        /// <summary>
        /// 默认请求头
        /// </summary>
        public Dictionary<string, string> DefaultHeaders { get; set; } = new Dictionary<string, string>();
    }
}