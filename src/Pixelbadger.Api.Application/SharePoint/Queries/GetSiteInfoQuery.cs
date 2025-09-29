using MediatR;
using Pixelbadger.Api.Domain.Entities.SharePoint;

namespace Pixelbadger.Api.Application.SharePoint.Queries;

public record GetSiteInfoQuery(string SiteId) : IRequest<SharePointSite>;