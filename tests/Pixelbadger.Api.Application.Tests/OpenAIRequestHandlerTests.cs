using OpenAI;
using Pixelbadger.Api.Application.OpenAI;

namespace Pixelbadger.Api.Application.Tests.OpenAI;

public record TestOpenAIRequest : OpenAIRequest
{
    public TestOpenAIRequest(string userMessage, string[]? filePaths = null)
        : base(userMessage, filePaths)
    {
    }
}

public class TestOpenAIRequestHandler : OpenAIRequestHandler<TestOpenAIRequest>
{
    private readonly string _systemPrompt;

    public TestOpenAIRequestHandler(OpenAIClient openAIClient, string systemPrompt = "You are a helpful assistant.")
        : base(openAIClient)
    {
        _systemPrompt = systemPrompt;
    }

    protected override string GetSystemPrompt() => _systemPrompt;

    public string GetSystemPromptPublic() => GetSystemPrompt();
}

[TestFixture]
public class OpenAIRequestHandlerTests
{
    [Test]
    public void Constructor_ShouldAcceptOpenAIClient()
    {
        // Arrange
        var apiKey = "test-api-key";
        var openAIClient = new OpenAIClient(apiKey);

        // Act & Assert
        Assert.DoesNotThrow(() => new TestOpenAIRequestHandler(openAIClient));
    }

    [Test]
    public void GetSystemPrompt_ShouldReturnConfiguredPrompt()
    {
        // Arrange
        var apiKey = "test-api-key";
        var openAIClient = new OpenAIClient(apiKey);
        var expectedPrompt = "Custom system prompt for testing";
        var handler = new TestOpenAIRequestHandler(openAIClient, expectedPrompt);

        // Act
        var result = handler.GetSystemPromptPublic();

        // Assert
        Assert.That(result, Is.EqualTo(expectedPrompt));
    }

    [Test]
    public void OpenAIRequest_ShouldAcceptUserMessageAndFilePaths()
    {
        // Arrange
        var userMessage = "Test message";
        var filePaths = new[] { "/path/to/file1.jpg", "/path/to/file2.png" };

        // Act
        var request = new TestOpenAIRequest(userMessage, filePaths);

        // Assert
        Assert.That(request.UserMessage, Is.EqualTo(userMessage));
        Assert.That(request.FilePaths, Is.EqualTo(filePaths));
    }

    [Test]
    public void OpenAIRequest_WithNoFilePaths_ShouldHaveNullFilePaths()
    {
        // Arrange
        var userMessage = "Test message";

        // Act
        var request = new TestOpenAIRequest(userMessage);

        // Assert
        Assert.That(request.UserMessage, Is.EqualTo(userMessage));
        Assert.That(request.FilePaths, Is.Null);
    }
}