using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.Web.Authentication
{
    /// <summary>
    /// WebApplicationBuilder的扩展方法
    /// </summary>
    public static class WebApplicationBuilderExtensions
    {
        /// <summary>
        /// 配置JWT认证服务
        /// </summary>
        /// <param name="builder">WebApplicationBuilder实例</param>
        /// <returns>配置后的WebApplicationBuilder实例</returns>
        public static WebApplicationBuilder ConfigureJwtAuthentication(this WebApplicationBuilder builder)
        {
            // 从自定义配置文件中读取JWT设置
            var customConfig = SecurityConfigHelper.GetCustomConfiguration();
            var jwtSettingsFromCustom = customConfig.GetSection("Jwt");
            var jwtKey = jwtSettingsFromCustom["Key"] ?? "YourSuperSecretKeyHereThatIsAtLeast32CharactersLong";
            var jwtIssuer = jwtSettingsFromCustom["Issuer"] ?? "UserAuthServer";
            var jwtAudience = jwtSettingsFromCustom["Audience"] ?? "UserAuthClient";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // 配置B：手动配置TokenValidationParameters
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // 1. 验证颁发者（Issuer）
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer, // 从自定义配置文件读取

                        // 2. 验证受众（Audience）
                        ValidateAudience = true,
                        ValidAudience = jwtAudience, // 从自定义配置文件读取

                        // 3. 验证有效期
                        ValidateLifetime = true,

                        // 4. 验证签名密钥
                        ValidateIssuerSigningKey = true,
                        // 使用从自定义配置文件读取的密钥进行验证
                        IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtKey)), // 从自定义配置文件读取

                        // 5. 时钟偏移量（避免时间同步问题）
                        ClockSkew = TimeSpan.Zero
                    };

                    // 配置C：事件处理
                    options.Events = new JwtBearerEvents
                    {
                        // 当收到token时触发
                        OnMessageReceived = context =>
                        {
                            // 可以从不同的地方获取token
                            var token = context.Request.Headers["Authorization"]
                                .FirstOrDefault()?.Replace("Bearer ", "");

                            // 或者从cookie获取
                            if (string.IsNullOrEmpty(token))
                                token = context.Request.Cookies["access_token"];

                            context.Token = token;
                            return Task.CompletedTask;
                        },

                        // 认证失败时触发
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"认证失败: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },

                        // token验证成功时触发
                        OnTokenValidated = context =>
                        {
                            // 可以在这里添加额外的验证逻辑
                            var claims = context.Principal.Claims;
                            var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                            // 可以在这里调用UserAuth.Server验证用户状态
                            // 例如检查用户是否被禁用
                            return Task.CompletedTask;
                        }
                    };
                });

            return builder;
        }

        /// <summary>
        /// 配置Kestrel使用HTTPS和自定义证书
        /// </summary>
        /// <param name="builder">WebApplicationBuilder实例</param>
        /// <param name="certificatePath">证书文件路径</param>
        /// <param name="certificatePassword">证书密码</param>
        /// <param name="clientCertificateMode">客户端证书模式，默认为AllowCertificate</param>
        /// <param name="trustAnyClientCertificate">
        /// 是否在TLS层放行任意客户端证书（含自签）。用于反向代理网关：
        /// 端点级为AllowCertificate，但需要自签客户端证书能通过握手，安全边界交由上层按路径+指纹白名单校验。
        /// </param>
        /// <returns>配置后的WebApplicationBuilder实例</returns>
        public static WebApplicationBuilder ConfigureKestrelHttps(
            this WebApplicationBuilder builder,
            string certificatePath,
            string certificatePassword,
            ClientCertificateMode clientCertificateMode = ClientCertificateMode.AllowCertificate,
            string allowedClientThumbprints = "",
            bool trustAnyClientCertificate = false)
        {
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.ConfigureHttpsDefaults(httpsOptions =>
                {
                    // 配置客户端证书模式
                    httpsOptions.ClientCertificateMode = clientCertificateMode;

                    // 客户端证书指纹白名单（信任方式 A）：按指纹放行
                    var whitelist = new ClientCertificateWhitelist(allowedClientThumbprints);
                    if (trustAnyClientCertificate)
                    {
                        // 网关场景：TLS层放行任意客户端证书（含自签），由上层中间件按路径校验
                        httpsOptions.ClientCertificateValidation = (cert, chain, errors) => true;
                    }
                    else if (clientCertificateMode == ClientCertificateMode.RequireCertificate)
                    {
                        httpsOptions.ClientCertificateValidation = (cert, chain, errors) =>
                            whitelist.Validate(cert, chain, errors);
                    }

                    // 加载服务器证书
                    if (File.Exists(certificatePath))
                    {
                        // 使用X509CertificateLoader加载证书（推荐）
                        httpsOptions.ServerCertificate = X509CertificateLoader.LoadPkcs12FromFile(
                            certificatePath, certificatePassword);
                    }
                    else
                    {
                        throw new FileNotFoundException($"证书文件未找到: {certificatePath}");
                    }
                });
            });

            return builder;
        }

        /// <summary>
        /// 配置Kestrel使用HTTPS和自定义证书（从配置读取证书信息）
        /// 优先读取自定义配置文件，如果不存在则使用主配置文件
        /// </summary>
        /// <param name="builder">WebApplicationBuilder实例</param>
        /// <param name="configurationSection">配置节点名称，默认为"Kestrel:Certificate"</param>
        /// <param name="trustAnyClientCertificate">是否在TLS层放行任意客户端证书（网关场景）</param>
        /// <returns>配置后的WebApplicationBuilder实例</returns>
        public static WebApplicationBuilder ConfigureKestrelHttpsFromConfig(
            this WebApplicationBuilder builder,
            string configurationSection = "Certificate",
            bool trustAnyClientCertificate = false)
        {
            // 1. 首先尝试从自定义配置文件读取
            var customConfig = SecurityConfigHelper.GetCustomConfiguration();
            var customCertificateConfig = customConfig.GetSection(configurationSection);
            
            string certificatePath = null;
            string certificatePassword = null;
            string clientCertificateMode = null;
            string allowedClientThumbprints = null;

            // 检查自定义配置中是否有证书配置
            if (!string.IsNullOrEmpty(customCertificateConfig["Path"]))
            {
                certificatePath = customCertificateConfig["Path"];
                certificatePassword = customCertificateConfig["Password"];
                clientCertificateMode = customCertificateConfig["ClientCertificateMode"];
                allowedClientThumbprints = customCertificateConfig["AllowedClientThumbprints"];
                
                Console.WriteLine($"从自定义配置文件读取证书配置: {certificatePath}");
            }
            else
            {
                // 2. 如果自定义配置中没有，则使用主配置文件
                var mainCertificateConfig = builder.Configuration.GetSection(configurationSection);
                certificatePath = mainCertificateConfig["Path"];
                certificatePassword = mainCertificateConfig["Password"];
                clientCertificateMode = mainCertificateConfig["ClientCertificateMode"];
                allowedClientThumbprints = mainCertificateConfig["AllowedClientThumbprints"];
                
                Console.WriteLine($"从主配置文件读取证书配置: {certificatePath}");
            }

            // 3. 如果都没有配置，使用默认开发证书
            if (string.IsNullOrEmpty(certificatePath))
            {
                Console.WriteLine("未找到证书配置，使用默认开发证书");
                return builder.ConfigureKestrelHttpsForDevelopment();
            }

            // 解析证书路径（支持相对路径和绝对路径）
            string resolvedCertificatePath = ResolveCertificatePath(certificatePath);

            // 解析客户端证书模式
            ClientCertificateMode mode = ClientCertificateMode.AllowCertificate;
            if (!string.IsNullOrEmpty(clientCertificateMode))
            {
                if (Enum.TryParse<ClientCertificateMode>(clientCertificateMode, out var parsedMode))
                {
                    mode = parsedMode;
                }
            }

            return builder.ConfigureKestrelHttps(
                resolvedCertificatePath,
                certificatePassword,
                mode,
                allowedClientThumbprints ?? "",
                trustAnyClientCertificate);
        }

        /// <summary>
        /// 解析证书路径
        /// </summary>
        private static string ResolveCertificatePath(string certificatePath)
        {
            // 如果是绝对路径，直接返回
            if (Path.IsPathRooted(certificatePath))
            {
                return certificatePath;
            }

            // 如果是相对路径，尝试从多个位置查找
            // 1. 相对于应用程序基目录
            var appBasePath = Path.Combine(AppContext.BaseDirectory, certificatePath);
            if (File.Exists(appBasePath))
            {
                return appBasePath;
            }

            // 2. 相对于自定义配置文件所在目录
            var customConfigDir = Path.GetDirectoryName(SecurityConfigHelper.GetCustomConfigPath());
            if (!string.IsNullOrEmpty(customConfigDir))
            {
                var customConfigPath = Path.Combine(customConfigDir, certificatePath);
                if (File.Exists(customConfigPath))
                {
                    return customConfigPath;
                }
            }

            // 3. 相对于当前工作目录
            var currentDirPath = Path.Combine(Directory.GetCurrentDirectory(), certificatePath);
            if (File.Exists(currentDirPath))
            {
                return currentDirPath;
            }

            // 如果都找不到，返回原始路径（会在ConfigureKestrelHttps中抛出异常）
            return certificatePath;
        }

        /// <summary>
        /// 配置Kestrel使用HTTPS和开发证书
        /// </summary>
        /// <param name="builder">WebApplicationBuilder实例</param>
        /// <param name="relativePath">证书相对路径，默认为"Certificates\\LocalDev_Certificate.pfx"</param>
        /// <param name="password">证书密码，默认为"1234"</param>
        /// <returns>配置后的WebApplicationBuilder实例</returns>
        public static WebApplicationBuilder ConfigureKestrelHttpsForDevelopment(
            this WebApplicationBuilder builder,
            string relativePath = "Certificates\\LocalDev_Certificate.pfx",
            string password = "1234")
        {
            string certificatePath = Path.Combine(AppContext.BaseDirectory, relativePath);
            return builder.ConfigureKestrelHttps(certificatePath, password);
        }

        /// <summary>
        /// 添加自定义配置文件到配置源
        /// </summary>
        /// <param name="builder">WebApplicationBuilder实例</param>
        /// <returns>配置后的WebApplicationBuilder实例</returns>
        public static WebApplicationBuilder AddCustomConfigurationFile(this WebApplicationBuilder builder)
        {
            string customConfigPath = SecurityConfigHelper.GetCustomConfigPath();
            
            if (File.Exists(customConfigPath))
            {
                builder.Configuration.AddJsonFile(customConfigPath, optional: true, reloadOnChange: true);
                Console.WriteLine($"加载自定义配置文件: {customConfigPath}");
            }
            else
            {
                Console.WriteLine($"自定义配置文件不存在: {customConfigPath}");
            }

            return builder;
        }
    }
}