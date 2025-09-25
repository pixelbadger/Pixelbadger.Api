using MediatR;
using OpenAI;
using OpenAI.Chat;

namespace Pixelbadger.Api.Application.OpenAI;

public record OpenAIRequest(string UserMessage, string[]? FilePaths = null) : IRequest<string>;

public abstract class OpenAIRequestHandler<TRequest> : IRequestHandler<TRequest, string>
    where TRequest : OpenAIRequest
{
    protected readonly OpenAIClient _openAIClient;

    protected OpenAIRequestHandler(OpenAIClient openAIClient)
    {
        _openAIClient = openAIClient;
    }

    protected abstract string GetSystemPrompt();

    public async Task<string> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var chatClient = _openAIClient.GetChatClient("gpt-4o");

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(GetSystemPrompt()),
            new UserChatMessage(request.UserMessage)
        };

        // Add file attachments if provided
        if (request.FilePaths is not null && request.FilePaths.Length > 0)
        {
            var messageContent = new List<ChatMessageContentPart>
            {
                ChatMessageContentPart.CreateTextPart(request.UserMessage)
            };

            foreach (var filePath in request.FilePaths)
            {
                if (File.Exists(filePath))
                {
                    var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
                    var fileName = Path.GetFileName(filePath);
                    var mimeType = GetMimeType(filePath);

                    messageContent.Add(ChatMessageContentPart.CreateImagePart(
                        BinaryData.FromBytes(fileBytes),
                        mimeType));
                }
            }

            // Replace the simple user message with multimodal content
            messages[1] = new UserChatMessage(messageContent);
        }

        var completion = await chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        return completion.Value.Content[0].Text;
    }

    private static string GetMimeType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}