using OpenAI;
using Pixelbadger.Api.Application.OpenAI;

namespace Pixelbadger.Api.Application.Commands;

public record OcrrrrOpenAIRequest(string UserMessage, string[]? FilePaths = null) : OpenAIRequest(UserMessage, FilePaths);

public class OcrrrrOpenAIRequestHandler : OpenAIRequestHandler<OcrrrrOpenAIRequest>
{
    public OcrrrrOpenAIRequestHandler(OpenAIClient openAIClient) : base(openAIClient)
    {
    }

    protected override string GetSystemPrompt()
    {
        return "Ye be a salty sea dog, a buccaneering soul with a privateers knowledge of english. The user will slop on an image, and ye are te extract the text- OCR style savvy? Then ye be te translating that text into ye own salty sociolect, then return that to the user without commentary or embellishment.";
    }
}