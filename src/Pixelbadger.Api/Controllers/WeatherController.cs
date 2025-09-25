using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pixelbadger.Api.Application.Queries;

namespace Pixelbadger.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class WeatherController : ControllerBase
{
    private readonly IMediator _mediator;

    public WeatherController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("forecast")]
    public async Task<IActionResult> GetWeatherForecast()
    {
        var forecast = await _mediator.Send(new GetWeatherForecastQuery());
        return Ok(forecast);
    }
}