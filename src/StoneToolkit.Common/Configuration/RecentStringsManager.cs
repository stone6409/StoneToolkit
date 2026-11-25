using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoneToolkit.Common.Configuration
{
    public class RecentStringsManager
    {
        private const int MaxRecentItems = 10; // 最多保存的最近使用项数量
        private readonly string _configKey; // 配置文件中对应的键
        private readonly Func<string> _getConfigValue; // 获取配置值的委托
        private readonly Action<string> _setConfigValue; // 设置配置值的委托

        /// <summary>
        /// 初始化 RecentStringsManager
        /// </summary>
        /// <param name="configKey">配置文件中对应的键</param>
        /// <param name="getConfigValue">获取配置值的委托</param>
        /// <param name="setConfigValue">设置配置值的委托</param>
        public RecentStringsManager(string configKey, Func<string> getConfigValue, Action<string> setConfigValue)
        {
            _configKey = configKey;
            _getConfigValue = getConfigValue;
            _setConfigValue = setConfigValue;
        }

        /// <summary>
        /// 获取最近使用的字符串列表
        /// </summary>
        public List<string> GetRecentStrings()
        {
            string recentStringsText = _getConfigValue();
            return recentStringsText?.Split(';').ToList() ?? new List<string>();
        }

        /// <summary>
        /// 更新最近使用的字符串列表
        /// </summary>
        /// <param name="recentString">最近使用的字符串</param>
        public void UpdateRecentStrings(string recentString)
        {
            if (string.IsNullOrWhiteSpace(recentString))
            {
                throw new ArgumentException("最近使用的字符串不能为空或空白。");
            }

            List<string> recentStrings = GetRecentStrings();

            // 如果列表中已经存在该字符串，先移除
            if (recentStrings.Contains(recentString))
            {
                recentStrings.Remove(recentString);
            }

            // 将字符串添加到列表的最前面
            recentStrings.Insert(0, recentString);

            // 如果列表长度超过最大限制，移除最后一个
            while (recentStrings.Count > MaxRecentItems)
            {
                recentStrings.RemoveAt(recentStrings.Count - 1);
            }

            // 将列表保存到配置文件中
            string recentStringsText = string.Join(";", recentStrings);
            _setConfigValue(recentStringsText);
        }

        /// <summary>
        /// 清空最近使用的字符串列表
        /// </summary>
        public void ClearRecentStrings()
        {
            _setConfigValue(string.Empty);
        }
    }
}
