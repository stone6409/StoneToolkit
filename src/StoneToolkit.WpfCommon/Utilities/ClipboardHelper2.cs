using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace StoneToolkit.WpfCommon.Utilities
{
    /// <summary>
    /// 剪贴板操作辅助工具，提供带重试和错误分类的写入能力，
    /// 用于规避 CLIPBRD_E_CANT_OPEN（0x800401D0）异常。
    /// </summary>
    public static class ClipboardHelper2
    {
        /// <summary>
        /// CLIPBRD_E_CANT_OPEN：剪贴板被其他进程暂时占用，重试即可恢复。
        /// </summary>
        private const int CLIPBRD_E_CANT_OPEN = unchecked((int)0x800401D0);

        private const int RetryCount = 10;
        private const int RetryDelayMs = 50;

        /// <summary>
        /// 将文本写入剪贴板。剪贴板被占用（CLIPBRD_E_CANT_OPEN）时按固定间隔重试；
        /// 其余异常（如线程模式不匹配 RPC_E_CHANGED_MODE）重试无意义，立即放弃并记录日志。
        /// </summary>
        /// <param name="text">要写入剪贴板的文本。</param>
        /// <returns>是否写入成功。</returns>
        public static bool TrySetText(string text)
        {
            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    Clipboard.SetText(text);
                    return true;
                }
                catch (COMException ex) when (ex.ErrorCode == CLIPBRD_E_CANT_OPEN)
                {
                    // 剪贴板被其他进程暂时占用，等待后重试
                    Thread.Sleep(RetryDelayMs);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("写入剪贴板失败（非占用错误）: " + ex);
                    return false;
                }
            }

            Debug.WriteLine("写入剪贴板失败：剪贴板持续被占用 (CLIPBRD_E_CANT_OPEN)");
            return false;
        }
    }
}
