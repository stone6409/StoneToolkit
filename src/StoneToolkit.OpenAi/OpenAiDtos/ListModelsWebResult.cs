namespace StoneToolkit.OpenAI.OpenAiDtos
{
    internal sealed record ListModelsWebResult(
        string @object,
        ModelWebResult[] data);
}
