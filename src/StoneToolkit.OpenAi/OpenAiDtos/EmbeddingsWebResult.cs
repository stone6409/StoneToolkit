namespace StoneToolkit.OpenAI.OpenAiDtos
{
    public sealed record EmbeddingsWebResult(
        IReadOnlyCollection<EmbeddingWebResult> data,
        string model,
        EmbeddingsUsageWebResult usage);
}