using StoneToolkit.OpenAI.Chat.Completions;
using StoneToolkit.OpenAI.Completions;
using StoneToolkit.OpenAI.Embeddings;
using StoneToolkit.OpenAI.Files;
using StoneToolkit.OpenAI.FineTunes;
using StoneToolkit.OpenAI.Models;

namespace StoneToolkit.OpenAI;

public interface IOpenAiApiClient : IDisposable
{
    IFineTunesApiClient FineTunes { get; }

    IFilesApiClient Files { get; }

    IModelsApiClient Models { get; }

    ICompletionsApiClient Completions { get; }

    IChatCompletionsApiClient Chat { get; }

    IEmbeddingsApiClient Embeddings { get; }
}