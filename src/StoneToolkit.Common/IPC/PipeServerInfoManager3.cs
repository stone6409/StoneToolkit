using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace StoneToolkit.Common.IPC
{
    internal class PipeServerInfoManager3
    {
        internal readonly int MaxCapacity = 10;
        private readonly string SharedMemoryName = "PipeNameHelperSharedMemory";
        private MemoryMappedFile memoryMappedFile;
        private MemoryMappedViewStream viewStream;
        private int serverInfoSize = Marshal.SizeOf<ServerInfo>();

        public PipeServerInfoManager3(string pipeName, int maxCapacity = 10)
        {
            SharedMemoryName = $"{pipeName}_SharedMemory";
            InitializeSharedMemory();
        }

        private void InitializeSharedMemory()
        {
            // 创建或打开共享内存文件
            memoryMappedFile = MemoryMappedFile.CreateNew(SharedMemoryName, serverInfoSize * MaxCapacity);
            viewStream = memoryMappedFile.CreateViewStream();
            // 初始化所有ServerInfo对象为Empty
            for (int i = 0; i < MaxCapacity; i++)
            {
                ServerInfo serverInfo = new ServerInfo { PipeName = "", CreateTime = DateTime.MinValue, IsEmpty = true };
                byte[] buffer = StructToBytes(serverInfo);
                viewStream.Write(buffer, 0, buffer.Length);
                viewStream.Seek(i * serverInfoSize, SeekOrigin.Begin);
            }
        }

        public void AddServerInfo(string pipeName)
        {
            ServerInfo serverInfo = new ServerInfo
            {
                PipeName = pipeName,
                CreateTime = DateTime.Now,
                IsEmpty = false
            };

            AddServerInfo(serverInfo);
        }

        public void AddServerInfo(ServerInfo serverInfo)
        {
            // 确保共享内存已初始化
            if (memoryMappedFile == null || viewStream == null)
            {
                throw new InvalidOperationException("Shared memory is not initialized.");
            }

            ServerInfo[] serverInfos = new ServerInfo[MaxCapacity];
            for (int i = 0; i < MaxCapacity; i++)
            {
                byte[] buffer = new byte[serverInfoSize];
                viewStream.Read(buffer, 0, buffer.Length);
                viewStream.Seek(i * serverInfoSize, SeekOrigin.Begin);
                serverInfos[i] = BytesToStruct<ServerInfo>(buffer);
            }

            // 找到第一个空位
            for (int i = 0; i < MaxCapacity; i++)
            {
                if (serverInfos[i].IsEmpty)
                {
                    byte[] buffer = StructToBytes(serverInfo);
                    viewStream.Write(buffer, 0, buffer.Length);
                    viewStream.Seek(i * serverInfoSize, SeekOrigin.Begin);
                    return;
                }
            }

            // 如果没有空位，抛出异常或处理共享内存已满的情况
            throw new InvalidOperationException("Shared memory is full.");
        }

        public void RemoveServerInfo(string pipeName)
        {
            // 确保共享内存已初始化
            if (memoryMappedFile == null || viewStream == null)
            {
                throw new InvalidOperationException("Shared memory is not initialized.");
            }

            ServerInfo[] serverInfos = new ServerInfo[MaxCapacity];
            for (int i = 0; i < MaxCapacity; i++)
            {
                byte[] buffer = new byte[serverInfoSize];
                viewStream.Read(buffer, 0, buffer.Length);
                viewStream.Seek(i * serverInfoSize, SeekOrigin.Begin);
                serverInfos[i] = BytesToStruct<ServerInfo>(buffer);
            }

            // 找到要删除的ServerInfo的索引
            for (int i = 0; i < MaxCapacity; i++)
            {
                if (!serverInfos[i].IsEmpty && serverInfos[i].PipeName == pipeName)
                {
                    ServerInfo emptyServerInfo = new ServerInfo { PipeName = "", CreateTime = DateTime.MinValue, IsEmpty = true };
                    byte[] buffer = StructToBytes(emptyServerInfo);
                    viewStream.Write(buffer, 0, buffer.Length);
                    viewStream.Seek(i * serverInfoSize, SeekOrigin.Begin);
                    return;
                }
            }

            // 如果没有找到，抛出异常或处理未找到的情况
            throw new InvalidOperationException("ServerInfo not found.");
        }

        public void ClearAllServerInfo()
        {
            // 确保共享内存已初始化
            if (memoryMappedFile == null || viewStream == null)
            {
                throw new InvalidOperationException("Shared memory is not initialized.");
            }

            for (int i = 0; i < MaxCapacity; i++)
            {
                ServerInfo emptyServerInfo = new ServerInfo { PipeName = "", CreateTime = DateTime.MinValue, IsEmpty = true };
                byte[] buffer = StructToBytes(emptyServerInfo);
                viewStream.Write(buffer, 0, buffer.Length);
                viewStream.Seek(i * serverInfoSize, SeekOrigin.Begin);
            }
        }

        public List<ServerInfo> GetAllServerInfo()
        {
            List<ServerInfo> serverInfoList = new List<ServerInfo>();
            ServerInfo[] serverInfos = new ServerInfo[MaxCapacity];
            for (int i = 0; i < MaxCapacity; i++)
            {
                byte[] buffer = new byte[serverInfoSize];
                viewStream.Read(buffer, 0, buffer.Length);
                viewStream.Seek(i * serverInfoSize, SeekOrigin.Begin);
                serverInfos[i] = BytesToStruct<ServerInfo>(buffer);
            }

            foreach (ServerInfo serverInfo in serverInfos)
            {
                if (!serverInfo.IsEmpty)
                {
                    serverInfoList.Add(serverInfo);
                }
            }
            return serverInfoList;
        }

        public void DisposeSharedMemory()
        {
            if (viewStream != null)
            {
                viewStream.Dispose();
                viewStream = null;
            }

            if (memoryMappedFile != null)
            {
                memoryMappedFile.Dispose();
                memoryMappedFile = null;
            }
        }

        private byte[] StructToBytes<T>(T structure)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            IntPtr bufferPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, bufferPtr, true);
            Marshal.Copy(bufferPtr, buffer, 0, size);
            Marshal.FreeHGlobal(bufferPtr);
            return buffer;
        }

        private T BytesToStruct<T>(byte[] bytes)
        {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr bufferPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, bufferPtr, size);
            T structure = (T)Marshal.PtrToStructure(bufferPtr, typeof(T));
            Marshal.FreeHGlobal(bufferPtr);
            return structure;
        }
    }
}
