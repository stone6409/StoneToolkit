using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace StoneToolkit.Web.Authentication
{
    /// <summary>
    /// mTLS 客户端证书指纹白名单：仅放行指纹匹配的客户端证书。
    /// 配合 Kestrel 的 ClientCertificateValidation 使用（信任方式 A）。
    /// </summary>
    public class ClientCertificateWhitelist
    {
        private readonly HashSet<string> _allowedThumbprints;

        public ClientCertificateWhitelist(string thumbprints)
        {
            _allowedThumbprints = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(thumbprints))
            {
                foreach (var t in thumbprints.Split(new[] { ';', ',', '|' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (!string.IsNullOrWhiteSpace(t))
                        _allowedThumbprints.Add(t);
                }
            }
        }

        public bool IsEmpty => _allowedThumbprints.Count == 0;

        public bool Contains(string thumbprint)
        {
            return !string.IsNullOrWhiteSpace(thumbprint) && _allowedThumbprints.Contains(thumbprint);
        }

        /// <summary>
        /// 校验客户端证书：指纹在名单内才放行；名单为空则放行（宽松模式，用于调试/过渡）。
        /// </summary>
        public bool Validate(X509Certificate2 cert, X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            if (cert is null)
                return false;

            // 名单为空 => 宽松放行（避免误伤，先不强制）
            if (IsEmpty)
                return true;

            return Contains(cert.Thumbprint);
        }
    }
}
