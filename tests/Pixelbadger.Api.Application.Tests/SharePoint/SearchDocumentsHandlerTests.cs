using Moq;
using NUnit.Framework;
using Pixelbadger.Api.Application.SharePoint.Handlers;
using Pixelbadger.Api.Application.SharePoint.Queries;
using Pixelbadger.Api.Domain.Entities.SharePoint;
using Pixelbadger.Api.Infrastructure.Services;

namespace Pixelbadger.Api.Application.Tests.SharePoint;

[TestFixture]
public class SearchDocumentsHandlerTests
{
    private Mock<ISharePointService> _mockSharePointService = null!;
    private SearchDocumentsHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _mockSharePointService = new Mock<ISharePointService>();
        _handler = new SearchDocumentsHandler(_mockSharePointService.Object);
    }

    [Test]
    public async Task Handle_ValidSearchQuery_ReturnsMatchingDocuments()
    {
        // Arrange
        var siteId = "site-id";
        var searchQuery = "budget report";
        var expectedResults = new List<SharePointDriveItem>
        {
            new SharePointDriveItem
            {
                Id = "doc1",
                Name = "Q4 Budget Report.xlsx",
                IsFolder = false,
                ParentPath = "/Documents/Finance",
                Size = 45678,
                CreatedDateTime = DateTime.UtcNow.AddDays(-10),
                LastModifiedDateTime = DateTime.UtcNow.AddDays(-2)
            },
            new SharePointDriveItem
            {
                Id = "doc2",
                Name = "Annual Budget Summary.pdf",
                IsFolder = false,
                ParentPath = "/Documents/Reports",
                Size = 23456,
                CreatedDateTime = DateTime.UtcNow.AddDays(-5),
                LastModifiedDateTime = DateTime.UtcNow.AddDays(-1)
            }
        };

        _mockSharePointService
            .Setup(s => s.SearchDocumentsAsync(siteId, searchQuery, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        var query = new SearchDocumentsQuery(siteId, searchQuery);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(r => r.Name.Contains("Budget", StringComparison.OrdinalIgnoreCase)), Is.True);
        _mockSharePointService.Verify(s => s.SearchDocumentsAsync(siteId, searchQuery, It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_NoResults_ReturnsEmptyCollection()
    {
        // Arrange
        var siteId = "site-id";
        var searchQuery = "nonexistent document";
        var expectedResults = new List<SharePointDriveItem>();

        _mockSharePointService
            .Setup(s => s.SearchDocumentsAsync(siteId, searchQuery, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        var query = new SearchDocumentsQuery(siteId, searchQuery);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}