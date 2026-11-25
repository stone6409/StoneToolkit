namespace StoneToolkit.OpenAI.Models
{
    public sealed record ModelsList(
        IReadOnlyCollection<Model> Models);
}