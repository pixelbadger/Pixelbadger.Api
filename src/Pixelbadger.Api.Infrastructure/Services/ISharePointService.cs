using Pixelbadger.Api.Domain.Entities.SharePoint;

namespace Pixelbadger.Api.Infrastructure.Services;

public interface ISharePointService
{
    Task<SharePointSite> GetSiteAsync(string siteId, string? userAccessToken = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<SharePointDriveItem>> ListDriveItemsAsync(string siteId, string itemPath = "", string? userAccessToken = null, CancellationToken cancellationToken = default);
    Task<SharePointDocument> GetDocumentMetadataAsync(string siteId, string itemId, string? userAccessToken = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<SharePointDriveItem>> SearchDocumentsAsync(string siteId, string searchQuery, string? userAccessToken = null, CancellationToken cancellationToken = default);
    Task<byte[]> GetDocumentContentAsync(string siteId, string itemId, string? userAccessToken = null, CancellationToken cancellationToken = default);
}