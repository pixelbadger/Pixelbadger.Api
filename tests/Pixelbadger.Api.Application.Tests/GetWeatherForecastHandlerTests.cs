using Pixelbadger.Api.Application.Queries;

namespace Pixelbadger.Api.Application.Tests.Queries;

[TestFixture]
public class GetWeatherForecastHandlerTests
{
    private GetWeatherForecastHandler _handler;

    [SetUp]
    public void Setup()
    {
        _handler = new GetWeatherForecastHandler();
    }

    [Test]
    public async Task Handle_ShouldReturnFiveForecastItems()
    {
        var query = new GetWeatherForecastQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.That(result.Count(), Is.EqualTo(5));
    }

    [Test]
    public async Task Handle_ShouldReturnForecastWithFutureDates()
    {
        var query = new GetWeatherForecastQuery();
        var today = DateOnly.FromDateTime(DateTime.Now);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.That(result.All(f => f.Date > today), Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnForecastWithValidTemperatureRange()
    {
        var query = new GetWeatherForecastQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.That(result.All(f => f.TemperatureC >= -20 && f.TemperatureC < 55), Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnForecastWithValidSummaries()
    {
        var query = new GetWeatherForecastQuery();
        var validSummaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.That(result.All(f => validSummaries.Contains(f.Summary)), Is.True);
    }

    [Test]
    public void WeatherForecastDto_TemperatureF_ShouldConvertCorrectly()
    {
        var forecast = new WeatherForecastDto(DateOnly.FromDateTime(DateTime.Now), 0, "Test");

        Assert.That(forecast.TemperatureF, Is.EqualTo(32));
    }
}