namespace StoneToolkit.OpenAI.Http
{
    public interface IOpenAiHttpClientFactory
    {
        HttpClient Create(OpenAiApiConfiguration config);
    }
}