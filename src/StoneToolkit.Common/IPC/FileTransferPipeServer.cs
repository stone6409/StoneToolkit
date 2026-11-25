using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.Common.IPC
{
    public class FileTransferPipeServer : PipeServer
    {
        public FileTransferPipeServer(string pipeName, bool canMultiInstance = false) : base(pipeName, canMultiInstance)
        {
        }

        protected override void HandleClientMessage(NamedPipeServerStream pipeServer, string message)
        {
            if (message == "GET_FILES")
            {
                // 自动获取需要回复的文件（例如从特定目录中获取）
                string directoryPath = @"D:\src\ScTrials\src\CodeFactory\PipeServerDemo\Files";
                string[] filePaths = Directory.GetFiles(directoryPath);

                if (filePaths.Length > 0)
                {
                    foreach (string filePath in filePaths)
                    {
                        try
                        {
                            // 读取文件内容
                            string fileContent = File.ReadAllText(filePath);
                            string responseMessage = $"FILE_RESPONSE:{filePath}\n```\n{fileContent}\n```";

                            // 发送文件内容给客户端
                            byte[] responseBuffer = Encoding.UTF8.GetBytes(responseMessage);
                            pipeServer.WriteAsync(responseBuffer, 0, responseBuffer.Length).Wait();
                            Console.WriteLine($"Sent file content: {filePath}");
                        }
                        catch (Exception ex)
                        {
                            // 发送错误消息
                            string errorMessage = $"FILE_ERROR:{filePath} could not be read: {ex.Message}";
                            byte[] errorBuffer = Encoding.UTF8.GetBytes(errorMessage);
                            pipeServer.WriteAsync(errorBuffer, 0, errorBuffer.Length).Wait();
                            Console.WriteLine($"Error reading file: {filePath}");
                        }
                    }
                }
                else
                {
                    // 没有找到文件，发送提示消息
                    string noFilesMessage = "FILE_ERROR:No files found in the directory.";
                    byte[] noFilesBuffer = Encoding.UTF8.GetBytes(noFilesMessage);
                    pipeServer.WriteAsync(noFilesBuffer, 0, noFilesBuffer.Length).Wait();
                    Console.WriteLine("No files found.");
                }

                // 发送结束标记
                string endMarker = "END_OF_FILES";
                byte[] endMarkerBuffer = Encoding.UTF8.GetBytes(endMarker);
                pipeServer.WriteAsync(endMarkerBuffer, 0, endMarkerBuffer.Length).Wait();
                Console.WriteLine("Sent end marker.");
            }
            else
            {
                // 触发普通消息事件
                base.HandleClientMessage(pipeServer, message);
            }
        }
    }
}