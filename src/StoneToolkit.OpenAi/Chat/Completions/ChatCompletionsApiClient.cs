using System.Dynamic;
using System.Net.Http.Json;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;

using Newtonsoft.Json;

using NexusLabs.Contracts;
using StoneToolkit.OpenAI.Http;
using StoneToolkit.OpenAI.OpenAiDtos;
using System.Diagnostics.CodeAnalysis;

namespace StoneToolkit.OpenAI.Chat.Completions;

internal sealed class ChatCompletionsApiClient : IChatCompletionsApiClient
{
    private readonly IHttpClient _httpClient;

    private readonly string _apiUrl = "https://api.openai.com/v1/chat/completions";

    public ChatCompletionsApiClient(
        OpenAiApiConfiguration config, 
        IHttpClient httpClient)
    {
        ArgumentContract.RequiresNotNull(httpClient, nameof(httpClient));
        _httpClient = httpClient;

        if (config != null)
        {
            _apiUrl = new Uri(new Uri(config.ApiEndpoint), config.ApiRouteChatCompletions).ToString();
        }
    }

    public async Task<Completion> CreateAsync(
        CompletionParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentContract.RequiresNotNull(parameters, nameof(parameters));

        var payload = new ExpandoObject();
        payload.TryAdd("model", parameters.Model);

        var messagesPayload = new ExpandoObject[parameters.Messages.Count];
        for (var i = 0; i < parameters.Messages.Count; i++)
        {
            var messagePayload = new ExpandoObject();
            messagePayload.TryAdd("role", parameters.Messages[i].Role);
            messagePayload.TryAdd("content", parameters.Messages[i].Content);
            AddParameterIfSet("name", parameters.Messages[i].Name, messagePayload);

            if (parameters.Messages[i].FunctionCall != null)
            {
                var messageFunctionCallPayload = new ExpandoObject();
                messageFunctionCallPayload.TryAdd(
                    "name", 
                    parameters.Messages[i].FunctionCall!.Name);
                messageFunctionCallPayload.TryAdd(
                    "arguments",
                    parameters.Messages[i].FunctionCall!.Arguments);

                messagePayload.TryAdd("function_call", messageFunctionCallPayload);
            }

            messagesPayload[i] = messagePayload;
        }

        payload.TryAdd("messages", messagesPayload);

        AddParameterIfSet("functions", parameters.Functions, payload);
        AddParameterIfSet("function_call", parameters.FunctionCall, payload);        
        AddParameterIfSet("temperature", parameters.Temperature, payload);
        AddParameterIfSet("top_p", parameters.TopP, payload);
        AddParameterIfSet("n", parameters.N, payload);
        AddParameterIfSet("stream", parameters.Stream, payload);
        AddParameterIfSet("stop", parameters.Stop, payload);
        AddParameterIfSet("max_tokens", parameters.MaxTokens, payload);
        AddParameterIfSet("presence_penalty", parameters.PresencePenalty, payload);
        AddParameterIfSet("frequency_penalty", parameters.FrequencyPenalty, payload);
        AddParameterIfSet("logit_bias", parameters.LogitBias, payload);
        AddParameterIfSet("user", parameters.User, payload);

        var jsonPayload = JsonConvert.SerializeObject(payload);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_apiUrl),
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };

        var responseString = await _httpClient
            .SendRequestAndReceiveStringAsync(
                request,
                cancellationToken)
            .ConfigureAwait(false);

        var webResult = DeserializeResponse<ChatCompletionWebResult>(responseString);
        var result = new Completion(
            webResult.id,
            new DateTime(1970, 1, 1).AddSeconds(webResult.created),
            webResult.model,
            webResult.choices == null
                ? Array.Empty<CompletionChoice>()
                : webResult
                    .choices
                    .Select(x => new CompletionChoice(
                        x.index,
                        new(
                            x.message.role,
                            x.message.content,
                            x.message.function_call == null
                                ? null
                                : new(
                                    x.message.function_call.name,
                                    x.message.function_call.arguments)),
                        x.finish_reason))
                    .ToArray(),
            new CompletionUsage(
                webResult.usage.prompt_tokens,
                webResult.usage.completion_tokens,
                webResult.usage.total_tokens));
        return result;

        static void AddParameterIfSet<T>(string name, T arg, ExpandoObject payload)
        {
            if (arg != null)
            {
                payload.TryAdd(name, arg);
            }
        }
    }

    public async IAsyncEnumerable<(string Result, bool Stop)> CreateStreamAsync(
        CompletionParameters parameters,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentContract.RequiresNotNull(parameters, nameof(parameters));

        var payload = new ExpandoObject();
        payload.TryAdd("model", parameters.Model);

        var messagesPayload = new ExpandoObject[parameters.Messages.Count];
        for (var i = 0; i < parameters.Messages.Count; i++)
        {
            var messagePayload = new ExpandoObject();
            messagePayload.TryAdd("role", parameters.Messages[i].Role);
            messagePayload.TryAdd("content", parameters.Messages[i].Content);
            AddParameterIfSet("name", parameters.Messages[i].Name, messagePayload);

            if (parameters.Messages[i].FunctionCall != null)
            {
                var messageFunctionCallPayload = new ExpandoObject();
                messageFunctionCallPayload.TryAdd(
                    "name",
                    parameters.Messages[i].FunctionCall!.Name);
                messageFunctionCallPayload.TryAdd(
                    "arguments",
                    parameters.Messages[i].FunctionCall!.Arguments);

                messagePayload.TryAdd("function_call", messageFunctionCallPayload);
            }

            messagesPayload[i] = messagePayload;
        }

        payload.TryAdd("messages", messagesPayload);

        AddParameterIfSet("functions", parameters.Functions, payload);
        AddParameterIfSet("function_call", parameters.FunctionCall, payload);
        AddParameterIfSet("temperature", parameters.Temperature, payload);
        AddParameterIfSet("top_p", parameters.TopP, payload);
        AddParameterIfSet("n", parameters.N, payload);
        AddParameterIfSet("stream", true, payload);
        AddParameterIfSet("stop", parameters.Stop, payload);
        AddParameterIfSet("max_tokens", parameters.MaxTokens, payload);
        AddParameterIfSet("presence_penalty", parameters.PresencePenalty, payload);
        AddParameterIfSet("frequency_penalty", parameters.FrequencyPenalty, payload);
        AddParameterIfSet("logit_bias", parameters.LogitBias, payload);
        AddParameterIfSet("user", parameters.User, payload);

        var jsonPayload = JsonConvert.SerializeObject(payload);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_apiUrl),
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Connection", "keep-alive");

        using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);

            yield return (errorMessage, true);
            yield break;

            //response.EnsureSuccessStatusCode();
        }

        await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using StreamReader streamReader = new(stream);

        while (!streamReader.EndOfStream)
        {
            StringBuilder result = new();

            string? line = await streamReader.ReadLineAsync(cancellationToken);

            if (line == null)
            {
                yield return (result.ToString(), true);
                yield break;
            }

            if (string.IsNullOrEmpty(line) || !line.StartsWith("data: "))
            {
                continue;
            }

            string json = line["data: ".Length..];

            if (json == "[DONE]")
            {
                yield return (result.ToString(), true);
                yield break;
            }

            StreamChatCompletionWebResult webResult = DeserializeResponse<StreamChatCompletionWebResult>(json);

            if (webResult?.choices is not { Length: > 0 })
            {
                yield return (result.ToString(), true);
                yield break;
            }

            StreamChatCompletionChoiceWebResult choice = webResult.choices[0];
            if (choice.delta?.content != null)
            {
                result.Append(choice.delta.content);
            }

            if (choice.finish_reason == "stop")
            {
                yield return (result.ToString(), true);
                yield break;
            }

            yield return (result.ToString(), false);
        }

        static void AddParameterIfSet<T>(string name, T arg, ExpandoObject payload)
        {
            if (arg != null)
            {
                payload.TryAdd(name, arg);
            }
        }
    }

    private static T DeserializeResponse<T>(string responseString)
    {
        var webResult = JsonConvert.DeserializeObject<T>(responseString);
        Contract.RequiresNotNull(
            webResult,
            () => $"JSON serializer returned null value for type {typeof(T)}.");
#pragma warning disable CS8603 // Possible null reference return.
        return webResult;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
