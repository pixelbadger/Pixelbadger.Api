This repository implements a generalist toolkit API providing varied explorations in functionality.
The project is written using SOLID principles and Clean Architecture.

## Architecture

The solution follows Clean Architecture principles with these layers:

- **Domain**: Contains business entities and core domain logic
- **Application**: Contains business logic, CQRS handlers using MediatR, and application services
- **Infrastructure**: Handles external concerns like databases, message bus, and cloud storage
- **API**: The web host that exposes endpoints and communicates with the Application layer via MediatR

## Technology Stack

- **Language**: C# with .NET 8
- **Patterns**: CQRS with MediatR, Clean Architecture, SOLID principles
- **Framework**: ASP.NET Core Web API with Controller-based architecture
- **Authentication**: Azure AD/Entra ID with JWT Bearer tokens

## Testing Structure

The solution includes NUnit test projects for the core architectural layers:

- **tests/Pixelbadger.Api.Domain.Tests**: Tests for domain services (domain entities do not require testing)
- **tests/Pixelbadger.Api.Application.Tests**: Tests for application services and CQRS handlers
- **tests/Pixelbadger.Api.Infrastructure.Tests**: Tests for infrastructure services and external integrations

## Testing Requirements

**All CQRS commands and services implemented must have corresponding unit tests.** This includes:

- All MediatR command and query handlers in the Application layer
- All application services and domain services
- All infrastructure services and repository implementations
- Integration tests for external service dependencies

Domain entities (simple data models) do not require unit tests, but domain services with business logic must be tested using the Domain.Tests project.

## CQRS with MediatR Implementation

The Application layer implements the CQRS pattern using MediatR for handling commands and queries:

### Request/Response Pattern

- **Queries**: Implement `IRequest<TResponse>` for data retrieval operations
- **Commands**: Implement `IRequest` or `IRequest<TResponse>` for data modification operations
- **Handlers**: Implement `IRequestHandler<TRequest, TResponse>` to process requests

### MediatR Configuration

MediatR is configured in `Program.cs`:

```csharp
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetWeatherForecastHandler).Assembly));
```

### Example Implementation

Query definition:
```csharp
public record GetWeatherForecastQuery : IRequest<IEnumerable<WeatherForecastDto>>;
```

Handler implementation:
```csharp
public class GetWeatherForecastHandler : IRequestHandler<GetWeatherForecastQuery, IEnumerable<WeatherForecastDto>>
{
    public Task<IEnumerable<WeatherForecastDto>> Handle(GetWeatherForecastQuery request, CancellationToken cancellationToken)
    {
        // Business logic implementation
    }
}
```

## API Controllers

The API uses full controller-based architecture that communicates with the Application layer through MediatR. Controllers are located in the `src/Pixelbadger.Api/Controllers/` directory and follow these conventions:

- Controllers inherit from `ControllerBase`
- Use `[ApiController]` and `[Route("[controller]")]` attributes
- Apply `[Authorize]` attribute for protected endpoints
- Return `IActionResult` for proper HTTP response handling
- Inject `IMediator` to send commands and queries to the Application layer

### Controller Example

```csharp
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
```

## Authentication

The API is secured using Azure AD/Entra ID authentication with JWT Bearer tokens.

### Configuration

Authentication is configured in `appsettings.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "[Enter the tenantId here]",
    "ClientId": "[Enter the clientId here]"
  }
}
```

### Endpoints

- **Public endpoints**: `/health` - No authentication required
- **Protected endpoints**: `/weather/forecast` - Requires valid Azure AD JWT token

### Setup Requirements

To use authentication:

1. Register an application in Azure AD/Entra ID
2. Update `appsettings.json` with your actual TenantId and ClientId
3. Clients must obtain JWT tokens from Azure AD and include them in Authorization headers

## Infrastructure and Deployment

The solution includes Terraform infrastructure as code and GitHub Actions CI/CD pipelines for automated deployment to Azure.

### Terraform Infrastructure

Infrastructure is defined in `infrastructure/terraform/` using Terraform with the Azure provider:

- **Resource Group**: `rg-pixelbadger-{environment}`
- **App Service Plan**: `asp-pixelbadger-{environment}` (F1 free tier for cost optimization)
- **App Service**: `app-pixelbadger-{environment}` (Linux with .NET 8)
- **Remote State**: Stored in Azure Storage for team collaboration

#### Resource Naming Convention

All Azure resources follow standard naming conventions using the workload name "pixelbadger":
- Resource groups: `rg-pixelbadger-{environment}`
- App Service plans: `asp-pixelbadger-{environment}`
- App Services: `app-pixelbadger-{environment}`

#### Environment Support

The infrastructure supports multiple environments (dev, staging, prod) through Terraform variables:
- `workload`: Default "pixelbadger"
- `environment`: Environment name (dev, staging, prod)
- `location`: Azure region (default "East US")

### GitHub Actions Workflows

The repository includes reusable GitHub Actions workflows for deployment automation:

#### Core Workflows

1. **terraform-deploy.yml**: Manual infrastructure deployment
   - Plans infrastructure changes with approval gates
   - Deploys infrastructure to specified environment
   - Uses environment-specific Terraform state files

2. **api-deploy.yml**: Manual API build and deployment
   - Builds .NET 8 API with tests
   - Deploys to Azure App Service
   - Includes health checks and validation

3. **release.yml**: Combined release pipeline
   - Orchestrates infrastructure and API deployment
   - Supports skipping infrastructure if already deployed
   - Provides release summary and status

#### Reusable Components

- **terraform-reusable.yml**: Shared Terraform workflow for plan/apply operations
- **api-reusable.yml**: Shared API build and deployment workflow
- **azure-setup action**: Composite action for Azure authentication and Terraform setup

#### Required GitHub Secrets

For deployment workflows to function, configure these repository secrets:

- `AZURE_CREDENTIALS`: Service principal credentials in JSON format
- `TERRAFORM_STATE_RG`: Resource group containing Terraform state storage
- `TERRAFORM_STATE_SA`: Storage account for Terraform state
- `TERRAFORM_STATE_CONTAINER`: Blob container for state files

#### Azure Service Principal Format

```json
{
  "clientId": "your-client-id",
  "clientSecret": "your-client-secret",
  "subscriptionId": "your-subscription-id",
  "tenantId": "your-tenant-id"
}
```

### Deployment Process

1. **Infrastructure Deployment**: Run terraform-deploy workflow to create Azure resources
2. **API Deployment**: Run api-deploy workflow to build and deploy the application
3. **Complete Release**: Use release workflow for end-to-end deployment

All workflows are manual triggers with environment selection for controlled deployments.

### Health Monitoring

The deployed API includes health check endpoints at `/health` for monitoring application status and validating successful deployments.
