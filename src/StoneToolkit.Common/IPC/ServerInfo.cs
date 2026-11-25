using System;
using System.Runtime.InteropServices;

namespace StoneToolkit.Common.IPC
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ServerInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] // 假设PipeName最大长度为256
        public string PipeName;
        public DateTime CreateTime;
        public bool IsEmpty; // 标记是否为空
    }
}
