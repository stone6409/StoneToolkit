using System;
using System.Collections.Generic;
using System.Threading;

namespace StoneToolkit.Common.IPC
{
    public static class PipeServerInfoHelper
    {
        public static string GenerateUniquePipeName(string basePipeName)
        {
            string newName = basePipeName;
            uint number = 1;

            while (true)
            {
                if (!IsExistPipeName(basePipeName, newName))
                {
                    break;
                }

                newName = basePipeName + number;
                ++number;
            }

            return newName;
        }


        public static bool IsExistPipeName(string basePipeName, string pipeName)
        {
            PipeServerInfoManager pipeServerInfoManager = PipeServerInfoManager.GetPipeServerInfoManager(basePipeName);

            List<ServerInfo> serverInfos = pipeServerInfoManager.GetAllServerInfo();
            foreach (ServerInfo serverInfo in serverInfos)
            {
                if (!serverInfo.IsEmpty && serverInfo.PipeName == pipeName)
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetExistPipeName(string basePipeName)
        {
            PipeServerInfoManager pipeServerInfoManager = PipeServerInfoManager.GetPipeServerInfoManager(basePipeName);

            List<ServerInfo> serverInfos = pipeServerInfoManager.GetAllServerInfo();
            foreach (ServerInfo serverInfo in serverInfos)
            {
                if (!serverInfo.IsEmpty && IsPipeExist(serverInfo.PipeName))
                {
                    return serverInfo.PipeName;
                }
            }

            return null;
        }

        public static bool IsAnyExistPipeName(string basePipeName)
        {
            PipeServerInfoManager pipeServerInfoManager = PipeServerInfoManager.GetPipeServerInfoManager(basePipeName);

            List<ServerInfo> serverInfos = pipeServerInfoManager.GetAllServerInfo();
            foreach (ServerInfo serverInfo in serverInfos)
            {
                if (!serverInfo.IsEmpty && IsPipeExist(serverInfo.PipeName))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsPipeExist(string pipeName)
        {
            return IsMutexExists(pipeName);

            //try
            //{
            //    // 尝试连接到管道
            //    using (var pipeClient = new NamedPipeClientStream(pipeName))
            //    {
            //        pipeClient.Connect(500);  // 设置超时时间为500毫秒
            //        return true;
            //    }
            //}
            //catch (TimeoutException)
            //{
            //    // 如果超时异常，说明管道不存在
            //    return false;
            //}
            //catch (IOException)
            //{
            //    // 如果发生IO异常，管道也可能不存在
            //    return false;
            //}
        }

        // 检测指定名称的Mutex是否存在的方法
        public static bool IsMutexExists(string mutexName)
        {
            try
            {
                bool createdNew;
                using (Mutex mutex = new Mutex(false, mutexName, out createdNew))
                {
                    if (!createdNew)
                    {
                        return true;
                    }
                }

                //mutex.ReleaseMutex();

                //Mutex mutex;
                //if (Mutex.TryOpenExisting(mutexName, out mutex))
                //{
                //    mutex.ReleaseMutex();
                //    return true;
                //}

                return false; // 如果没有抛出异常，说明Mutex不存在
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                return true; // 如果抛出异常，说明Mutex存在
            }
            catch (Exception ex)
            {
                // 处理其他异常情况，比如日志记录等
                Console.WriteLine($"An unexpected exception occurred: {ex.Message}");
                return false;
            }
        }

        public static void AddPipeServerInfos(string basePipeName, string newPipeName)
        {
            PipeServerInfoManager pipeServerInfoManager = PipeServerInfoManager.GetPipeServerInfoManager(basePipeName);

            // 从共享内存中获取所有的 ServerInfo 对象
            List<ServerInfo> serverInfos = pipeServerInfoManager.GetAllServerInfo();
            List<ServerInfo> newServerInfos = new List<ServerInfo>();

            // 对这些对象按照 PipeName 进行排序
            //serverInfos.Sort((info1, info2) => info1.CreateTime.CompareTo(info2.CreateTime));

            foreach (ServerInfo serverInfo in serverInfos)
            {
                if (IsPipeExist(serverInfo.PipeName)) // 删除已经不存在的管道服务
                {
                    newServerInfos.Add(serverInfo);
                }
            }

            ServerInfo newServerInfo = new ServerInfo
            {
                PipeName = newPipeName,
                CreateTime = DateTime.Now,
                IsEmpty = false
            };
            newServerInfos.Add(newServerInfo);

            pipeServerInfoManager.UpdateAllServerInfo(newServerInfos); 
        }
    }
}



