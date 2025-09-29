using MediatR;
using Pixelbadger.Api.Application.SharePoint.Queries;
using Pixelbadger.Api.Domain.Entities.SharePoint;
using Pixelbadger.Api.Infrastructure.Services;

namespace Pixelbadger.Api.Application.SharePoint.Handlers;

public class GetSiteInfoHandler : IRequestHandler<GetSiteInfoQuery, SharePointSite>
{
    private readonly ISharePointService _sharePointService;

    public GetSiteInfoHandler(ISharePointService sharePointService)
    {
        _sharePointService = sharePointService;
    }

    public async Task<SharePointSite> Handle(GetSiteInfoQuery request, CancellationToken cancellationToken)
    {
        return await _sharePointService.GetSiteAsync(request.SiteId, cancellationToken);
    }
}