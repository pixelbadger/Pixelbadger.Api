using MediatR;
using Pixelbadger.Api.Domain.Entities.SharePoint;

namespace Pixelbadger.Api.Application.SharePoint.Queries;

public record ListDriveItemsQuery(string SiteId, string ItemPath = "") : IRequest<IEnumerable<SharePointDriveItem>>;