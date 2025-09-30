using MediatR;
using Pixelbadger.Api.Application.SharePoint.Queries;
using Pixelbadger.Api.Domain.Entities.SharePoint;
using Pixelbadger.Api.Infrastructure.Services;

namespace Pixelbadger.Api.Application.SharePoint.Handlers;

public class SearchDocumentsHandler : IRequestHandler<SearchDocumentsQuery, IEnumerable<SharePointDriveItem>>
{
    private readonly ISharePointService _sharePointService;

    public SearchDocumentsHandler(ISharePointService sharePointService)
    {
        _sharePointService = sharePointService;
    }

    public async Task<IEnumerable<SharePointDriveItem>> Handle(SearchDocumentsQuery request, CancellationToken cancellationToken)
    {
        return await _sharePointService.SearchDocumentsAsync(request.SiteId, request.SearchQuery, request.UserAccessToken, cancellationToken);
    }
}