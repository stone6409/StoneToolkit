namespace StoneToolkit.OpenAI.OpenAiDtos
{
    public sealed record EmbeddingsUsageWebResult(
        int prompt_tokens,
        int total_tokens);
}