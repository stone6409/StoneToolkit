namespace StoneToolkit.OpenAI.OpenAiDtos;

internal sealed record StreamChatCompletionWebResult(
    string id,
    string @object,
    long created,
    string model,
    StreamChatCompletionChoiceWebResult[] choices,
    CompletionUsageWebResult usage);
