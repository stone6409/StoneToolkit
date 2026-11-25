using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System;

namespace StoneToolkit.Web.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string API_KEY_SCHEME = "Bearer";
        private readonly ApiKeySettings _apiKeySettings;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ApiKeySettings apiKeySettings) : base(options, logger, encoder)
        {
            _apiKeySettings = apiKeySettings;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // 从Authorization头获取API Key
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Task.FromResult(AuthenticateResult.Fail("API Key is missing"));
            }

            var authHeaderValue = authHeader.ToString();
            if (string.IsNullOrEmpty(authHeaderValue))
            {
                return Task.FromResult(AuthenticateResult.Fail("API Key is missing"));
            }

            // 解析Authorization头，格式应为 "ApiKey {apiKey}"
            if (!authHeaderValue.StartsWith(API_KEY_SCHEME + " ", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key format"));
            }

            var apiKey = authHeaderValue.Substring(API_KEY_SCHEME.Length + 1).Trim();

            if (apiKey != _apiKeySettings.ValidApiKey)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "API Client") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)));
        }
    }
}