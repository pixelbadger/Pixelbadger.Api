using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

namespace Pixelbadger.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var openAiApiKey = configuration["OpenAI:ApiKey"];

        if (string.IsNullOrEmpty(openAiApiKey))
        {
            throw new InvalidOperationException("OpenAI:ApiKey configuration is required but was not found.");
        }

        services.AddSingleton<OpenAIClient>(provider => new OpenAIClient(openAiApiKey));

        return services;
    }
}
