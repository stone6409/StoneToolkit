namespace StoneToolkit.OpenAI.OpenAiDtos
{
    internal sealed record ListFineTunesWebResult(
        string @object,
        FineTuneWebResult[] data);
}
