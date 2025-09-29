using MediatR;
using Pixelbadger.Api.Application.SharePoint.Queries;
using Pixelbadger.Api.Domain.Entities.SharePoint;
using Pixelbadger.Api.Infrastructure.Services;

namespace Pixelbadger.Api.Application.SharePoint.Handlers;

public class ListDriveItemsHandler : IRequestHandler<ListDriveItemsQuery, IEnumerable<SharePointDriveItem>>
{
    private readonly ISharePointService _sharePointService;

    public ListDriveItemsHandler(ISharePointService sharePointService)
    {
        _sharePointService = sharePointService;
    }

    public async Task<IEnumerable<SharePointDriveItem>> Handle(ListDriveItemsQuery request, CancellationToken cancellationToken)
    {
        return await _sharePointService.ListDriveItemsAsync(request.SiteId, request.ItemPath, cancellationToken);
    }
}