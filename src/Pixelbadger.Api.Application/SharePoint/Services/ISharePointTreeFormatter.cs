namespace Pixelbadger.Api.Application.SharePoint.Services;

public interface ISharePointTreeFormatter
{
    Task<string> FormatTreeAsync(
        string siteId,
        string rootPath,
        string? userAccessToken,
        CancellationToken cancellationToken = default);
}
