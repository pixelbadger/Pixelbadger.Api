using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pixelbadger.Api.Application.SharePoint.Queries;

namespace Pixelbadger.Api.Controllers;

/// <summary>
/// MCP-compatible SharePoint interrogation tools for LLM agents
/// </summary>
[ApiController]
[Route("mcp/sharepoint")]
[Authorize]
public class SharePointController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SharePointController> _logger;

    public SharePointController(IMediator mediator, ILogger<SharePointController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// MCP Tool: Get SharePoint site information
    /// </summary>
    /// <param name="siteId">SharePoint site ID or hostname:path (e.g., contoso.sharepoint.com:/sites/teamsite)</param>
    [HttpGet("sites/{siteId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSiteInfo(string siteId)
    {
        try
        {
            var query = new GetSiteInfoQuery(siteId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving site info for {SiteId}", siteId);
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// MCP Tool: List items in a SharePoint folder (like 'ls' command)
    /// </summary>
    /// <param name="siteId">SharePoint site ID</param>
    /// <param name="path">Folder path (empty for root, or path like "/Documents/Folder1")</param>
    [HttpGet("sites/{siteId}/items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListItems(string siteId, [FromQuery] string path = "")
    {
        try
        {
            var query = new ListDriveItemsQuery(siteId, path);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing items for site {SiteId} at path {Path}", siteId, path);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// MCP Tool: Get detailed metadata for a specific document
    /// </summary>
    /// <param name="siteId">SharePoint site ID</param>
    /// <param name="itemId">Document item ID</param>
    [HttpGet("sites/{siteId}/items/{itemId}/metadata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDocumentMetadata(string siteId, string itemId)
    {
        try
        {
            var query = new GetDocumentMetadataQuery(siteId, itemId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata for item {ItemId} in site {SiteId}", itemId, siteId);
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// MCP Tool: Search for documents matching a query across the SharePoint site
    /// </summary>
    /// <param name="siteId">SharePoint site ID</param>
    /// <param name="q">Search query (keywords to find in document names or content)</param>
    [HttpGet("sites/{siteId}/search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchDocuments(string siteId, [FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { error = "Search query 'q' is required" });
        }

        try
        {
            var query = new SearchDocumentsQuery(siteId, q);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching documents in site {SiteId} with query {Query}", siteId, q);
            return BadRequest(new { error = ex.Message });
        }
    }
}