using MediatR;

namespace Pixelbadger.Api.Application.SharePoint.Queries;

public record ListDriveItemsQuery(string SiteId, string ItemPath = "", string? UserAccessToken = null) : IRequest<string>;