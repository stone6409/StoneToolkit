namespace StoneToolkit.OpenAI.Embeddings
{
    public sealed record Embedding(
        int Index,
        IReadOnlyList<float> Vectors);
}