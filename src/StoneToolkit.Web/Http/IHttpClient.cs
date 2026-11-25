using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoneToolkit.Web.Http
{
    /// <summary>
    /// HttpClient服务接口
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <param name="endpoint">API端点</param>
        /// <param name="parameters">URL参数</param>
        /// <returns>API响应</returns>
        Task<ApiResponse<T>> GetAsync<T>(string endpoint, params object[] parameters);

        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <param name="endpoint">API端点</param>
        /// <param name="data">请求数据</param>
        /// <returns>API响应</returns>
        Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data);

        /// <summary>
        /// 发送DELETE请求
        /// </summary>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <param name="endpoint">API端点</param>
        /// <param name="parameters">URL参数</param>
        /// <returns>API响应</returns>
        Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, params object[] parameters);

        /// <summary>
        /// 发送PATCH请求
        /// </summary>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <param name="endpoint">API端点</param>
        /// <param name="data">请求数据</param>
        /// <returns>API响应</returns>
        Task<ApiResponse<T>> PatchAsync<T>(string endpoint, object data);
    }
}