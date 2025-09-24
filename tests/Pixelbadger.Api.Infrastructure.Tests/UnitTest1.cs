using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;

namespace Pixelbadger.Api.Infrastructure.Tests;

public class AuthenticationIntegrationTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [Test]
    public async Task HealthEndpoint_ShouldReturnOk_WithoutAuthentication()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("Healthy"));
        Assert.That(content, Does.Contain("timestamp")); // JSON serialization uses camelCase
    }

    [Test]
    public async Task WeatherForecastEndpoint_ShouldReturnUnauthorized_WithoutAuthentication()
    {
        // Act
        var response = await _client.GetAsync("/weather/forecast");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}