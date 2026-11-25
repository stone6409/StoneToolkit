using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StoneToolkit.Common.IPC
{
    public class PipeClient
    {
        private string _pipeName;
        private NamedPipeClientStream _pipeClient;
        private CancellationTokenSource _cts;
        private Task _readTask;

        public event MessageReceivedHandler MessageReceived;

        public PipeClient(string pipeName, bool canMultiInstance = false)
        {
            if (canMultiInstance)
            {
                string existPipeName = PipeServerInfoHelper.GetExistPipeName(pipeName);
                if (existPipeName != null)
                {
                    pipeName = existPipeName;
                }
            }

            _pipeName = pipeName;
            _cts = new CancellationTokenSource();
        }

        public async Task ConnectAsync()
        {
            _pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            await _pipeClient.ConnectAsync(500); // Timeout after 500ms
            Console.WriteLine("Connected to pipe.");

            // Start reading from the pipe asynchronously
            _readTask = Task.Run(async () => await ReadFromPipeAsync(_cts.Token), _cts.Token);
        }

        //private async Task ReadFromPipeAsync(CancellationToken token)
        //{
        //    try
        //    {
        //        byte[] buffer = new byte[256]; // 增加缓冲区大小
        //        Decoder decoder = Encoding.UTF8.GetDecoder();
        //        StringBuilder stringBuilder = new StringBuilder();
        //        char[] charBuffer = new char[Encoding.UTF8.GetMaxCharCount(buffer.Length)];
        //        int incompleteBytes = 0; // 记录不完整的字节数
        //        byte[] incompleteBuffer = new byte[4]; // 最多保留 4 个字节（UTF-8 最大字符长度）

        //        while (!token.IsCancellationRequested && _pipeClient.IsConnected)
        //        {
        //            int bytesRead = await _pipeClient.ReadAsync(buffer, incompleteBytes, buffer.Length - incompleteBytes, token);
        //            if (bytesRead > 0)
        //            {
        //                // 如果有不完整的字节，将其与新数据合并
        //                if (incompleteBytes > 0)
        //                {
        //                    Buffer.BlockCopy(incompleteBuffer, 0, buffer, 0, incompleteBytes);
        //                    //Buffer.BlockCopy(buffer, 0, buffer, incompleteBytes, bytesRead);
        //                    bytesRead += incompleteBytes;
        //                    incompleteBytes = 0;
        //                }

        //                // 解码数据
        //                decoder.Reset();
        //                int charCount = decoder.GetChars(buffer, 0, bytesRead, charBuffer, 0, false);
        //                string receivedMessage = new string(charBuffer, 0, charCount);
        //                stringBuilder.Append(receivedMessage);

        //                string allReceivedMessage = stringBuilder.ToString();

        //                Console.WriteLine("Received partial data: " + receivedMessage);

        //                // 检查是否接收到结束标记 "EOF"
        //                if (stringBuilder.ToString().EndsWith("-EOF-"))
        //                {
        //                    string completeMessage = stringBuilder.ToString().Replace("-EOF-", "");
        //                    Console.WriteLine("Received complete data: " + completeMessage);

        //                    // 触发事件，传递完整消息和管道流
        //                    MessageReceived?.Invoke(this, new PipeMessageEventArgs(completeMessage, _pipeClient));

        //                    // 清空 StringBuilder 以准备接收下一批数据
        //                    stringBuilder.Clear();
        //                }
        //                else
        //                {
        //                    // 检查是否有不完整的字符
        //                    int byteCount = Encoding.UTF8.GetByteCount(receivedMessage);
        //                    if (byteCount < bytesRead)
        //                    {
        //                        incompleteBytes = bytesRead - byteCount;
        //                        Buffer.BlockCopy(buffer, byteCount, incompleteBuffer, 0, incompleteBytes);
        //                    }
        //                    else
        //                    {
        //                        incompleteBytes = 0; // 重置不完整字节数
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // 如果读取到的字节数为 0，说明服务器已关闭连接
        //                break;
        //            }
        //        }
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        Console.WriteLine("Reading was canceled.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error reading from pipe: {ex.Message}");
        //    }
        //}

        private async Task ReadFromPipeAsync(CancellationToken token)
        {
            try
            {
                byte[] buffer = new byte[256]; // 增加缓冲区大小
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] charBuffer = new char[Encoding.UTF8.GetMaxCharCount(buffer.Length)];
                StringBuilder stringBuilder = new StringBuilder();

                while (!token.IsCancellationRequested && _pipeClient.IsConnected)
                {
                    int bytesRead = await _pipeClient.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead > 0)
                    {
                        // 解码数据
                        int charCount = decoder.GetChars(buffer, 0, bytesRead, charBuffer, 0, false);
                        string receivedMessage = new string(charBuffer, 0, charCount);
                        stringBuilder.Append(receivedMessage);

                        //string allReceivedMessage = stringBuilder.ToString();

                        Console.WriteLine("Received partial data: " + receivedMessage);

                        // 检查是否接收到结束标记 "EOF"
                        if (stringBuilder.ToString().EndsWith("-EOF-"))
                        {
                            string completeMessage = stringBuilder.ToString().Replace("-EOF-", "");
                            Console.WriteLine("Received complete data: " + completeMessage);

                            // 触发事件，传递完整消息和管道流
                            MessageReceived?.Invoke(this, new PipeMessageEventArgs(completeMessage, _pipeClient));

                            // 清空 StringBuilder 以准备接收下一批数据
                            stringBuilder.Clear();
                        }
                    }
                    else
                    {
                        // 如果读取到的字节数为 0，说明服务器已关闭连接
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Reading was canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from pipe: {ex.Message}");
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (_pipeClient == null || !_pipeClient.IsConnected)
            {
                throw new InvalidOperationException("Client is not connected to the pipe.");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await _pipeClient.WriteAsync(buffer, 0, buffer.Length);
            Console.WriteLine("Sent: " + message);
        }

        public async Task RequestFilesAsync()
        {
            if (_pipeClient == null || !_pipeClient.IsConnected)
            {
                throw new InvalidOperationException("Client is not connected to the pipe.");
            }

            // 发送获取文件的请求消息
            string requestMessage = "GET_FILES";
            byte[] buffer = Encoding.UTF8.GetBytes(requestMessage);
            await _pipeClient.WriteAsync(buffer, 0, buffer.Length);
            Console.WriteLine("Sent file request.");
        }

        public void Disconnect()
        {
            _cts.Cancel();
            _readTask?.Wait();
            _pipeClient?.Close();
            Console.WriteLine("Disconnected from pipe.");
        }
    }
}
