namespace StoneToolkit.OpenAI.FineTunes
{
    public sealed record FineTuneParameters(
        string TrainingFileId,
        string? BaseModelId = null,
        int? NumberOfEpochs = null,
        int? BatchSize = null,
        float? LearningRateMultiplier = null);
}
