using MediatR;
using Pixelbadger.Api.Domain.Entities.SharePoint;

namespace Pixelbadger.Api.Application.SharePoint.Queries;

public record ListDriveItemsQuery(string SiteId, string ItemPath = "", string? UserAccessToken = null) : IRequest<IEnumerable<SharePointDriveItem>>;