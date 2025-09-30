using Moq;
using NUnit.Framework;
using Pixelbadger.Api.Application.SharePoint.Handlers;
using Pixelbadger.Api.Application.SharePoint.Queries;
using Pixelbadger.Api.Application.SharePoint.Services;

namespace Pixelbadger.Api.Application.Tests.SharePoint;

[TestFixture]
public class ListDriveItemsHandlerTests
{
    private Mock<ISharePointTreeFormatter> _mockTreeFormatter = null!;
    private ListDriveItemsHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _mockTreeFormatter = new Mock<ISharePointTreeFormatter>();
        _handler = new ListDriveItemsHandler(_mockTreeFormatter.Object);
    }

    [Test]
    public async Task Handle_RootPath_ReturnsFormattedTree()
    {
        // Arrange
        var siteId = "site-id";
        var expectedTree = "/ [d:root] 2 items\n  Documents/ [d:folder_001] 0 items 09-30 12:00\n  Shared/ [d:folder_002] 0 items 09-30 12:00\n";

        _mockTreeFormatter
            .Setup(f => f.FormatTreeAsync(siteId, "", It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTree);

        var query = new ListDriveItemsQuery(siteId, "");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Does.Contain("/ [d:root]"));
        Assert.That(result, Does.Contain("Documents/"));
        _mockTreeFormatter.Verify(f => f.FormatTreeAsync(siteId, "", It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_SpecificPath_ReturnsFormattedTree()
    {
        // Arrange
        var siteId = "site-id";
        var path = "/Documents/Projects";
        var expectedTree = "/ [d:root] 1 items\n  Project1.docx [f:doc_001] 12K docx 09-30 12:00\n";

        _mockTreeFormatter
            .Setup(f => f.FormatTreeAsync(siteId, path, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTree);

        var query = new ListDriveItemsQuery(siteId, path);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Does.Contain("Project1.docx"));
        Assert.That(result, Does.Contain("[f:doc_001]"));
    }
}