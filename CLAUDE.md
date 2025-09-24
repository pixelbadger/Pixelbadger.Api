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

## API Controllers

The API uses full controller-based architecture instead of minimal API map registrations. Controllers are located in the `src/Pixelbadger.Api/Controllers/` directory and follow standard ASP.NET Core conventions:

- Controllers inherit from `ControllerBase`
- Use `[ApiController]` and `[Route("[controller]")]` attributes
- Apply `[Authorize]` attribute for protected endpoints
- Return `IActionResult` for proper HTTP response handling

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
