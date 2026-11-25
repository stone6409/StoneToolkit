using System.Text;

namespace StoneToolkit.Common.ConsoleUI
{
    /// <summary>
    /// 控制台字符宽度计算与定宽裁剪（纯函数，无状态）。
    /// </summary>
    public static class ConsoleTextMetrics
    {
        /// <summary>字符显示宽度：CJK / 全角字符占 2 列，其余占 1 列。</summary>
        public static int GetCharWidth(char c)
        {
            if (c >= 0x1100 && (
                c <= 0x115F ||
                c == 0x2329 || c == 0x232A ||
                (c >= 0x2E80 && c <= 0xA4CF && c != 0x303F) ||
                (c >= 0xAC00 && c <= 0xD7A3) ||
                (c >= 0xF900 && c <= 0xFAFF) ||
                (c >= 0xFE30 && c <= 0xFE6F) ||
                (c >= 0xFF00 && c <= 0xFF60) ||
                (c >= 0xFFE0 && c <= 0xFFE6) ||
                (c >= 0x20000 && c <= 0x3FFFD)))
                return 2;
            return 1;
        }

        /// <summary>字符串显示宽度（按列计）。</summary>
        public static int GetDisplayWidth(string text)
        {
            if (text == null)
                return 0;

            int w = 0;
            foreach (char c in text)
                w += GetCharWidth(c);
            return w;
        }

        /// <summary>截断/补白到指定显示宽度（按列计），保证整行覆盖不残留旧内容。</summary>
        public static string FitToWidth(string text, int width)
        {
            if (width <= 0)
                return string.Empty;

            if (text == null)
                text = string.Empty;

            var sb = new StringBuilder(width);
            int w = 0;
            foreach (char c in text)
            {
                int cw = GetCharWidth(c);
                if (w + cw > width)
                    break;
                sb.Append(c);
                w += cw;
            }

            if (w < width)
                sb.Append(' ', width - w);
            return sb.ToString();
        }
    }
}
