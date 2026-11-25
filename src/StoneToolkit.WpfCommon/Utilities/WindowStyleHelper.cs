using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace StoneToolkit.WpfCommon.Utilities
{
    /// <summary>
    /// 窗口样式辅助类
    /// </summary>
    public static class WindowStyleHelper
    {
        // Win32 API常量
        private const int GWL_STYLE = -16;
        private const int WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// 禁用窗口的最小化按钮
        /// </summary>
        /// <param name="window">目标窗口</param>
        public static void DisableMinimizeButton(System.Windows.Window window)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));

            var hwnd = new WindowInteropHelper(window).Handle;
            DisableMinimizeButton(hwnd);
        }

        /// <summary>
        /// 启用窗口的最小化按钮
        /// </summary>
        /// <param name="window">目标窗口</param>
        public static void EnableMinimizeButton(System.Windows.Window window)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));

            var hwnd = new WindowInteropHelper(window).Handle;
            EnableMinimizeButton(hwnd);
        }

        /// <summary>
        /// 直接通过窗口句柄禁用最小化按钮
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        public static void DisableMinimizeButton(IntPtr hwnd)
        {
            var style = GetWindowLong(hwnd, GWL_STYLE);
            SetWindowLong(hwnd, GWL_STYLE, style & ~WS_MINIMIZEBOX);
        }

        /// <summary>
        /// 直接通过窗口句柄启用最小化按钮
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        public static void EnableMinimizeButton(IntPtr hwnd)
        {
            var style = GetWindowLong(hwnd, GWL_STYLE);
            SetWindowLong(hwnd, GWL_STYLE, style | WS_MINIMIZEBOX);
        }
    }
}