using System.Text.Json.Serialization;

namespace StoneToolkit.Web.Http
{
    /// <summary>
    /// 统一的API响应模型
    /// </summary>
    /// <typeparam name="T">响应数据类型</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 响应数据
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// HTTP状态码
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 创建成功的响应
        /// </summary>
        /// <param name="data">响应数据</param>
        /// <param name="statusCode">HTTP状态码</param>
        /// <returns>成功的ApiResponse</returns>
        public static ApiResponse<T> SuccessResponse(T data, int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                StatusCode = statusCode,
                ErrorMessage = null
            };
        }

        /// <summary>
        /// 创建失败的响应
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="statusCode">HTTP状态码</param>
        /// <returns>失败的ApiResponse</returns>
        public static ApiResponse<T> ErrorResponse(string errorMessage, int statusCode = 500)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                StatusCode = statusCode,
                ErrorMessage = errorMessage
            };
        }

        /// <summary>
        /// 从HttpResponseMessage创建响应
        /// </summary>
        /// <param name="response">HTTP响应</param>
        /// <param name="data">反序列化的数据</param>
        /// <returns>ApiResponse</returns>
        public static ApiResponse<T> FromHttpResponse(System.Net.Http.HttpResponseMessage response, T? data)
        {
            return new ApiResponse<T>
            {
                Success = response.IsSuccessStatusCode,
                Data = data,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = response.IsSuccessStatusCode ? null : $"HTTP错误: {response.StatusCode}"
            };
        }
    }
}