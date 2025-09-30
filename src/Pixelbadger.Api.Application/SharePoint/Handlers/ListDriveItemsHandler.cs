using MediatR;
using Pixelbadger.Api.Application.SharePoint.Queries;
using Pixelbadger.Api.Application.SharePoint.Services;

namespace Pixelbadger.Api.Application.SharePoint.Handlers;

public class ListDriveItemsHandler : IRequestHandler<ListDriveItemsQuery, string>
{
    private readonly ISharePointTreeFormatter _treeFormatter;

    public ListDriveItemsHandler(ISharePointTreeFormatter treeFormatter)
    {
        _treeFormatter = treeFormatter;
    }

    public async Task<string> Handle(ListDriveItemsQuery request, CancellationToken cancellationToken)
    {
        return await _treeFormatter.FormatTreeAsync(
            request.SiteId,
            request.ItemPath,
            request.UserAccessToken,
            cancellationToken);
    }
}