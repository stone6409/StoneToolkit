using StoneToolkit.OpenAI.Chat.Completions;
using StoneToolkit.OpenAI.Completions;
using StoneToolkit.OpenAI.Embeddings;
using StoneToolkit.OpenAI.Files;
using StoneToolkit.OpenAI.FineTunes;
using StoneToolkit.OpenAI.Http;
using StoneToolkit.OpenAI.Models;

namespace StoneToolkit.OpenAI;

/// <summary>
/// The default implementation of <see cref="IOpenAiApiClientFactory"/>.
/// </summary>
public sealed class OpenAiApiClientFactory : IOpenAiApiClientFactory
{
    private readonly IOpenAiHttpClientWrapperFactory _openAiHttpClientWrapperFactory;

    /// <summary>
    /// Creates an instance of <see cref="OpenAiApiClientFactory"/>.
    /// </summary>
    /// <param name="openAiHttpClientWrapperFactory">
    /// The <see cref="IOpenAiHttpClientWrapperFactory"/> to use when 
    /// creating <see cref="IHttpClient"/> instances.
    /// </param>
    public OpenAiApiClientFactory(
        IOpenAiHttpClientWrapperFactory openAiHttpClientWrapperFactory)
    {
        _openAiHttpClientWrapperFactory = openAiHttpClientWrapperFactory;
    }

    public IOpenAiApiClient Create(OpenAiApiConfiguration configuration)
    {
        var httpClient = _openAiHttpClientWrapperFactory.Create(configuration);

        var fileInfoWebResultConverter = new FileInfoWebResultConverter();
        var filesApi = new FilesApiClient(
            configuration,
            httpClient,
            fileInfoWebResultConverter);

        var fineTuneWebResultConverter = new FineTuneWebResultConverter(fileInfoWebResultConverter);
        var fineTuneApi = new FineTunesApiClient(
            httpClient,
            fineTuneWebResultConverter);
        var modelsApi = new ModelsApiClient(httpClient);
        var completionsApi = new CompletionsApiClient(configuration, httpClient);
        var chatCompletionsApi = new ChatCompletionsApiClient(configuration, httpClient);
        var embeddingsApi = new EmbeddingsApiClient(httpClient);
        var openAiApiClient = new OpenAiApiClient(
            fineTuneApi,
            filesApi,
            modelsApi,
            completionsApi,
            chatCompletionsApi,
            embeddingsApi,
            () => httpClient.Dispose());
        return openAiApiClient;
    }
}
