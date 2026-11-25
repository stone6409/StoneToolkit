namespace StoneToolkit.OpenAI.FineTunes
{
    public sealed record FineTuneHyperparams(
        int? BatchSize,
        float? LearningRateMultiplier,
        int? NumberOfEpochs,
        float? PromptLossWeight);
}
