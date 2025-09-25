using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pixelbadger.Api.Application.Commands;

namespace Pixelbadger.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class OcrrrrController : ControllerBase
{
    private readonly IMediator _mediator;

    public OcrrrrController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("extract")]
    public async Task<IActionResult> ExtractText([FromBody] ExtractTextRequest request)
    {
        var ocrrrrRequest = new OcrrrrOpenAIRequest(request.UserMessage ?? "Extract and translate the text from this image", request.FilePaths);
        var result = await _mediator.Send(ocrrrrRequest);
        return Ok(new { ExtractedText = result });
    }
}

public record ExtractTextRequest(string? UserMessage = null, string[]? FilePaths = null);