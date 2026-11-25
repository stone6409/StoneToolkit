namespace StoneToolkit.OpenAI.Http
{
    public static class IHttpClientExtensions
    {
        public static async Task<string> SendRequestAndReceiveStringAsync(
            this IHttpClient httpClient,
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await httpClient
                .SendAsync(
                    request,
                    HttpCompletionOption.ResponseContentRead,
                    cancellationToken)
                .ConfigureAwait(false);
            var responseString = await response
                .Content
                .ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"The response ({response.StatusCode}) indicated that the " +
                    $"request was not successful." +
                    $"\r\n{responseString}");
            }

            return responseString;
        }
    }
}
