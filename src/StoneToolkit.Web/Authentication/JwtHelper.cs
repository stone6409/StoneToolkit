using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace StoneToolkit.Web.Authentication
{
    public static class JwtHelper
    {
        public static Dictionary<string, string> DecodeToken(string token)
        {
            var result = new Dictionary<string, string>();

            try
            {
                var handler = new JwtSecurityTokenHandler();
                
                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    
                    result["Subject"] = jwtToken.Subject;
                    result["Issuer"] = jwtToken.Issuer;
                    result["ValidTo"] = jwtToken.ValidTo.ToString("yyyy-MM-dd HH:mm:ss");
                    
                    foreach (var claim in jwtToken.Claims)
                    {
                        if (!result.ContainsKey(claim.Type))
                        {
                            result[claim.Type] = claim.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result["Error"] = $"解析失败: {ex.Message}";
            }

            return result;
        }

        public static bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }
    }
}