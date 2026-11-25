using System.Numerics;

namespace StoneToolkit.OpenAI.OpenAiDtos
{
    public sealed record EmbeddingWebResult(
        float[] embedding,
        int index);
}