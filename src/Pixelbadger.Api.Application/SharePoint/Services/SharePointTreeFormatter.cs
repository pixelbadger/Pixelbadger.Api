using System.Text;
using Pixelbadger.Api.Domain.Entities.SharePoint;
using Pixelbadger.Api.Infrastructure.Services;

namespace Pixelbadger.Api.Application.SharePoint.Services;

public class SharePointTreeFormatter : ISharePointTreeFormatter
{
    private readonly ISharePointService _sharePointService;
    private int _folderCounter = 0;
    private int _fileCounter = 0;

    public SharePointTreeFormatter(ISharePointService sharePointService)
    {
        _sharePointService = sharePointService;
    }

    public async Task<string> FormatTreeAsync(
        string siteId,
        string rootPath,
        string? userAccessToken,
        CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();
        _folderCounter = 0;
        _fileCounter = 0;

        // Get root items
        var rootItems = await _sharePointService.ListDriveItemsAsync(siteId, rootPath, userAccessToken, cancellationToken);
        var itemsList = rootItems.ToList();

        // Format root
        sb.AppendLine($"/ [d:root] {itemsList.Count} items");

        // Process each item recursively
        foreach (var item in itemsList.OrderBy(x => !x.IsFolder).ThenBy(x => x.Name))
        {
            await FormatItemAsync(sb, item, 1, siteId, userAccessToken, cancellationToken);
        }

        return sb.ToString();
    }

    private async Task FormatItemAsync(
        StringBuilder sb,
        SharePointDriveItem item,
        int level,
        string siteId,
        string? userAccessToken,
        CancellationToken cancellationToken)
    {
        var indent = new string(' ', level * 2);

        if (item.IsFolder)
        {
            _folderCounter++;
            var folderId = $"folder_{_folderCounter:D3}";

            // Get children to count items
            var childPath = string.IsNullOrEmpty(item.ParentPath)
                ? item.Name
                : $"{item.ParentPath}/{item.Name}";

            var children = await _sharePointService.ListDriveItemsAsync(siteId, childPath, userAccessToken, cancellationToken);
            var childrenList = children.ToList();
            var itemCount = childrenList.Count;
            var modifiedDate = FormatDate(item.LastModifiedDateTime);

            sb.AppendLine($"{indent}{item.Name}/ [d:{folderId}] {itemCount} items {modifiedDate}");

            // Recursively process children
            foreach (var child in childrenList.OrderBy(x => !x.IsFolder).ThenBy(x => x.Name))
            {
                await FormatItemAsync(sb, child, level + 1, siteId, userAccessToken, cancellationToken);
            }
        }
        else
        {
            _fileCounter++;
            var fileId = $"doc_{_fileCounter:D3}";
            var extension = GetFileExtension(item.Name);
            var size = FormatSize(item.Size);
            var modifiedDate = FormatDate(item.LastModifiedDateTime);

            sb.AppendLine($"{indent}{item.Name} [f:{fileId}] {size} {extension} {modifiedDate}");
        }
    }

    private static string FormatSize(long bytes)
    {
        if (bytes == 0) return "0B";

        const long kb = 1024;
        const long mb = kb * 1024;
        const long gb = mb * 1024;

        if (bytes >= gb)
            return $"{(double)bytes / gb:0.0}G".Replace(".0G", "G");
        if (bytes >= mb)
            return $"{(double)bytes / mb:0.0}M".Replace(".0M", "M");
        if (bytes >= kb)
            return $"{(double)bytes / kb:0}K";

        return $"{bytes}B";
    }

    private static string FormatDate(DateTime dateTime)
    {
        return dateTime.ToString("MM-dd HH:mm");
    }

    private static string GetFileExtension(string fileName)
    {
        var lastDotIndex = fileName.LastIndexOf('.');
        if (lastDotIndex == -1 || lastDotIndex == fileName.Length - 1)
            return "file";

        return fileName.Substring(lastDotIndex + 1).ToLower();
    }
}
