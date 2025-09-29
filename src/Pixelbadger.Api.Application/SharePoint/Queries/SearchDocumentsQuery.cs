using MediatR;
using Pixelbadger.Api.Domain.Entities.SharePoint;

namespace Pixelbadger.Api.Application.SharePoint.Queries;

public record SearchDocumentsQuery(string SiteId, string SearchQuery) : IRequest<IEnumerable<SharePointDriveItem>>;