using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StoneToolkit.Common.IPC
{
    public class PipeServer
    {
        private string _basePipeName = null;
        private string _pipeName = null;
        private static Mutex _mutex = null;
        private bool _isRunning = true;
        private EventWaitHandle _readyEvent = null;

        public event MessageReceivedHandler MessageReceived;

        public PipeServer(string pipeName, bool canMultiInstance = false)
        {
            _basePipeName = pipeName;

            if (canMultiInstance)
            {
                pipeName = PipeServerInfoHelper.GenerateUniquePipeName(_basePipeName);
            }

            _pipeName = pipeName;
        }

        public bool StartServer()
        {
            bool createdNew;
            _mutex = new Mutex(true, _pipeName, out createdNew);
            if (!createdNew)
            {
                return false;
            }

            // 清理可能残留的上一次事件句柄
            try
            {
                if (EventWaitHandle.TryOpenExisting(_pipeName + "_Ready", out var oldEvent))
                {
                    oldEvent.Dispose();
                }
            }
            catch
            {
                // 忽略清理异常
            }

            // 创建命名事件，用于跨进程通知管道就绪状态
            _readyEvent = new EventWaitHandle(false, EventResetMode.ManualReset, _pipeName + "_Ready");

            PipeServerInfoHelper.AddPipeServerInfos(_basePipeName, _pipeName);

            // Start the server asynchronously
            Task.Run(() => InternalStartServer());

            return true;
        }

        /// <summary>
        /// 等待指定管道的服务器就绪（跨进程）。
        /// </summary>
        /// <param name="pipeName">管道名称</param>
        /// <param name="timeoutMilliseconds">超时时间（毫秒）</param>
        /// <returns>如果管道就绪返回 true，超时返回 false</returns>
        public static bool WaitForReady(string pipeName, int timeoutMilliseconds)
        {
            string eventName = pipeName + "_Ready";
            try
            {
                using (var handle = EventWaitHandle.OpenExisting(eventName))
                {
                    return handle.WaitOne(timeoutMilliseconds);
                }
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                return false;
            }
        }

        protected virtual void InternalStartServer()
        {
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                // 管道流已创建完成，通知等待方（客户端）
                _readyEvent?.Set();

                while (_isRunning)
                {
                    Console.WriteLine("Waiting for client connection...");
                    try
                    {
                        pipeServer.WaitForConnection();
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Client disconnected.");
                        break;
                    }

                    Console.WriteLine("Client connected.");

                    try
                    {
                        byte[] buffer = new byte[1024];
                        Decoder decoder = Encoding.UTF8.GetDecoder();
                        char[] charBuffer = new char[Encoding.UTF8.GetMaxCharCount(buffer.Length)];

                        StringBuilder stringBuilder = new StringBuilder();
                        while (true)
                        {
                            int bytesRead = pipeServer.ReadAsync(buffer, 0, buffer.Length).Result;

                            //if (bytesRead == 0)
                            //{
                            //    continue;
                            //}

                            if (bytesRead > 0)
                            {
                                // 解码数据
                                int charCount = decoder.GetChars(buffer, 0, bytesRead, charBuffer, 0, false);
                                string receivedMessage = new string(charBuffer, 0, charCount);
                                stringBuilder.Append(receivedMessage);
                            }

                            if (bytesRead < buffer.Length)
                            {
                                break;
                            }
                        }

                        string allReceivedMessage = stringBuilder.ToString();
                        HandleClientMessage(pipeServer, allReceivedMessage);

                        // 不立即关闭连接，等待客户端确认接收完毕
                        // 或者约定特殊标记（如 "EOF"）表示数据传输结束
                        byte[] eofBuffer = Encoding.UTF8.GetBytes("-EOF-");
                        pipeServer.WriteAsync(eofBuffer, 0, eofBuffer.Length).Wait();
                        pipeServer.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }

                    // 关闭当前连接，等待下一个连接
                    pipeServer.Disconnect();
                }

                pipeServer.Close();
            }
        }

        protected virtual void HandleClientMessage(NamedPipeServerStream pipeServer, string message)
        {
            // 触发事件，传递消息和管道流
            MessageReceived?.Invoke(this, new PipeMessageEventArgs(message, pipeServer));
        }

        public void StopServer()
        {
            _isRunning = false;
            _readyEvent?.Dispose();
            _readyEvent = null;
        }
    }
}