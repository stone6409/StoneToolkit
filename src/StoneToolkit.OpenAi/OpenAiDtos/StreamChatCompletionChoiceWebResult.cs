namespace StoneToolkit.OpenAI.OpenAiDtos;

internal sealed record StreamChatCompletionChoiceWebResult(
    int index,
    StreamChatCompletionMessageWebResult delta,
    string finish_reason);
