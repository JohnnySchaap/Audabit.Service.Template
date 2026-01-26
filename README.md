# Audabit.Service.Template

A comprehensive reference implementation demonstrating enterprise-grade best practices and design patterns for building ASP.NET Core Web APIs using the Audabit Common Libraries.

## Overview

This service showcases a complete, production-ready ASP.NET Core Web API implementation with:

- **Clean Architecture**: Strict separation of concerns across Api, App, and WebApi layers
- **Common Library Integration**: Comprehensive usage of all Audabit.Common.* packages
- **Design Patterns**: Client-Service-Repository pattern, Dependency Injection, Mapper pattern
- **Telemetry & Observability**: Structured logging with custom events for operation tracking
- **Best Practices**: Error handling, validation, security, API versioning, and comprehensive testing
- **CI/CD Ready**: Azure DevOps pipeline with automated build, test, package, and deployment
- **Performance Optimizations**: Sealed classes for JIT optimization where appropriate

## Recent Improvements

**January 2026:**
- ✅ Initial commit

## Architecture

### Clean Architecture Principles

This solution implements **Clean Architecture** with a pragmatic approach optimized for microservices. It balances architectural purity with real-world maintainability.

#### Core Principles Implemented

✅ **Dependency Direction**: Dependencies flow inward (WebApi + Infra → App → Api)  
✅ **Framework Independence**: Domain models are framework-agnostic  
✅ **Testability**: All layers are testable via interfaces  
✅ **Separation of Concerns**: Clear boundaries between layers  
✅ **Business Logic Isolation**: Core logic independent of web frameworks  
✅ **Infrastructure Isolation**: External integrations separated from business logic

#### Architecture Layers

The solution follows a layered architecture with strict separation of concerns:

```
Audabit.Service.Template/
├── Audabit.Service.Template.Api/        # API contracts and DTOs
│   ├── GlobalConfiguration.cs         # Global API configuration
│   ├── StarWars/                      # StarWars feature
│   │   └── v1/
│   │       ├── Models/                # DTOs for API responses
│   │       └── Transport/             # Request/Response wrappers
│   └── Weather/                       # Weather feature
│       └── v1/
│           ├── Models/
│           └── Transport/
│
├── Audabit.Service.Template.App.Models/ # Domain entities (Entities layer)
│   ├── StarWars/                      # StarWars domain models
│   │   └── StarWarsPerson.cs         # Domain model
│   └── Weather/                       # Weather domain models
│       └── WeatherForecast.cs        # Domain model
│
├── Audabit.Service.Template.App/        # Business logic (Use Cases layer)
│   ├── StarWars/                      # StarWars feature
│   │   ├── Clients/                  # Client abstractions
│   │   │   ├── IStarWarsClient.cs    # Client interface
│   │   │   └── Factories/            # Factory abstractions
│   │   │       └── IStarWarsClientFactory.cs
│   │   ├── Services/                 # Business logic services
│   │   │   ├── IStarWarsService.cs
│   │   │   └── StarWarsService.cs
│   │   └── Telemetry/                # Observability events
│   │       ├── StarWarsPersonRetrievingEvent.cs
│   │       └── StarWarsPersonRetrievedEvent.cs
│   └── Weather/                       # Weather feature
│       ├── Services/                 # Business logic services
│       │   ├── IWeatherService.cs
│       │   └── WeatherService.cs
│       ├── Settings/                 # Configuration models
│       │   └── WeatherSettings.cs
│       ├── Validators/               # FluentValidation validators
│       │   └── Settings/
│       │       └── WeatherSettingsValidator.cs
│       └── Telemetry/                # Observability events
│           ├── WeatherForecastGeneratingEvent.cs
│           ├── WeatherForecastGeneratedEvent.cs
│
├── Audabit.Service.Template.Infra/      # Infrastructure layer (External integrations)
│   ├── StarWars/                      # StarWars feature
│   │   ├── Clients/                  # HTTP client implementations
│   │   │   ├── StarWarsClient.cs     # HTTP client v1 implementation
│   │   │   ├── StarWarsClientV2.cs   # HTTP client v2 implementation
│   │   │   ├── Factories/            # Client selection factories
│   │   │   │   └── StarWarsClientFactory.cs
│   │   │   ├── Mappers/              # Client DTO → Domain mappings
│   │   │   │   └── StarWarsClientMapper.cs
│   │   │   └── Models/               # External API DTOs
│   │   │       └── StarWarsPersonClientModel.cs
│   │   ├── Decorators/               # Observability decorators
│   │   │   ├── StarWarsServiceLoggingDecorator.cs  # Service telemetry
│   │   │   └── StarWarsClientLoggingDecorator.cs   # Client error telemetry
│   │   └── Telemetry/                # Infrastructure-specific telemetry events
│   │       ├── StarWarsPersonRetrievingEvent.cs    # Before operation
│   │       ├── StarWarsPersonRetrievedEvent.cs     # After success
│   │       └── StarWarsPersonRetrievalFailedEvent.cs  # After failure
│   └── Weather/                       # Weather feature
│       ├── Decorators/               # Observability decorators
│       │   └── WeatherServiceLoggingDecorator.cs   # Service telemetry
│       └── Telemetry/                # Infrastructure-specific telemetry events
│           ├── WeatherForecastGeneratingEvent.cs   # Before operation
│           └── WeatherForecastGeneratedEvent.cs    # After success
│
├── Audabit.Service.Template.WebApi/     # API presentation layer
│   ├── Program.cs                     # Service configuration
│   ├── Shared/                        # Shared across features
│   │   └── SwaggerExamples/          # Shared swagger examples
│   │       └── ExampleConstants.cs
│   ├── StarWars/                      # StarWars feature
│   │   ├── Controllers/v1/           # API endpoints
│   │   │   ├── StarWarsController.cs
│   │   │   └── SwaggerExamples/
│   │   └── Mappers/                  # Domain → DTO mappings
│   │       └── StarWarsMapper.cs
│   └── Weather/                       # Weather feature
│       ├── Controllers/v1/           # API endpoints
│       │   ├── WeatherController.cs
│       │   └── SwaggerExamples/
│       ├── Mappers/                  # Domain → DTO mappings
│       │   └── WeatherMapper.cs
│       └── Validators/v1/            # FluentValidation validators
│           └── Transport/
│               └── PutWeatherForecastRequestValidator.cs
│
└── Tests/                             # Comprehensive test coverage
    ├── Audabit.Service.Template.App.Tests.Unit/
    │   ├── StarWars/                  # StarWars feature tests
    │   │   ├── Clients/              # Client layer tests
    │   │   │   ├── Factories/
    │   │   │   │   └── StarWarsClientFactoryTests.cs
    │   │   │   ├── Mappers/
    │   │   │   │   └── StarWarsClientMapperTests.cs
    │   │   │   ├── StarWarsClientTests.cs
    │   │   │   └── StarWarsClientV2Tests.cs
    │   │   └── Services/             # Service layer tests
    │   │       └── StarWarsServiceTests.cs
    │   └── Weather/                   # Weather feature tests
    │       ├── Services/             # Service layer tests
    │       │   └── WeatherServiceTests.cs
    │       └── Validators/Settings/  # Settings validator tests
    │           └── WeatherSettingsValidatorTests.cs
    ├── Audabit.Service.Template.WebApi.Tests.Unit/
    │   ├── StarWars/                  # StarWars feature tests
    │   │   ├── Controllers/v1/       # Controller tests
    │   │   │   └── StarWarsControllerTests.cs
    │   │   └── Mappers/              # Mapper tests
    │   │       └── StarWarsMapperTests.cs
    │   └── Weather/                   # Weather feature tests
    │       ├── Controllers/v1/       # Controller tests
    │       │   └── WeatherControllerTests.cs
    │       ├── Mappers/              # Mapper tests
    │       │   └── WeatherMapperTests.cs
    │       └── Validators/v1/        # Validator tests
    │           └── Transport/
    │               └── PutWeatherForecastRequestValidatorTests.cs
    └── Audabit.Service.Template.Tests.Integration/
        ├── StarWars/                  # StarWars feature integration tests
        │   └── Controllers/v1/
        │       └── StarWarsControllerIntegrationTests.cs
        └── Weather/                   # Weather feature integration tests
            └── Controllers/v1/
                └── WeatherControllerIntegrationTests.cs
```

#### Layer Descriptions

**1. Api Project (Contracts Layer)**
- **Purpose**: API contracts shared with consumers
- **Contains**: Request/Response DTOs, API models
- **Dependencies**: None (pure POCOs)
- **Characteristics**: Framework-agnostic, shareable via NuGet

**2. App.Models Project (Entities/Domain Layer)**
- **Purpose**: Core domain entities and business objects
- **Contains**: Domain models (`StarWarsPerson`, `WeatherForecast`)
- **Dependencies**: None (pure POCOs)
- **Characteristics**: Framework-free, reusable across different applications

**3. App Project (Use Cases/Application Layer)**
- **Purpose**: Business logic and application orchestration
- **Contains**: Service interfaces and implementations, client abstractions, settings, validators
- **Dependencies**: App.Models, minimal framework dependencies (options, FluentValidation)
- **Characteristics**: Testable, contains business rules and use cases, depends on abstractions, **free of observability concerns** (handled by decorators)

**4. Infra Project (Infrastructure Layer)**
- **Purpose**: External system integrations and technical implementations
- **Contains**: HTTP client implementations, factories, mappers (external DTO → domain), external API models, **observability decorators**, telemetry events
- **Dependencies**: App, App.Models, HTTP client libraries, Riok.Mapperly, Audabit.Common.Observability
- **Characteristics**: Implements interfaces from App layer, handles external communication, **implements cross-cutting concerns via decorators**

**5. WebApi Project (Presentation Layer)**
- **Purpose**: HTTP endpoints and web infrastructure concerns
- **Contains**: Controllers, middleware configuration, DI setup, request validators, mappers (domain → DTO)
- **Dependencies**: App, App.Models, Api, Infra, ASP.NET Core, all Common libraries
- **Characteristics**: Thin orchestration layer, delegates to App and Infra layers

#### Pragmatic Clean Architecture Decisions

This implementation makes deliberate trade-offs for microservice efficiency:

**✅ What We Follow (Core Clean Architecture):**
- Dependencies point inward (WebApi → App, never reversed)
- Domain models are pure C# (no framework dependencies)
- Business logic depends on abstractions (`IStarWarsClient`, not implementations)
- Each layer has clear, single responsibility
- Highly testable via dependency injection and interfaces

**⚠️ Pragmatic Deviations from "Pure" Clean Architecture:**

1. **Infrastructure Layer Consolidation**
   - **Classic approach**: Separate Infrastructure projects for persistence, external APIs, messaging, etc.
   - **Our approach**: Single Infra project for all external integrations
   - **Rationale**: 
     - Microservices have focused responsibilities - single Infra project is sufficient
     - Reduces project proliferation and solution complexity
     - Still properly abstracted via interfaces defined in App layer
   - **Impact**: Simpler solution structure while maintaining proper separation

2. **Observability via Decorator Pattern**
   - **Classic approach**: Domain shouldn't know about logging
   - **Our approach**: Telemetry handled by decorators in Infra layer
   - **Rationale**: 
     - Decorator pattern separates cross-cutting concerns from business logic
     - App layer remains completely free of observability dependencies
     - Decorators wrap service/client interfaces to add telemetry transparently
     - Registered via Scrutor in DI container
   - **Impact**: **Zero** - business logic is pure, observability is infrastructure concern

3. **Domain Models Separated from Use Cases**
   - **Classic approach**: Separate "Entities" and "Use Cases" projects
   - **Our approach**: Separate App.Models (Entities) and App (Use Cases) projects
   - **Rationale**: Provides flexibility to reuse domain models while keeping Use Cases in App layer
   - **Impact**: Better separation of concerns, domain models can be shared independently

**When to Use Pure Clean Architecture:**
- Large monolithic systems with complex domains
- Multiple platform implementations (same domain, different UIs)
- Enterprise systems requiring maximum flexibility
- Teams needing strict architectural boundaries

**When Our Pragmatic Approach Works Best:**
- Microservices with focused responsibilities ✅ **(This project)**
- Teams prioritizing maintainability over architectural purity
- Projects where time-to-market is critical
- .NET-only solutions without cross-platform requirements

#### Comparison: Pure vs Pragmatic Clean Architecture

| Aspect | Pure Clean Architecture | Our Pragmatic Approach |
|--------|------------------------|------------------------|
| **Layers** | 4-5 (Entities, Use Cases, Interface Adapters, Frameworks) | 5 (Api, App.Models, App, Infra, WebApi) |
| **Clients Location** | Separate Infrastructure projects per concern | Single Infra project for all external integrations |
| **Domain Purity** | 100% framework-free | 100% framework-free (App.Models + App abstractions) |
| **Complexity** | Higher (more projects/interfaces) | Moderate (proper layers, pragmatic consolidation) |
| **Testability** | Excellent | Excellent |
| **Maintainability** | Good for large teams | Excellent for small teams |
| **Flexibility** | Maximum (swap any layer) | High (swap via interfaces) |
| **Learning Curve** | Steeper | Gentler |
| **Best For** | Enterprise monoliths | Microservices |

#### Dependency Flow Diagram

```
┌─────────────────────────────────────────────┐
│         WebApi (Presentation)               │
│  Controllers, DI Setup, Middleware Config   │
│           Depends on ↓                      │
└──────────────────┬──────────────────────────┘
                   │
       ┌───────────┴───────────┐
       │                       │
┌──────▼────────────────┐  ┌───▼──────────────────────┐
│  Infra (Infrastructure)│  │ App (Use Cases)          │
│  HTTP Clients,        │  │ Services, Interfaces,    │
│  Factories, Mappers   │  │ Settings, Telemetry      │
│  Depends on ↓         │  │ Depends on ↓             │
└──────┬────────────────┘  └───┬──────────────────────┘
       │                       │
       └───────────┬───────────┘
                   │
       ┌───────────┴───────────┐
       │                       │
┌──────▼────────┐      ┌───────▼──────────────┐
│ App.Models    │      │  Api (Contracts)     │
│ (Entities)    │      │  Request/Response    │
│ Domain Models │      │  DTOs (Pure POCOs)   │
│ No deps       │      │  No dependencies     │
└───────────────┘      └──────────────────────┘
```

**Key Insight**: The WebApi layer depends on Infra, App, App.Models, and Api. The Infra layer implements interfaces from App and depends on App.Models for domain entities. The App layer defines abstractions and depends only on App.Models. Both App.Models and Api have no dependencies, making them the most stable and reusable components.

## Key Features

### 1. StarWars API - Best Practice Reference Implementation

The StarWars API demonstrates the **gold standard** implementation pattern:

**Multi-Layer Architecture:**
- ✅ **Client Layer**: Multiple HTTP client versions (v1, v2) with proper error handling (404 vs exceptions)
- ✅ **Client Factory**: `StarWarsClientFactory` selects appropriate client version based on business rules
- ✅ **Client Models**: DTOs for external API responses  
- ✅ **Client Mapper**: Maps external DTOs to domain models
- ✅ **Domain Models**: Business entities independent of external APIs
- ✅ **Service Layer**: Business logic with telemetry integration, uses factory for client selection
- ✅ **API DTOs**: Response models for API consumers
- ✅ **API Mapper**: Maps domain models to API DTOs
- ✅ **Controller**: Thin HTTP layer handling requests/responses

**Factory Pattern:**
- Demonstrates version selection based on business rules (ID-based routing in this example)
- Factory lives in App layer with business logic, not WebApi infrastructure layer
- Provides flexibility for A/B testing, canary deployments, or gradual rollouts

**Telemetry Events** (emitted by decorators, not services):
- `StarWarsPersonRetrievingEvent` - Before API call (decorator)
- `StarWarsPersonRetrievedEvent` - Successful retrieval (decorator)
- `StarWarsPersonRetrievalFailedEvent` - Failed retrieval (decorator)

**Comprehensive Testing:**
- Unit tests for all layers (Client, Mapper, Service, Controller)
- Mocked HTTP responses for client testing
- 100% code coverage for critical paths

### 2. Weather Forecast API

Demonstrates simpler implementation without external HTTP clients:
- Service generates mock data
- Validates request patterns
- Shows telemetry integration

### 3. Integrated Common Libraries

This service demonstrates usage of all Audabit.Common.* libraries:

- **Audabit.Common.ApiVersioning.AspNet**: API versioning (v1, v2, etc.)
- **Audabit.Common.CorrelationId.AspNet**: Request correlation tracking
- **Audabit.Common.ExceptionHandling.AspNet**: Global exception handling
- **Audabit.Common.HttpClient.AspNet**: HTTP client with Polly resilience
- **Audabit.Common.Observability**: Structured logging and custom events
- **Audabit.Common.Observability.AspNet**: ASP.NET Core observability
- **Audabit.Common.Security.AspNet**: API key authentication
- **Audabit.Common.Serialization**: JSON serialization with sensitive data masking
- **Audabit.Common.ServiceInfo.AspNet**: Service information endpoint (displays version)
- **Audabit.Common.Swagger.AspNet**: Swagger/OpenAPI documentation with XML comments
- **Audabit.Common.Validation.AspNet**: FluentValidation integration

### 4. Build Pipeline & Versioning

**Azure DevOps Pipeline** (`azure-pipelines.yml`):
- ✅ Automated build, test, and packaging
- ✅ Code formatting verification (`dotnet format`)
- ✅ Unit tests with code coverage
- ✅ NuGet package creation for Api project
- ✅ WebApi published as deployment artifact
- ✅ Version format: `yyyy.MM.dd.counter` (e.g., 2026.01.20.1)
- ✅ GitHub repository mirroring

**Version Display:**
- Runtime version shown via HomeController (`/` endpoint)
- Assembly version, file version, and informational version all synchronized
- Human-readable date-based versioning

### 5. Testing Strategy

**Comprehensive Test Coverage:**

**Unit Testing:**
- ✅ xUnit framework
- ✅ NSubstitute for mocking
- ✅ AutoFixture for test data generation
- ✅ Shouldly for assertions
- ✅ Nested test class organization with base classes
- ✅ Pattern: `TestClassBase` establishes shared fixtures and mocks

**Integration Testing:**
- ✅ WebApplicationFactory for in-memory HTTP testing
- ✅ WebApplicationFactoryCollection for shared factory instance (prevents ServiceName conflicts)
- ✅ Real HTTP requests without external hosting
- ✅ AutoFixture for dynamic test data with customizations for FluentValidation
- ✅ Tests against actual controllers and middleware
- ✅ Validates complete request/response pipeline
- ✅ No mocking - tests real integrations
- ✅ Nested base class pattern with [Collection] attributes on test classes

**Test Organization:**
```
Tests/
├── Audabit.Service.Template.App.Tests.Unit/          # Business logic tests
│   ├── StarWars/                                   # StarWars feature tests
│   │   └── Services/                              # Service layer tests
│   │       └── StarWarsServiceTests.cs
│   └── Weather/                                    # Weather feature tests
│       ├── Services/                              # Service layer tests
│       │   └── WeatherServiceTests.cs
│       └── Validators/Settings/                   # Settings validator tests
│           └── WeatherSettingsValidatorTests.cs
├── Audabit.Service.Template.Infra.Tests.Unit/        # Infrastructure tests
│   ├── StarWars/                                   # StarWars feature tests
│   │   ├── Clients/                               # HTTP client tests
│   │   │   ├── Factories/
│   │   │   │   └── StarWarsClientFactoryTests.cs
│   │   │   ├── Mappers/
│   │   │   │   └── StarWarsClientMapperTests.cs
│   │   │   ├── StarWarsClientTests.cs
│   │   │   └── StarWarsClientV2Tests.cs
│   │   └── Decorators/                            # Decorator tests
│   │       ├── StarWarsServiceLoggingDecoratorTests.cs
│   │       └── StarWarsClientLoggingDecoratorTests.cs
│   └── Weather/                                    # Weather feature tests
│       └── Decorators/                            # Decorator tests
│           └── WeatherServiceLoggingDecoratorTests.cs
├── Audabit.Service.Template.WebApi.Tests.Unit/       # API layer tests
│   ├── StarWars/                                   # StarWars feature tests
│   │   ├── Controllers/v1/                        # Controller tests
│   │   │   └── StarWarsControllerTests.cs
│   │   └── Mappers/                               # Mapper tests
│   │       └── StarWarsMapperTests.cs
│   └── Weather/                                    # Weather feature tests
│       ├── Controllers/v1/                        # Controller tests
│       │   └── WeatherControllerTests.cs
│       ├── Mappers/                               # Mapper tests
│       │   └── WeatherMapperTests.cs
│       └── Validators/v1/Transport/               # Validator tests
│           └── PutWeatherForecastRequestValidatorTests.cs
└── Audabit.Service.Template.Tests.Integration/       # End-to-end tests
    ├── StarWars/                                   # StarWars feature integration tests
    │   └── Controllers/v1/
    │       └── StarWarsControllerIntegrationTests.cs
    └── Weather/                                    # Weather feature integration tests
        └── Controllers/v1/
            └── WeatherControllerIntegrationTests.cs
```

**Test Directory Structure Pattern:**
- ✅ Test directories **mirror the production code structure** for easy navigation
- ✅ **Feature-first organization** - tests grouped by feature (StarWars/, Weather/)
- ✅ Makes it easy to find related tests when working on a feature
- ✅ Maintains consistency between source and test organization

**Test Pattern with AutoFixture:**
```csharp
public class MyServiceTests
{
    public class MyServiceTestsBase
    {
        protected readonly Fixture _fixture = new();
        protected readonly IMyDependency _dependency;
        protected readonly MyService _service;

        public MyServiceTestsBase()
        {
            _dependency = Substitute.For<IMyDependency>();
            _service = new MyService(_dependency);
        }
    }

    public class MyMethod : MyServiceTestsBase
    {
        [Theory, AutoData]
        public async Task GivenValidInput_ShouldSucceed(string input)
        {
            // AutoFixture generates test data
            var expected = _fixture.Create<MyModel>();
            _dependency.GetAsync(input).Returns(expected);

            var result = await _service.MyMethod(input);

            result.ShouldBe(expected);
        }
    }
}
```

**Integration Test Pattern:**

**Step 1: Create WebApplicationFactoryCollection**
```csharp
[CollectionDefinition(nameof(WebApplicationFactoryCollection))]
public class WebApplicationFactoryCollection : ICollectionFixture<WebApplicationFactory<Program>>
{
}
```

**Step 2: Implement Integration Tests with Nested Base Class**
```csharp
public class MyControllerIntegrationTests
{
    public class MyControllerTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly HttpClient _client;

        public MyControllerTestsBase(WebApplicationFactory<Program> factory)
        {
            _fixture = new Fixture();
            // Add customizations for FluentValidation if needed
            _fixture.Customize<MyRequest>(composer => 
                composer.With(x => x.RequiredProperty, "ValidValue"));
            
            _client = factory.CreateClient();
        }
    }

    [Collection(nameof(WebApplicationFactoryCollection))]
    public class GetEndpoint(WebApplicationFactory<Program> factory) 
        : MyControllerTestsBase(factory)
    {
        [Fact]
        public async Task GivenValidRequest_ShouldReturnOk()
        {
            // Use _fixture.Create<T>() for customized test data
            var request = _fixture.Create<MyRequest>();
            
            var response = await _client.PostAsJsonAsync("/v1/MyController", request);
            
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
        
        [Theory]
        [InlineData("value1")]
        [InlineData("value2")]
        public async Task GivenValidInput_ShouldReturnOk(string input)
        {
            var response = await _client.GetAsync($"/v1/MyController/{input}");
            
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}
```

**Key Points:**
- **WebApplicationFactoryCollection**: Shared factory prevents ServiceName singleton conflicts
- **Nested Base Class**: Establishes shared fixture and HTTP client for all nested test classes
- **[Collection] Attribute**: Required on each nested test class to receive factory from xUnit
- **Primary Constructor**: Nested test classes receive factory via primary constructor and pass to base
- **AutoFixture Customizations**: Configure in base class constructor for FluentValidation compliance
- **Use _fixture.Create<T>()**: Get customized test data instead of [Theory, AutoData] parameters

**Global Usings Pattern:**
- **NO Usings.cs file** - All global usings defined in .csproj
- Consistent across all test projects
- Standard test usings: `AutoFixture`, `AutoFixture.Xunit2`, `NSubstitute`, `Shouldly`, `Xunit`

**Test Coverage:**
- ✅ Infrastructure layer - HTTP clients (HTTP mocking with NSubstitute)
- ✅ Infrastructure layer - Factories (client selection logic)
- ✅ Infrastructure layer - Mappers (external DTO → domain transformation)
- ✅ **Infrastructure layer - Decorators** (telemetry wrapping with emitter verification)
- ✅ Application layer - Services (business logic with mocked dependencies, **no emitter mocking**)
- ✅ Application layer - Validators (FluentValidation rules for settings)
- ✅ Presentation layer - Controllers (HTTP endpoints with mocked services)
- ✅ Presentation layer - Mappers (domain → DTO transformation)
- ✅ Presentation layer - Validators (FluentValidation rules for requests)
- ✅ Integration layer (full HTTP pipeline with WebApplicationFactory)

## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- Visual Studio 2022 or VS Code
- Azure Artifacts access (for Audabit NuGet packages)

### Configuration

**API Keys**: Add to `appsettings.Development.json`:
```json
{
  "ApiKeys": {
    "ValidKeys": ["your-dev-api-key"]
  }
}
```

**Service Configuration**: See `Program.cs` for dependency injection setup.

### Running Locally

1. Clone the repository
2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run --project Audabit.Service.Template.WebApi
   ```

4. Navigate to Swagger UI:
   ```
   https://localhost:5001/swagger
   ```

### Running Tests

```bash
# Run all tests (unit + integration)
dotnet test

# Run unit tests only
dotnet test --filter FullyQualifiedName~Tests.Unit

# Run integration tests only
dotnet test --filter FullyQualifiedName~Tests.Integration

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests for specific controller
dotnet test --filter FullyQualifiedName~StarWarsControllerIntegrationTests
```

**Test Execution:**
- Unit tests: Fast, isolated, mocked dependencies
- Integration tests: Slower, in-memory HTTP server, real pipeline

## Design Patterns Demonstrated

### 1. Decorator Pattern for Observability (Cross-Cutting Concerns)

**When to use**: Separating cross-cutting concerns (logging, telemetry, caching, authorization) from business logic

**Implementation**:
```csharp
// Service interface (App layer - no observability dependencies)
public interface IWeatherService
{
    Task<WeatherForecast[]> GetForecastAsync(string countryCode);
}

// Service implementation (App layer - pure business logic)
public sealed class WeatherService : IWeatherService
{
    public async Task<WeatherForecast[]> GetForecastAsync(string countryCode)
    {
        // Pure business logic - no logging/telemetry
        return GenerateForecasts(countryCode);
    }
}

// Decorator (Infra layer - adds telemetry)
public sealed class WeatherServiceLoggingDecorator(
    IWeatherService inner,
    IEmitter<WeatherServiceLoggingDecorator> emitter) : IWeatherService
{
    public async Task<WeatherForecast[]> GetForecastAsync(string countryCode)
    {
        emitter.RaiseDebug(new WeatherForecastGeneratingEvent(countryCode));
        
        var forecasts = await inner.GetForecastAsync(countryCode);
        
        emitter.RaiseDebug(new WeatherForecastGeneratedEvent(forecasts, countryCode));
        
        return forecasts;
    }
}
```

**Registration** (using Scrutor in `Program.cs`):
```csharp
// Register service first
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Decorate with logging (wraps the service)
builder.Services.Decorate<IWeatherService, WeatherServiceLoggingDecorator>();

// Multiple decorators can be chained:
// builder.Services.Decorate<IWeatherService, CachingDecorator>();
// builder.Services.Decorate<IWeatherService, AuthorizationDecorator>();
```

**Benefits**:
- ✅ **Separation of Concerns**: Business logic stays pure, observability is infrastructure
- ✅ **Single Responsibility**: Each decorator handles one concern
- ✅ **Open/Closed Principle**: Add new behaviors without modifying existing code
- ✅ **Testability**: Test business logic independently from observability
- ✅ **Composability**: Chain multiple decorators for different concerns
- ✅ **Clean Architecture**: App layer has zero dependencies on observability frameworks

**Decorators Implemented**:
- `StarWarsServiceLoggingDecorator` - Wraps `IStarWarsService` with before/after telemetry
- `StarWarsClientLoggingDecorator` - Wraps `IStarWarsClient` with error telemetry
- `WeatherServiceLoggingDecorator` - Wraps `IWeatherService` with before/after telemetry

**Telemetry Event Location**:
- **Before**: Moved from App to Infra (decorator pattern)
- **Events**: `StarWarsPersonRetrievingEvent`, `StarWarsPersonRetrievedEvent`, `StarWarsPersonRetrievalFailedEvent`
- **Events**: `WeatherForecastGeneratingEvent`, `WeatherForecastGeneratedEvent`

---

### 2. Multi-Layer HTTP Client Pattern (StarWars Reference)

**When to use**: External API integration with proper abstraction

```
External API → Client Layer → Domain Models → Service Layer → API Layer → Consumer
              (HTTP Client)   (Mapping)      (Business Logic) (DTO Mapping)
```

**Implementation:**
- `IStarWarsClient` - Client interface
- `StarWarsClient` - HTTP implementation with error handling
- `StarWarsClientMapper` - External DTO → Domain mapping
- `StarWarsService` - Business logic with telemetry
- `StarWarsMapper` - Domain → API DTO mapping
- `StarWarsController` - HTTP endpoint

**Benefits:**
- Domain models independent of external API changes
- Testable at each layer
- Proper separation of concerns
- Comprehensive telemetry

### 3. Simple Service Pattern (Weather Reference)

**When to use**: Internal data generation without external dependencies

```
Service Layer → API Layer → Consumer
(Business Logic) (DTO Mapping)
```

**Implementation:**
- `WeatherService` - Data generation
- `WeatherMapper` - Domain → API DTO mapping
- `WeatherController` - HTTP endpoint

### 4. Mapper Pattern with Riok.Mapperly

**Compile-time mapping** for performance:
```csharp
[Mapper]
public partial class StarWarsMapper
{
    public partial StarWarsPersonDto MapToDto(StarWarsPerson person);
}
```

**Benefits:**
- Zero reflection overhead
- Compile-time type safety
- Automatic property mapping
- Custom mapping methods

### 5. Telemetry Event Pattern (Used in Decorators)

**Structured observability events:**
```csharp
public sealed class StarWarsPersonRetrievingEvent : LoggingEvent
{
    public StarWarsPersonRetrievingEvent(int personId)
        : base(nameof(StarWarsPersonRetrievingEvent))
    {
        Properties.Add(nameof(personId), personId);
    }
}
```

**Usage in decorator** (not in business logic):
```csharp
public sealed class StarWarsServiceLoggingDecorator : IStarWarsService
{
    public async Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken ct)
    {
        emitter.RaiseDebug(new StarWarsPersonRetrievingEvent(id));
        
        var person = await inner.GetPersonAsync(id, ct);  // Delegates to actual service
        
        emitter.RaiseDebug(new StarWarsPersonRetrievedEvent(person, id));
        
        return person;
    }
}
```

**Benefits:**
- Queryable structured logs
- Performance monitoring
- Error tracking
- Audit trails
- **Separated from business logic** via decorator pattern

## API Endpoints

### Home
- `GET /` - Service information and version

### StarWars API (v1)
- `GET /v1/starwars/Person/{id}` - Get Star Wars person by ID
  - **Authentication**: API Key required
  - **Path Parameters**: `id` (integer) - Character ID
  - **Response**: StarWars person details (name, height, mass, etc.)
  - **Telemetry**: Retrieval events logged

### Weather Forecast API (v1)
- `GET /v1/weather/forecast/{countryCode}` - Get weather forecasts for a country
  - **Authentication**: API Key required
  - **Path Parameters**: `countryCode` (2-letter code) - Country code (e.g., "US")
  - **Response**: 5-day weather forecast array
  
- `PUT /v1/weather/forecast/{countryCode}` - Update weather forecast for a country
  - **Authentication**: API Key required
  - **Path Parameters**: `countryCode` (2-letter code) - Country code (e.g., "US")
  - **Validation**: FluentValidation on request body
  - **Response**: Success confirmation

## NuGet Package

The `Audabit.Service.Template.Api` project is packaged as a NuGet package for sharing API contracts with consumers.

**Package Configuration:**
- **PackageId**: `Audabit.Service.Template.Api`
- **Version**: Automatically set by pipeline (yyyy.MM.dd.counter)
- **License**: Apache-2.0
- **Repository**: GitHub

**Publishing:**
- Automatically published to Azure Artifacts on successful pipeline runs
- Includes symbols for debugging
- Source link enabled for F12 navigation

## Deployment

**Azure DevOps Pipeline** builds and publishes:
1. **NuGet Package**: `Audabit.Service.Template.Api` → Azure Artifacts
2. **Web Deployment**: `Audabit.Service.Template.WebApi` → Artifact for deployment

**Version Strategy:**
- Format: `yyyy.MM.dd.counter`
- Example: `2026.01.20.1`
- Increments on each pipeline run
- Displayed at runtime via HomeController

## Contributing

When adding new features, follow the **StarWars pattern** for consistency:

1. Define external client models (if needed)
2. Create client interface and implementation
3. Add client mapper for DTO → Domain
4. Define domain models
5. Create telemetry events
6. Implement service with business logic
7. Add API DTOs
8. Create API mapper for Domain → DTO
9. Implement controller
10. Add comprehensive unit tests at each layer

## Related Documentation

- [Design Patterns Analysis](DESIGN_PATTERNS_ANALYSIS.md) - Detailed pattern documentation
- [Copilot Instructions](.copilot-instructions.md) - AI assistant guidelines
- [Azure Pipeline](azure-pipelines.yml) - CI/CD configuration

### 4. Exception Handling
- Global exception middleware
- Structured error responses
- Logging integration

### 5. Observability
- Structured logging
- Event emission
- Correlation ID tracking
- Request/response logging

### 6. API Documentation
- Swagger/OpenAPI 3.0
- Version-specific documentation
- Example responses
- Security scheme documentation

## Configuration

Configuration is managed through `appsettings.json`:

```json
{
  "ApiSettings": {
    "ApiKeys": ["your-api-key-here"]
  },
  "ServiceSettings": {
    "Name": "Template Service",
    "Version": "1.0.0",
    "Environment": "Development"
  }
}
```

## Development Patterns

### Dependency Injection
All services are registered using the built-in DI container with appropriate lifetimes (Scoped, Singleton, Transient).

### Repository Pattern
Data access is abstracted through repository interfaces, making testing and implementation swapping easier.

### Service Layer
Business logic is encapsulated in service classes, keeping controllers thin and focused on HTTP concerns.

### Unit Testing
Comprehensive unit tests using:
- xUnit
- NSubstitute (mocking)
- AutoFixture (test data generation)
- Shouldly (assertions)

## Contributing

This is a reference implementation. For contributions to the Audabit Common Libraries, please refer to their respective repositories.

## License

Copyright © Audabit Software Solutions B.V. 2026

Licensed under the Apache License, Version 2.0. See [LICENSE](LICENSE) for details.

## Support

For issues or questions:
- Create an issue in the repository
- Contact: [email protected]

## Related Projects

- [Audabit.Common.Validation.AspNet](https://github.com/JohnnySchaap/Audabit.Common.Validation.AspNet)
- [Audabit.Common.ExceptionHandling.AspNet](https://github.com/JohnnySchaap/Audabit.Common.ExceptionHandling.AspNet)
- [Audabit.Common.Security.AspNet](https://github.com/JohnnySchaap/Audabit.Common.Security.AspNet)
- [And all other Audabit.Common.* libraries]
