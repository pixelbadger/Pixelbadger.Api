using MediatR;
using Pixelbadger.Api.Domain.Entities.SharePoint;

namespace Pixelbadger.Api.Application.SharePoint.Queries;

public record GetDocumentMetadataQuery(string SiteId, string ItemId) : IRequest<SharePointDocument>;