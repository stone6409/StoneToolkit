using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace StoneToolkit.Common.Utilities
{
    public static class StringHelper
    {
        public static string TruncateText(string multilineText, int maxLength = 100)
        {
            // 将多行文本转换为一行文本（替换换行符和回车符）
            string singleLineText = multilineText.Replace("\n", " ").Replace("\r", "");

            // 连续的空格合并成一个空格
            singleLineText = Regex.Replace(singleLineText, @"\s+", " ").Trim();

            // 限制文本长度并加上省略号
            return singleLineText.Length > maxLength
                ? singleLineText.Substring(0, maxLength) + "..."
                : singleLineText;
        }
    }
}
