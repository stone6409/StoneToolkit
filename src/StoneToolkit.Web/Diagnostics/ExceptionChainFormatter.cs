using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace StoneToolkit.Web.Diagnostics
{
    /// <summary>
    /// 异常链格式化工具：把 InnerException 逐层展开为单行文本，
    /// 保留 SocketError / HttpRequestError / HResult 等定位企业网络问题必需的信息。
    /// </summary>
    public static class ExceptionChainFormatter
    {
        private const int MaxDepth = 8;

        /// <summary>
        /// 格式化为单行：Type: Message [HttpRequestError=...] [SocketError=...] [HResult=...] &lt;- InnerType: ...
        /// </summary>
        public static string Format(Exception exception)
        {
            if (exception == null)
                return "(null)";

            var builder = new StringBuilder();
            var current = exception;
            var depth = 0;

            while (current != null && depth < MaxDepth)
            {
                if (depth > 0)
                    builder.Append(" <- ");

                builder.Append(current.GetType().Name)
                       .Append(": ")
                       .Append(current.Message);

                if (current is HttpRequestException httpRequestException)
                {
                    builder.Append(" [HttpRequestError=").Append(httpRequestException.HttpRequestError).Append(']');
                }

                if (current is SocketException socketException)
                {
                    builder.Append(" [SocketError=").Append(socketException.SocketErrorCode)
                           .Append(", NativeErrorCode=").Append(socketException.NativeErrorCode).Append(']');
                }

                builder.Append(" [HResult=0x").Append(current.HResult.ToString("X8")).Append(']');

                current = current.InnerException;
                depth++;
            }

            return builder.ToString();
        }
    }
}
