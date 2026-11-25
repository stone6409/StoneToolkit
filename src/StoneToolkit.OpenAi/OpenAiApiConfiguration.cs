namespace StoneToolkit.OpenAI
{
    public sealed record OpenAiApiConfiguration(
        string ApiKey,
        string ApiEndpoint = "https://api.openai.com/v1/",
        string? Proxy = null,
        string ApiRouteCompletions = "completions",
        string ApiRouteChatCompletions = "chat/completions",
        string ApiRouteFiles = "files");
}
