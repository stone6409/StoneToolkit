using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;

namespace StoneToolkit.Common.IPC
{
    internal class PipeServerInfoManager
    {
        internal readonly int MaxCapacity = 10;
        private readonly string SharedMemoryName = "PipeNameSharedMemory";
        private MemoryMappedFile memoryMappedFile;
        private MemoryMappedViewAccessor viewAccessor;
        private int serverInfoSize = Marshal.SizeOf<ServerInfo>();

        private static Mutex _mutex;

        private static Dictionary<string, PipeServerInfoManager> _pipeServerInfoManagerMap;

        public static Dictionary<string, PipeServerInfoManager> PipeServerInfoManagerMap
        {
            get
            {
                if (_pipeServerInfoManagerMap == null)
                {
                    _pipeServerInfoManagerMap = new Dictionary<string, PipeServerInfoManager>();
                }

                return _pipeServerInfoManagerMap;
            }
        }

        public static PipeServerInfoManager GetPipeServerInfoManager(string pipeName)
        {
            if (_mutex == null)
            {
                if (Mutex.TryOpenExisting("PipeNameSharedMemoryMutex", out _mutex))
                {
                    _mutex.WaitOne();
                }
                else
                {
                    _mutex = new Mutex(true, "PipeNameSharedMemoryMutex");
                }
            }
            else
            {
                _mutex.WaitOne();
            }

            PipeServerInfoManager pipeServerInfoManager = null;
            if (!PipeServerInfoManagerMap.TryGetValue(pipeName, out pipeServerInfoManager))
            {
                pipeServerInfoManager = new PipeServerInfoManager(pipeName);
                PipeServerInfoManagerMap.Add(pipeName, pipeServerInfoManager);
            }

            _mutex.ReleaseMutex();

            return pipeServerInfoManager;
        }

        private PipeServerInfoManager(string pipeName, int maxCapacity = 16)
        {
            SharedMemoryName = $"{pipeName}_SharedMemory";
            MaxCapacity = maxCapacity;

            _mutex.WaitOne();
            InitializeSharedMemory();
            _mutex.ReleaseMutex();
        }

        private void InitializeSharedMemory()
        {
            // 创建或打开共享内存文件
            //memoryMappedFile = MemoryMappedFile.CreateOrOpen(SharedMemoryName, serverInfoSize * MaxCapacity, 
            //    MemoryMappedFileAccess.ReadWrite,
            //    MemoryMappedFileOptions.None, HandleInheritability.Inheritable);

            //viewAccessor = memoryMappedFile.CreateViewAccessor();

            try
            {
                memoryMappedFile = MemoryMappedFile.OpenExisting(SharedMemoryName, MemoryMappedFileRights.ReadWrite);
                viewAccessor = memoryMappedFile.CreateViewAccessor();
            }
            catch (FileNotFoundException)
            {
                memoryMappedFile = MemoryMappedFile.CreateNew(SharedMemoryName, serverInfoSize * MaxCapacity);
                viewAccessor = memoryMappedFile.CreateViewAccessor();

                // 初始化所有ServerInfo对象为Empty
                for (int i = 0; i < MaxCapacity; i++)
                {
                    ServerInfo serverInfo = new ServerInfo { PipeName = "", CreateTime = DateTime.MinValue, IsEmpty = true };
                    byte[] bytes = StructToBytes(serverInfo);
                    viewAccessor.WriteArray(i * serverInfoSize, bytes, 0, bytes.Length);
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void AddServerInfo(string pipeName)
        {
            ServerInfo serverInfo = new ServerInfo
            {
                PipeName = pipeName,
                CreateTime = DateTime.Now,
                IsEmpty = false
            };

            AddServerInfo(serverInfo);
        }

        private void AddServerInfo(ServerInfo serverInfo)
        {
            // 确保共享内存已初始化
            if (memoryMappedFile == null || viewAccessor == null)
            {
                throw new InvalidOperationException("Shared memory is not initialized.");
            }

            // 读取所有ServerInfo字节
            byte[] allBytes = new byte[serverInfoSize * MaxCapacity];
            viewAccessor.ReadArray(0, allBytes, 0, allBytes.Length);

            // 使用BytesToStruct转换成ServerInfo[]
            ServerInfo[] serverInfos = new ServerInfo[MaxCapacity];
            for (int i = 0; i < MaxCapacity; i++)
            {
                serverInfos[i] = BytesToStruct<ServerInfo>(allBytes, i * serverInfoSize);
            }

            // 将serverInfos进行排序，排序规则：ServerInfo对象IsEmpty为false的排在前面，都为false的按CreateTime从早到晚排列
            //Array.Sort(serverInfos, (x, y) =>
            //{
            //    if (x.IsEmpty && !y.IsEmpty)
            //    {
            //        return 1;
            //    }
            //    else if (!x.IsEmpty && y.IsEmpty)
            //    {
            //        return -1;
            //    }
            //    else
            //    {
            //        return x.CreateTime.CompareTo(y.CreateTime);
            //    }
            //});

            // 找到第一个空位
            for (int i = 0; i < MaxCapacity; i++)
            {
                if (serverInfos[i].IsEmpty)
                {
                    serverInfos[i] = serverInfo;
                    break;
                }
            }

            for (int i = 0; i < MaxCapacity; i++)
            {
                byte[] bytes = StructToBytes(serverInfos[i]);
                viewAccessor.WriteArray(i * serverInfoSize, bytes, 0, bytes.Length);
            }
        }

        private void RemoveServerInfo(string pipeName)
        {
            // 确保共享内存已初始化
            if (memoryMappedFile == null || viewAccessor == null)
            {
                throw new InvalidOperationException("Shared memory is not initialized.");
            }

            // 读取所有ServerInfo字节
            byte[] allBytes = new byte[serverInfoSize * MaxCapacity];
            viewAccessor.ReadArray(0, allBytes, 0, allBytes.Length);

            // 使用BytesToStruct转换成ServerInfo[]
            ServerInfo[] serverInfos = new ServerInfo[MaxCapacity];
            for (int i = 0; i < MaxCapacity; i++)
            {
                serverInfos[i] = BytesToStruct<ServerInfo>(allBytes, i * serverInfoSize);
            }

            // 找到要删除的ServerInfo的索引
            for (int i = 0; i < MaxCapacity; i++)
            {
                if (!serverInfos[i].IsEmpty && serverInfos[i].PipeName == pipeName)
                {
                    ServerInfo emptyServerInfo = new ServerInfo { PipeName = "", CreateTime = DateTime.MinValue, IsEmpty = true };
                    byte[] bytes = StructToBytes(emptyServerInfo);
                    viewAccessor.WriteArray(i * serverInfoSize, bytes, 0, bytes.Length);
                    return;
                }
            }

            // 如果没有找到，抛出异常或处理未找到的情况
            throw new InvalidOperationException("ServerInfo not found.");
        }

        private void ClearAllServerInfo()
        {
            // 确保共享内存已初始化
            if (memoryMappedFile == null || viewAccessor == null)
            {
                throw new InvalidOperationException("Shared memory is not initialized.");
            }

            for (int i = 0; i < MaxCapacity; i++)
            {
                ServerInfo emptyServerInfo = new ServerInfo { PipeName = "", CreateTime = DateTime.MinValue, IsEmpty = true };
                byte[] bytes = StructToBytes(emptyServerInfo);
                viewAccessor.WriteArray(i * serverInfoSize, bytes, 0, bytes.Length);
            }
        }

        public List<ServerInfo> GetAllServerInfo()
        {

            List<ServerInfo> serverInfoList = new List<ServerInfo>();
            // 读取所有ServerInfo字节
            byte[] allBytes = new byte[serverInfoSize * MaxCapacity];

            _mutex.WaitOne();
            viewAccessor.ReadArray(0, allBytes, 0, allBytes.Length);
            _mutex.ReleaseMutex();

            // 使用BytesToStruct转换成ServerInfo[]
            ServerInfo[] serverInfos = new ServerInfo[MaxCapacity];
            for (int i = 0; i < MaxCapacity; i++)
            {
                serverInfos[i] = BytesToStruct<ServerInfo>(allBytes, i * serverInfoSize);
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

        public void UpdateAllServerInfo(List<ServerInfo> serverInfos)
        {
            int i;

            _mutex.WaitOne();

            for (i = 0; i < serverInfos.Count && i < MaxCapacity; i++)
            {
                ServerInfo serverInfo = serverInfos[i];
                byte[] bytes = StructToBytes(serverInfo);
                viewAccessor.WriteArray(i * serverInfoSize, bytes, 0, bytes.Length);
            }

            for (; i < MaxCapacity; i++)
            {
                ServerInfo serverInfo = new ServerInfo { PipeName = "", CreateTime = DateTime.MinValue, IsEmpty = true };
                byte[] bytes = StructToBytes(serverInfo);
                viewAccessor.WriteArray(i * serverInfoSize, bytes, 0, bytes.Length);
            }

            _mutex.ReleaseMutex();
        }

        private void DisposeSharedMemory()
        {
            if (viewAccessor != null)
            {
                viewAccessor.Dispose();
                viewAccessor = null;
            }

            if (memoryMappedFile != null)
            {
                memoryMappedFile.Dispose();
                memoryMappedFile = null;
            }
        }

        private static byte[] StructToBytes<T>(T structure)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            IntPtr bufferPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, bufferPtr, true);
            Marshal.Copy(bufferPtr, buffer, 0, size);
            Marshal.FreeHGlobal(bufferPtr);
            return buffer;
        }

        private static T BytesToStruct<T>(byte[] bytes, int offset)
        {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr bufferPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, offset, bufferPtr, size);
            T structure = (T)Marshal.PtrToStructure(bufferPtr, typeof(T));
            Marshal.FreeHGlobal(bufferPtr);
            return structure;
        }
    }
}