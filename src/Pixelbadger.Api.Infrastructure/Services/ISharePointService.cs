using Pixelbadger.Api.Domain.Entities.SharePoint;

namespace Pixelbadger.Api.Infrastructure.Services;

public interface ISharePointService
{
    Task<SharePointSite> GetSiteAsync(string siteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SharePointDriveItem>> ListDriveItemsAsync(string siteId, string itemPath = "", CancellationToken cancellationToken = default);
    Task<SharePointDocument> GetDocumentMetadataAsync(string siteId, string itemId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SharePointDriveItem>> SearchDocumentsAsync(string siteId, string searchQuery, CancellationToken cancellationToken = default);
    Task<byte[]> GetDocumentContentAsync(string siteId, string itemId, CancellationToken cancellationToken = default);
}