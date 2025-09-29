using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Pixelbadger.Api.Domain.Entities.SharePoint;

namespace Pixelbadger.Api.Infrastructure.Services;

public class SharePointService : ISharePointService
{
    private readonly GraphServiceClient _graphClient;

    public SharePointService(string tenantId, string clientId, string clientSecret)
    {
        var clientSecretCredential = new ClientSecretCredential(
            tenantId,
            clientId,
            clientSecret
        );

        _graphClient = new GraphServiceClient(clientSecretCredential);
    }

    public async Task<SharePointSite> GetSiteAsync(string siteId, CancellationToken cancellationToken = default)
    {
        var site = await _graphClient.Sites[siteId]
            .GetAsync(cancellationToken: cancellationToken);

        if (site == null)
        {
            throw new Exception($"Site with ID {siteId} not found");
        }

        return new SharePointSite
        {
            Id = site.Id ?? string.Empty,
            Name = site.Name ?? string.Empty,
            DisplayName = site.DisplayName ?? string.Empty,
            WebUrl = site.WebUrl ?? string.Empty,
            Description = site.Description ?? string.Empty,
            CreatedDateTime = site.CreatedDateTime?.DateTime ?? DateTime.MinValue,
            LastModifiedDateTime = site.LastModifiedDateTime?.DateTime ?? DateTime.MinValue
        };
    }

    public async Task<IEnumerable<SharePointDriveItem>> ListDriveItemsAsync(
        string siteId,
        string itemPath = "",
        CancellationToken cancellationToken = default)
    {
        var drives = await _graphClient.Sites[siteId].Drives
            .GetAsync(cancellationToken: cancellationToken);

        if (drives?.Value == null || !drives.Value.Any())
        {
            return Enumerable.Empty<SharePointDriveItem>();
        }

        var driveId = drives.Value.First().Id;

        DriveItemCollectionResponse? items;

        if (string.IsNullOrEmpty(itemPath))
        {
            items = await _graphClient.Drives[driveId].Items["root"].Children
                .GetAsync(cancellationToken: cancellationToken);
        }
        else
        {
            items = await _graphClient.Drives[driveId].Items["root"]
                .ItemWithPath(itemPath).Children
                .GetAsync(cancellationToken: cancellationToken);
        }

        if (items?.Value == null)
        {
            return Enumerable.Empty<SharePointDriveItem>();
        }

        return items.Value.Select(item => new SharePointDriveItem
        {
            Id = item.Id ?? string.Empty,
            Name = item.Name ?? string.Empty,
            WebUrl = item.WebUrl ?? string.Empty,
            ParentPath = itemPath,
            IsFolder = item.Folder != null,
            Size = item.Size ?? 0,
            CreatedDateTime = item.CreatedDateTime?.DateTime ?? DateTime.MinValue,
            LastModifiedDateTime = item.LastModifiedDateTime?.DateTime ?? DateTime.MinValue,
            CreatedBy = item.CreatedBy?.User?.DisplayName ?? string.Empty,
            LastModifiedBy = item.LastModifiedBy?.User?.DisplayName ?? string.Empty
        });
    }

    public async Task<SharePointDocument> GetDocumentMetadataAsync(
        string siteId,
        string itemId,
        CancellationToken cancellationToken = default)
    {
        var drives = await _graphClient.Sites[siteId].Drives
            .GetAsync(cancellationToken: cancellationToken);

        if (drives?.Value == null || !drives.Value.Any())
        {
            throw new Exception($"No drives found for site {siteId}");
        }

        var driveId = drives.Value.First().Id;

        var item = await _graphClient.Drives[driveId].Items[itemId]
            .GetAsync(cancellationToken: cancellationToken);

        if (item == null)
        {
            throw new Exception($"Item with ID {itemId} not found");
        }

        var metadata = new Dictionary<string, object>();

        if (item.AdditionalData != null)
        {
            foreach (var kvp in item.AdditionalData)
            {
                metadata[kvp.Key] = kvp.Value ?? string.Empty;
            }
        }

        return new SharePointDocument
        {
            Id = item.Id ?? string.Empty,
            Name = item.Name ?? string.Empty,
            WebUrl = item.WebUrl ?? string.Empty,
            ParentPath = item.ParentReference?.Path ?? string.Empty,
            Size = item.Size ?? 0,
            ContentType = item.File?.MimeType ?? string.Empty,
            CreatedDateTime = item.CreatedDateTime?.DateTime ?? DateTime.MinValue,
            LastModifiedDateTime = item.LastModifiedDateTime?.DateTime ?? DateTime.MinValue,
            CreatedBy = item.CreatedBy?.User?.DisplayName ?? string.Empty,
            LastModifiedBy = item.LastModifiedBy?.User?.DisplayName ?? string.Empty,
            Metadata = metadata
        };
    }

    public async Task<IEnumerable<SharePointDriveItem>> SearchDocumentsAsync(
        string siteId,
        string searchQuery,
        CancellationToken cancellationToken = default)
    {
        var drives = await _graphClient.Sites[siteId].Drives
            .GetAsync(cancellationToken: cancellationToken);

        if (drives?.Value == null || !drives.Value.Any())
        {
            return Enumerable.Empty<SharePointDriveItem>();
        }

        var driveId = drives.Value.First().Id;

        var searchResults = await _graphClient.Drives[driveId]
            .SearchWithQ(searchQuery)
            .GetAsSearchWithQGetResponseAsync(cancellationToken: cancellationToken);

        if (searchResults?.Value == null)
        {
            return Enumerable.Empty<SharePointDriveItem>();
        }

        return searchResults.Value.Select(item => new SharePointDriveItem
        {
            Id = item.Id ?? string.Empty,
            Name = item.Name ?? string.Empty,
            WebUrl = item.WebUrl ?? string.Empty,
            ParentPath = item.ParentReference?.Path ?? string.Empty,
            IsFolder = item.Folder != null,
            Size = item.Size ?? 0,
            CreatedDateTime = item.CreatedDateTime?.DateTime ?? DateTime.MinValue,
            LastModifiedDateTime = item.LastModifiedDateTime?.DateTime ?? DateTime.MinValue,
            CreatedBy = item.CreatedBy?.User?.DisplayName ?? string.Empty,
            LastModifiedBy = item.LastModifiedBy?.User?.DisplayName ?? string.Empty
        });
    }

    public async Task<byte[]> GetDocumentContentAsync(
        string siteId,
        string itemId,
        CancellationToken cancellationToken = default)
    {
        var drives = await _graphClient.Sites[siteId].Drives
            .GetAsync(cancellationToken: cancellationToken);

        if (drives?.Value == null || !drives.Value.Any())
        {
            throw new Exception($"No drives found for site {siteId}");
        }

        var driveId = drives.Value.First().Id;

        var stream = await _graphClient.Drives[driveId].Items[itemId].Content
            .GetAsync(cancellationToken: cancellationToken);

        if (stream == null)
        {
            throw new Exception($"Could not retrieve content for item {itemId}");
        }

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }
}