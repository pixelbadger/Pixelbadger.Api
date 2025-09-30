using Moq;
using NUnit.Framework;
using Pixelbadger.Api.Application.SharePoint.Handlers;
using Pixelbadger.Api.Application.SharePoint.Queries;
using Pixelbadger.Api.Domain.Entities.SharePoint;
using Pixelbadger.Api.Infrastructure.Services;

namespace Pixelbadger.Api.Application.Tests.SharePoint;

[TestFixture]
public class GetSiteInfoHandlerTests
{
    private Mock<ISharePointService> _mockSharePointService = null!;
    private GetSiteInfoHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _mockSharePointService = new Mock<ISharePointService>();
        _handler = new GetSiteInfoHandler(_mockSharePointService.Object);
    }

    [Test]
    public async Task Handle_ValidSiteId_ReturnsSiteInfo()
    {
        // Arrange
        var siteId = "contoso.sharepoint.com,site-guid,web-guid";
        var expectedSite = new SharePointSite
        {
            Id = siteId,
            Name = "Test Site",
            DisplayName = "Test Site Display",
            WebUrl = "https://contoso.sharepoint.com/sites/test",
            Description = "Test Description",
            CreatedDateTime = DateTime.UtcNow.AddDays(-30),
            LastModifiedDateTime = DateTime.UtcNow
        };

        _mockSharePointService
            .Setup(s => s.GetSiteAsync(siteId, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSite);

        var query = new GetSiteInfoQuery(siteId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(expectedSite.Id));
        Assert.That(result.Name, Is.EqualTo(expectedSite.Name));
        Assert.That(result.WebUrl, Is.EqualTo(expectedSite.WebUrl));
        _mockSharePointService.Verify(s => s.GetSiteAsync(siteId, It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void Handle_InvalidSiteId_ThrowsException()
    {
        // Arrange
        var siteId = "invalid-site-id";
        _mockSharePointService
            .Setup(s => s.GetSiteAsync(siteId, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception($"Site with ID {siteId} not found"));

        var query = new GetSiteInfoQuery(siteId);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _handler.Handle(query, CancellationToken.None));
    }
}