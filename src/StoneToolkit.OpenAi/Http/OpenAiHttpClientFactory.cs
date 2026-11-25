using System.Net;
using NexusLabs.Contracts;

namespace StoneToolkit.OpenAI.Http
{
    public sealed class OpenAiHttpClientFactory : IOpenAiHttpClientFactory
    {
        public HttpClient Create(OpenAiApiConfiguration config)
        {
            ArgumentContract.RequiresNotNull(config, nameof(config));

            //var httpClient = new HttpClient();
            HttpClientHandler handler = new HttpClientHandler
            {
                Proxy = new WebProxy(config.Proxy)
            };
            HttpClient httpClient = config.Proxy == null ? new() : new(handler);

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.ApiKey}");
            return httpClient;
        }
    }
}
