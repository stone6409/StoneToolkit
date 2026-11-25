using StoneToolkit.Common.Utilities;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StoneToolkit.Web.Authentication
{
    public class TokenStorage
    {
        private const string TOKEN_FILE = "auth_tokens.json";
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// 令牌目录覆盖。为 null（默认）时使用 %AppData%\{应用名}，按用户隔离。
        /// 需要跨账户共享同一份登录凭证的宿主（例如以 SYSTEM 身份运行、同时又要被交互式用户使用的后端进程）
        /// 可指向机器级目录，否则 %AppData% 会落到 systemprofile，与用户配置互不可见。
        /// </summary>
        public static string? StorageFolderOverride { get; set; }

        public TokenStorage()
        {
            string appFolder = string.IsNullOrWhiteSpace(StorageFolderOverride)
                ? Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    ApplicationUtility.ApplicationName)
                : StorageFolderOverride!;

            // 确保目录存在；失败不阻断构造（读取会返回 false，写入由 SaveTokens 捕获）
            try
            {
                Directory.CreateDirectory(appFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建令牌目录失败: {ex.Message}");
            }

            // 完整文件路径
            _filePath = Path.Combine(appFolder, TOKEN_FILE);

            // 使用目录覆盖（机器级）且目标文件尚不存在时，从旧的用户级位置迁移一次
            if (!string.IsNullOrWhiteSpace(StorageFolderOverride))
            {
                MigrateLegacyTokens();
            }

            // 配置 JSON 序列化选项
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true, // 替代 Formatting.Indented
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        /// <summary>一次性迁移旧的用户级令牌文件，避免升级后所有终端都要重新登录</summary>
        private void MigrateLegacyTokens()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    return;
                }

                string legacyPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    ApplicationUtility.ApplicationName,
                    TOKEN_FILE);

                if (File.Exists(legacyPath))
                {
                    File.Copy(legacyPath, _filePath, overwrite: false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"迁移令牌文件失败: {ex.Message}");
            }
        }

        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime TokenExpiry { get; set; }

        public void SaveTokens()
        {
            try
            {
                var data = new TokenStorageData
                {
                    AccessToken = AccessToken,
                    RefreshToken = RefreshToken,
                    TokenExpiry = TokenExpiry,
                    SavedAt = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(data, _jsonOptions);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                // 可以根据需要记录日志
                Console.WriteLine($"保存Token失败: {ex.Message}");
            }
        }

        public bool LoadTokens()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return false;

                var json = File.ReadAllText(_filePath);
                var data = JsonSerializer.Deserialize<TokenStorageData>(json, _jsonOptions);

                if (data == null)
                    return false;

                AccessToken = data.AccessToken;
                RefreshToken = data.RefreshToken;
                TokenExpiry = data.TokenExpiry;

                return true;
            }
            catch (Exception ex)
            {
                // 可以根据需要记录日志
                Console.WriteLine($"加载Token失败: {ex.Message}");
                return false;
            }
        }

        public void ClearTokens()
        {
            AccessToken = null;
            RefreshToken = null;
            TokenExpiry = DateTime.MinValue;

            try
            {
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                }
            }
            catch (Exception ex)
            {
                // 可以根据需要记录日志
                Console.WriteLine($"删除Token文件失败: {ex.Message}");
            }
        }

        public bool HasValidToken()
        {
            return !string.IsNullOrEmpty(AccessToken) &&
                   TokenExpiry > DateTime.UtcNow;
        }

        public bool IsTokenExpired()
        {
            return !string.IsNullOrEmpty(AccessToken) &&
                   TokenExpiry <= DateTime.UtcNow;
        }

        // 内部类用于序列化/反序列化
        private class TokenStorageData
        {
            public string? AccessToken { get; set; }
            public string? RefreshToken { get; set; }
            public DateTime TokenExpiry { get; set; }
            public DateTime SavedAt { get; set; }
        }
    }
}