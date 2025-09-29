using MediatR;
using Pixelbadger.Api.Application.SharePoint.Queries;
using Pixelbadger.Api.Domain.Entities.SharePoint;
using Pixelbadger.Api.Infrastructure.Services;

namespace Pixelbadger.Api.Application.SharePoint.Handlers;

public class GetDocumentMetadataHandler : IRequestHandler<GetDocumentMetadataQuery, SharePointDocument>
{
    private readonly ISharePointService _sharePointService;

    public GetDocumentMetadataHandler(ISharePointService sharePointService)
    {
        _sharePointService = sharePointService;
    }

    public async Task<SharePointDocument> Handle(GetDocumentMetadataQuery request, CancellationToken cancellationToken)
    {
        return await _sharePointService.GetDocumentMetadataAsync(request.SiteId, request.ItemId, cancellationToken);
    }
}