using OpenAI;
using Pixelbadger.Api.Application.Commands;

namespace Pixelbadger.Api.Application.Tests.Commands;

[TestFixture]
public class OcrrrrOpenAIRequestHandlerTests
{
    private OcrrrrOpenAIRequestHandler _handler;
    private OpenAIClient _openAIClient;

    [SetUp]
    public void Setup()
    {
        var apiKey = "test-api-key";
        _openAIClient = new OpenAIClient(apiKey);
        _handler = new OcrrrrOpenAIRequestHandler(_openAIClient);
    }

    [Test]
    public void Constructor_ShouldAcceptOpenAIClient()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => new OcrrrrOpenAIRequestHandler(_openAIClient));
    }

    [Test]
    public void GetSystemPrompt_ShouldReturnPirateOCRSystemPrompt()
    {
        // Act
        var result = _handler.GetType()
            .GetMethod("GetSystemPrompt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(_handler, null) as string;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Does.Contain("salty sea dog"));
        Assert.That(result, Does.Contain("OCR style"));
        Assert.That(result, Does.Contain("salty sociolect"));
        Assert.That(result, Does.Contain("without commentary or embellishment"));
    }

    [Test]
    public void OcrrrrOpenAIRequest_ShouldAcceptUserMessageAndFilePaths()
    {
        // Arrange
        var userMessage = "Extract text from this image";
        var filePaths = new[] { "/path/to/image.jpg" };

        // Act
        var request = new OcrrrrOpenAIRequest(userMessage, filePaths);

        // Assert
        Assert.That(request.UserMessage, Is.EqualTo(userMessage));
        Assert.That(request.FilePaths, Is.EqualTo(filePaths));
    }

    [Test]
    public void OcrrrrOpenAIRequest_ShouldInheritFromOpenAIRequest()
    {
        // Arrange
        var request = new OcrrrrOpenAIRequest("test message");

        // Act & Assert
        Assert.That(request, Is.InstanceOf<Pixelbadger.Api.Application.OpenAI.OpenAIRequest>());
    }
}