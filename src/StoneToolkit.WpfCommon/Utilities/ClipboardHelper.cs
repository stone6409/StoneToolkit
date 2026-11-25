using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace StoneToolkit.WpfCommon.Utilities
{
    /// <summary>
    /// 剪贴板操作辅助工具，基于 Win32 API 直接操作剪贴板，
    /// 规避了 WPF Clipboard 的 OLE 依赖及其在剪贴板被占用时
    /// 抛出的 CLIPBRD_E_CANT_OPEN（0x800401D0）异常。
    /// </summary>
    public static class ClipboardHelper
    {
        /// <summary>
        /// 剪贴板格式：CF_UNICODETEXT。
        /// </summary>
        private const uint CF_UNICODETEXT = 13;

        /// <summary>
        /// GlobalAlloc 标志：可移动内存。
        /// </summary>
        private const uint GMEM_MOVEABLE = 0x0002;

        private const int RetryCount = 10;
        private const int RetryDelayMs = 50;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalFree(IntPtr hMem);

        /// <summary>
        /// 将文本写入剪贴板。剪贴板被占用（OpenClipboard 失败，等价于 CLIPBRD_E_CANT_OPEN）时按固定间隔重试；
        /// 其余异常（如内存分配失败）重试无意义，立即放弃并记录日志。
        /// </summary>
        /// <param name="text">要写入剪贴板的文本。</param>
        /// <returns>是否写入成功。</returns>
        public static bool TrySetText(string text)
        {
            if (text == null)
            {
                return false;
            }

            for (int i = 0; i < RetryCount; i++)
            {
                if (OpenClipboard(IntPtr.Zero))
                {
                    try
                    {
                        if (!EmptyClipboard())
                        {
                            Debug.WriteLine("清空剪贴板失败: " + Marshal.GetLastWin32Error());
                            return false;
                        }

                        IntPtr hGlobal = SetTextData(text);
                        if (hGlobal == IntPtr.Zero)
                        {
                            return false;
                        }

                        if (SetClipboardData(CF_UNICODETEXT, hGlobal) == IntPtr.Zero)
                        {
                            // 设置失败，内存所有权尚未移交，需手动释放。
                            GlobalFree(hGlobal);
                            Debug.WriteLine("设置剪贴板数据失败: " + Marshal.GetLastWin32Error());
                            return false;
                        }

                        // SetClipboardData 成功后内存所有权已归系统，无需 GlobalFree。
                        return true;
                    }
                    finally
                    {
                        CloseClipboard();
                    }
                }

                // 剪贴板被其他进程占用，等待后重试。
                Thread.Sleep(RetryDelayMs);
            }

            Debug.WriteLine("写入剪贴板失败：剪贴板持续被占用 (CLIPBRD_E_CANT_OPEN)");
            return false;
        }

        /// <summary>
        /// 将文本以 Unicode 格式写入可移动内存，返回 GlobalAlloc 句柄。
        /// 调用方需在 SetClipboardData 失败时调用 GlobalFree 释放。
        /// </summary>
        private static IntPtr SetTextData(string text)
        {
            // 文本字节数（Unicode，每字符 2 字节），包含结尾的 '\0'。
            int byteCount = (text.Length + 1) * sizeof(char);
            IntPtr hGlobal = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)byteCount);
            if (hGlobal == IntPtr.Zero)
            {
                Debug.WriteLine("GlobalAlloc 失败: " + Marshal.GetLastWin32Error());
                return IntPtr.Zero;
            }

            IntPtr pData = GlobalLock(hGlobal);
            if (pData == IntPtr.Zero)
            {
                Debug.WriteLine("GlobalLock 失败: " + Marshal.GetLastWin32Error());
                GlobalFree(hGlobal);
                return IntPtr.Zero;
            }

            try
            {
                char[] buffer = new char[text.Length + 1];
                text.CopyTo(0, buffer, 0, text.Length);
                buffer[text.Length] = '\0';
                Marshal.Copy(buffer, 0, pData, buffer.Length);
            }
            finally
            {
                GlobalUnlock(hGlobal);
            }

            return hGlobal;
        }
    }
}
