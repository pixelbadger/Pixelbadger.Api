using Moq;
using NUnit.Framework;
using Pixelbadger.Api.Application.SharePoint.Handlers;
using Pixelbadger.Api.Application.SharePoint.Queries;
using Pixelbadger.Api.Domain.Entities.SharePoint;
using Pixelbadger.Api.Infrastructure.Services;

namespace Pixelbadger.Api.Application.Tests.SharePoint;

[TestFixture]
public class ListDriveItemsHandlerTests
{
    private Mock<ISharePointService> _mockSharePointService = null!;
    private ListDriveItemsHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _mockSharePointService = new Mock<ISharePointService>();
        _handler = new ListDriveItemsHandler(_mockSharePointService.Object);
    }

    [Test]
    public async Task Handle_RootPath_ReturnsRootItems()
    {
        // Arrange
        var siteId = "site-id";
        var expectedItems = new List<SharePointDriveItem>
        {
            new SharePointDriveItem
            {
                Id = "item1",
                Name = "Documents",
                IsFolder = true,
                ParentPath = "",
                Size = 0,
                CreatedDateTime = DateTime.UtcNow,
                LastModifiedDateTime = DateTime.UtcNow
            },
            new SharePointDriveItem
            {
                Id = "item2",
                Name = "Shared",
                IsFolder = true,
                ParentPath = "",
                Size = 0,
                CreatedDateTime = DateTime.UtcNow,
                LastModifiedDateTime = DateTime.UtcNow
            }
        };

        _mockSharePointService
            .Setup(s => s.ListDriveItemsAsync(siteId, "", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedItems);

        var query = new ListDriveItemsQuery(siteId, "");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.First().Name, Is.EqualTo("Documents"));
        _mockSharePointService.Verify(s => s.ListDriveItemsAsync(siteId, "", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_SpecificPath_ReturnsItemsInPath()
    {
        // Arrange
        var siteId = "site-id";
        var path = "/Documents/Projects";
        var expectedItems = new List<SharePointDriveItem>
        {
            new SharePointDriveItem
            {
                Id = "doc1",
                Name = "Project1.docx",
                IsFolder = false,
                ParentPath = path,
                Size = 12345,
                CreatedDateTime = DateTime.UtcNow,
                LastModifiedDateTime = DateTime.UtcNow
            }
        };

        _mockSharePointService
            .Setup(s => s.ListDriveItemsAsync(siteId, path, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedItems);

        var query = new ListDriveItemsQuery(siteId, path);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().IsFolder, Is.False);
        Assert.That(result.First().Size, Is.GreaterThan(0));
    }
}