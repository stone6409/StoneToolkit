using Microsoft.Extensions.Configuration;
using StoneToolkit.Common.Utilities;
using System;
using System.IO;

namespace StoneToolkit.Web.Authentication
{
    public static class SecurityConfigHelper
    {
        private const string MySecurityConfigFolder = @"D:\src\ScTrials\.webserver\SecurityConfig";

        public static string GetMyApplicationDataFolder()
        {
            if (Directory.Exists(MySecurityConfigFolder))
            {
                return MySecurityConfigFolder;
            }

            return GetApplicationDataFolder();
        }

        private static string GetApplicationDataFolder()
        {
            string applicationName = ApplicationUtility.ApplicationName;
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            folderPath = Path.Combine(folderPath, applicationName);

            return folderPath;
        }

        /// <summary>
        /// 获取自定义配置文件路径
        /// </summary>
        public static string GetCustomConfigPath()
        {
            return Path.Combine(GetMyApplicationDataFolder(), "appsettings.json");
        }

        /// <summary>
        /// 从自定义配置文件读取配置
        /// </summary>
        public static IConfiguration GetCustomConfiguration()
        {
            var customConfigPath = GetCustomConfigPath();
            var configBuilder = new ConfigurationBuilder();

            if (File.Exists(customConfigPath))
            {
                configBuilder.AddJsonFile(customConfigPath, optional: true, reloadOnChange: true);
            }

            return configBuilder.Build();
        }

        /// <summary>
        /// 获取有效的API密钥
        /// 优先从自定义配置文件读取，如果不存在则使用默认值
        /// </summary>
        /// <param name="defaultApiKey">默认API密钥</param>
        /// <returns>有效的API密钥</returns>
        public static string GetValidApiKey(string defaultApiKey = "your-secret-key-123")
        {
            var customConfig = GetCustomConfiguration();

            // 尝试从自定义配置文件中读取API密钥
            // 支持两种配置格式：
            // 1. "ApiKey": "your-secret-key-123" (简单格式)
            // 2. "ApiKey": { "ValidApiKey": "your-secret-key-123" } (嵌套格式)

            var apiKey = customConfig["ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = customConfig["ApiKey:ValidApiKey"];
            }

            return !string.IsNullOrEmpty(apiKey) ? apiKey : defaultApiKey;
        }
    }
}