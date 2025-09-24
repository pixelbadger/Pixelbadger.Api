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
- **Framework**: ASP.NET Core Web API
