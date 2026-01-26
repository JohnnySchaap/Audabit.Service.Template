# Audabit.Service.Template - Copilot Instructions

This file provides comprehensive guidance for the Audabit.Service.Template reference implementation.

---

## Project Overview

Audabit.Service.Template demonstrates enterprise-grade best practices for:
- Multi-layer architecture with clean separation of concerns
- External API integration with proper abstraction
- Comprehensive telemetry and observability
- Production-ready error handling and validation
- Automated build, test, and deployment pipelines

---

## Architecture & Design Patterns

### Pattern 1: Multi-Layer HTTP Client Pattern with Decorator (StarWars - REFERENCE IMPLEMENTATION)

**Use Case**: External API integration requiring proper abstraction, testability, and observability

**Layer Flow**:
```
External API → Client Layer → Domain Models → Service Layer → Decorator Layer → API Layer → Consumer
              (HTTP Client)   (Mapping)      (Business Logic) (Observability)  (DTO Mapping)
```

**Implementation Layers**:

1. **Client Models** (`Infra/StarWars/Clients/Models/`) - DTOs matching external API
2. **Client Mapper** (`Infra/StarWars/Clients/Mappers/`) - External DTO → Domain mapping (Riok.Mapperly)
3. **Client Implementation** (`Infra/StarWars/Clients/`) - HTTP client implementations (v1, v2)
4. **Client Factory Implementation** (`Infra/StarWars/Clients/Factories/`) - Factory for selecting client version
5. **Client Decorator** (`Infra/StarWars/Decorators/`) - StarWarsClientLoggingDecorator (error telemetry)
6. **Service Decorator** (`Infra/StarWars/Decorators/`) - StarWarsServiceLoggingDecorator (before/after telemetry)
7. **Infrastructure Telemetry** (`Infra/StarWars/Telemetry/`) - All telemetry events (Retrieving, Retrieved, RetrievalFailed)
8. **Client Interface** (`App/StarWars/Clients/`) - IStarWarsClient abstraction
9. **Factory Interface** (`App/StarWars/Clients/Factories/`) - IStarWarsClientFactory abstraction
10. **Domain Models** (`App.Models/StarWars/`) - Business entities independent of external API
11. **Service Layer** (`App/StarWars/Services/`) - **Pure business logic, NO telemetry/emitter dependencies**
12. **API DTOs** (`Api/StarWars/v1/Models/` and `Transport/`) - Public API contract models
13. **API Mapper** (`WebApi/StarWars/Mappers/`) - Domain → API DTO mapping
14. **Controller** (`WebApi/StarWars/Controllers/v1/`) - HTTP endpoints

**When to Use**: Any feature requiring external HTTP API integration

---

### Pattern 2: Client Factory Pattern (StarWars)

**Use Case**: Multiple client versions requiring dynamic selection based on business rules

**Implementation**:
```csharp
// Interface (App/StarWars/Clients/Factories/IStarWarsClientFactory.cs)
public interface IStarWarsClientFactory
{
    IStarWarsClient GetClient(int id);
}

// Factory Implementation (Infra/StarWars/Clients/Factories/StarWarsClientFactory.cs)
public sealed class StarWarsClientFactory(IEnumerable<IStarWarsClient> starWarsClients) 
    : IStarWarsClientFactory
{
    public IStarWarsClient GetClient(int id)
    {
        // NOTE: This is just an example of business logic to choose the client version.
        // NOTE2: You could also use starWarsClients.OfType<StarWarsClient>().First() & starWarsClients.OfType<StarWarsClientV2>().First()
        var version = (id % 10 == 2) ? "v2" : "v1";
        return starWarsClients.FirstOrDefault(client => client.Version == version)
            ?? throw new InvalidOperationException($"Client version '{version}' not registered.");
    }
}

// Service usage
public sealed class StarWarsService(IStarWarsClientFactory factory) : IStarWarsService
{
    public async Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken cancellationToken)
    {
        var client = factory.GetClient(id);
        return await client.GetPersonAsync(id, cancellationToken);
    }
}
```

**Registration** (`Program.cs`):
```csharp
builder.Services.AddScoped<IStarWarsClient, StarWarsClient>();
builder.Services.AddScoped<IStarWarsClient, StarWarsClientV2>();
builder.Services.AddScoped<IStarWarsClientFactory, StarWarsClientFactory>();
```

**Architectural Note**: 
- Factory interface lives in App layer (`IStarWarsClientFactory`) - defines the abstraction
- Factory implementation lives in Infra layer (`StarWarsClientFactory`) - implements client selection
- This follows proper Clean Architecture - App depends on abstractions, Infra provides implementations
- Business logic (in App) calls the factory interface, infrastructure provides the concrete implementation

**When to Use**:
- A/B testing between client versions
- Canary deployments with gradual rollout
- Feature flags controlling which version to use
- Multiple backend versions requiring different client implementations

---

### Pattern 3: Decorator Pattern for Cross-Cutting Concerns (Observability)

**Use Case**: Separating observability/telemetry from business logic while maintaining clean architecture

**Implementation**:
```csharp
// Step 1: Service with pure business logic (App layer - NO observability dependencies)
public sealed class WeatherService : IWeatherService
{
    public async Task<WeatherForecast[]> GetForecastAsync(string countryCode)
    {
        // Pure business logic - no IEmitter, no telemetry events
        return GenerateForecasts(countryCode);
    }
}

// Step 2: Decorator wraps service with telemetry (Infra layer)
public sealed class WeatherServiceLoggingDecorator(
    IWeatherService inner,  // The actual service
    IEmitter<WeatherServiceLoggingDecorator> emitter) : IWeatherService
{
    public async Task<WeatherForecast[]> GetForecastAsync(string countryCode)
    {
        // Before: Emit telemetry
        emitter.RaiseDebug(new WeatherForecastGeneratingEvent(countryCode));
        
        // Delegate to actual service
        var forecasts = await inner.GetForecastAsync(countryCode);
        
        // After: Emit telemetry
        emitter.RaiseDebug(new WeatherForecastGeneratedEvent(forecasts, countryCode));
        
        return forecasts;
    }
}

// Step 3: Telemetry events (Infra/StarWars/Telemetry/ or Infra/Weather/Telemetry/)
public sealed class WeatherForecastGeneratingEvent : LoggingEvent
{
    public WeatherForecastGeneratingEvent(string countryCode)
        : base(nameof(WeatherForecastGeneratingEvent))
    {
        Properties.Add(nameof(countryCode), countryCode);
    }
}
```

**Registration** (`Program.cs`) using Scrutor:
```csharp
// Register service first
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Decorate with logging (wraps the service transparently)
builder.Services.Decorate<IWeatherService, WeatherServiceLoggingDecorator>();

// Multiple decorators can be chained if needed:
// builder.Services.Decorate<IWeatherService, CachingDecorator>();
// builder.Services.Decorate<IWeatherService, AuthorizationDecorator>();
```

**Decorator Testing** (verify telemetry without business logic):
```csharp
public class WeatherServiceLoggingDecoratorTests
{
    [Theory, AutoData]
    public async Task GivenValidCountryCode_ShouldCallInnerServiceAndRaiseTelemetry(
        string countryCode)
    {
        // Arrange
        var innerService = Substitute.For<IWeatherService>();
        var emitter = Substitute.For<IEmitter<WeatherServiceLoggingDecorator>>();
        var forecasts = new[] { new WeatherForecast { /* ... */ } };
        innerService.GetForecastAsync(countryCode).Returns(forecasts);
        
        var decorator = new WeatherServiceLoggingDecorator(innerService, emitter);
        
        // Act
        var result = await decorator.GetForecastAsync(countryCode);
        
        // Assert
        await innerService.Received(1).GetForecastAsync(countryCode);
        emitter.Received(1).Raise(Arg.Any<IEvent>(), LogLevel.Debug, Arg.Any<Dictionary<string, object>>());
        result.ShouldBe(forecasts);
    }
}
```

**Service Testing** (NO emitter mocking required):
```csharp
public class WeatherServiceTests
{
    [Theory, AutoData]
    public async Task GivenValidCountryCode_ShouldReturnForecasts(string countryCode)
    {
        // Arrange
        var settings = Options.Create(new WeatherSettings());
        var service = new WeatherService(settings);
        
        // Act
        var result = await service.GetForecastAsync(countryCode);
        
        // Assert
        result.ShouldNotBeNull();
        // No emitter verification - decorator tested separately
    }
}
```

**Architectural Benefits**:
- ✅ **Clean Architecture**: App layer has ZERO dependencies on observability (Audabit.Common.Observability)
- ✅ **Single Responsibility**: Services focus on business logic, decorators handle observability
- ✅ **Open/Closed Principle**: Add observability without modifying service code
- ✅ **Testability**: Test business logic and telemetry independently
- ✅ **Composability**: Chain multiple decorators for different concerns
- ✅ **Separation of Concerns**: Cross-cutting concerns separated from domain logic

**When to Use**:
- Any cross-cutting concern (logging, caching, authorization, retry, circuit breaker)
- When you need to add behavior to interfaces without modifying implementations
- When you want to keep business logic pure and testable
- When implementing aspect-oriented programming patterns

**Decorators Implemented**:
- `StarWarsServiceLoggingDecorator` - Wraps `IStarWarsService` with before/after telemetry
- `StarWarsClientLoggingDecorator` - Wraps `IStarWarsClient` with error telemetry
- `WeatherServiceLoggingDecorator` - Wraps `IWeatherService` with before/after telemetry

**Telemetry Event Location** (MOVED from App to Infra):
- **Before**: `App/StarWars/Telemetry/` and `App/Weather/Telemetry/`
- **After**: `Infra/StarWars/Telemetry/` and `Infra/Weather/Telemetry/`
- **Rationale**: Telemetry is infrastructure concern, not domain concern

---

### Pattern 4: Simple Service Pattern (Weather)

**Use Case**: Internal data generation without external dependencies

**Layers**: Service → Decorator → Mapper → Controller

**Note**: Observability handled by decorator, not service

---

### Pattern 5: Telemetry Event Pattern (Used in Decorators)

**Event Lifecycle**:
1. `{Operation}RetrievingEvent` - Before operation (emitted by decorator)
2. `{Operation}RetrievedEvent` - Success (includes result, emitted by decorator)
3. `{Operation}RetrievalFailedEvent` - Failure (includes error, emitted by decorator)

**Location**: All telemetry events now in `Infra/{Feature}/Telemetry/` (moved from App layer)

**Usage**: Structured observability for monitoring, debugging, analytics via decorator pattern

---

## Adding New Features Checklist (StarWars Pattern with Decorators)

When adding features with external API integration:

1. [ ] Define client models (`Infra/{Feature}/Clients/Models/`)
2. [ ] Add client mapper with `[Mapper]` attribute (`Infra/{Feature}/Clients/Mappers/`)
3. [ ] Implement HTTP client(s) (`Infra/{Feature}/Clients/{Feature}Client.cs`, `{Feature}ClientV2.cs` if multiple versions)
4. [ ] Add client factory implementation (`Infra/{Feature}/Clients/Factories/{Feature}ClientFactory.cs`) if multiple versions
5. [ ] **Create logging decorators** (`Infra/{Feature}/Decorators/{Feature}ServiceLoggingDecorator.cs`, `{Feature}ClientLoggingDecorator.cs`)
6. [ ] **Add ALL telemetry events to Infra** (`Infra/{Feature}/Telemetry/`) - Retrieving, Retrieved, RetrievalFailed events
7. [ ] Create client interface (`App/{Feature}/Clients/I{Feature}Client.cs`) with Version property
8. [ ] Add client factory interface (`App/{Feature}/Clients/Factories/I{Feature}ClientFactory.cs`) if multiple versions
9. [ ] Define domain models (`App.Models/{Feature}/`)
10. [ ] **Implement service with PURE business logic** (`App/{Feature}/Services/`) - **NO IEmitter, NO telemetry events**
11. [ ] Define API DTOs (`Api/{Feature}/v1/Models/` and `Transport/`)
12. [ ] Add API mapper (`WebApi/{Feature}/Mappers/`)
13. [ ] Implement controller with API versioning (`WebApi/{Feature}/Controllers/v1/`)
14. [ ] **Register services AND decorators** in `Program.cs` using Scrutor's `.Decorate<TInterface, TDecorator>()`
15. [ ] Write tests for all layers:
    - [ ] Service tests (App.Tests.Unit) - **NO emitter mocking**
    - [ ] Decorator tests (Infra.Tests.Unit) - Verify emitter calls and delegation
    - [ ] Client tests (Infra.Tests.Unit)
    - [ ] Factory tests (Infra.Tests.Unit)
    - [ ] Controller tests (WebApi.Tests.Unit)
16. [ ] Update README.md and this file

**Critical Changes from Previous Pattern**:
- ❌ **DON'T** add IEmitter parameter to services
- ❌ **DON'T** emit events from services
- ❌ **DON'T** create telemetry events in App layer
- ✅ **DO** create decorators in Infra layer
- ✅ **DO** put ALL telemetry events in Infra/*/Telemetry/
- ✅ **DO** register decorators with Scrutor in Program.cs
- ✅ **DO** test services WITHOUT emitter mocking
- ✅ **DO** test decorators WITH emitter verification

---

## CI/CD Pipeline

**Version Format**: `yyyy.MM.dd.counter` (e.g., 2026.01.20.1)  
**Pipeline Stages**: Format check → Restore → Build → Test → Pack (Api) → Publish (WebApi)  
**Artifacts**: NuGet package to Azure Artifacts, WebApi for deployment

---

## Best Practices Summary

**DO** ✅:
- Follow StarWars pattern for external APIs
- Use Riok.Mapperly for mappings
- Add telemetry events for operations
- Write tests for every layer
- Use primary constructors for DI
- Keep controllers thin

**DON'T** ❌:
- Expose domain models in API responses (use DTOs)
- Use reflection-based mappers
- Skip telemetry for important operations
- Mix business logic in controllers

---

## Documentation Standards

### XML Documentation

**Service Projects**: XML documentation is **OPTIONAL** for service project code.
- Focus on clean, self-documenting code over verbose documentation
- Use XML docs sparingly, only where truly needed for clarity
- Prefer clear naming and simple implementations

**Common Libraries**: XML documentation is **REQUIRED** for all public APIs in Audabit.Common.* libraries.

### Code Comments

**Use inline comments** (`//`) for:
- Business logic explanations
- Non-obvious implementation details
- Examples of alternative approaches

**Example** (StarWarsClientFactory):
```csharp
public IStarWarsClient GetClient(int id)
{
    // NOTE: This is just an example of business logic to choose the client version.
    // NOTE2: You could also use starWarsClients.OfType<StarWarsClient>().First() & starWarsClients.OfType<StarWarsClientV2>().First()
    var version = (id % 10 == 2) ? "v2" : "v1";
    return starWarsClients.FirstOrDefault(client => client.Version == version)
        ?? throw new InvalidOperationException($"Client version '{version}' not registered.");
}
```

---

## Modern C# Features

### .NET 9 Interface Members

**Abstract Properties** (.NET 9+):
```csharp
// ✅ Valid in .NET 9 - explicitly marks property as abstract
public interface IStarWarsClient
{
    abstract string Version { get; }
    Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken ct = default);
}
```

**Alternative** (Implicit abstract - works in all .NET versions):
```csharp
public interface IStarWarsClient  
{
    string Version { get; } // Implicitly abstract
    Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken ct = default);
}
```

### ConfigureAwait(false) - Application Code

**Service Projects**: `.ConfigureAwait(false)` is **NOT REQUIRED**.
- ASP.NET Core doesn't have a synchronization context to capture
- Omitting ConfigureAwait improves code readability
- Performance difference is negligible in ASP.NET Core applications

**Common Libraries**: `.ConfigureAwait(false)` is **REQUIRED** on all awaits.

```csharp
// ✅ Service Projects - ConfigureAwait NOT required
var person = await client.GetPersonAsync(id, cancellationToken);

// ✅ Common Libraries - ConfigureAwait REQUIRED
var response = await httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
```

---

## Sealed Modifier Pattern - Web API Projects

### When to Use `sealed`

**Always Sealed**:
- ✅ Settings records: `public sealed record {Feature}Settings`
- ✅ Validators: `public sealed class {Feature}Validator : AbstractValidator<T>`
- ✅ Services: `public sealed class {Feature}Service : I{Feature}Service`
- ✅ Clients: `public sealed class {Feature}Client : I{Feature}Client`
- ✅ Factories: `public sealed class {Feature}ClientFactory : I{Feature}ClientFactory`
- ✅ DTOs/Transport models (optional but recommended): `public sealed record {Feature}Request`

**Never Sealed**:
- ❌ Controllers (ASP.NET framework compatibility)
- ❌ Interfaces
- ❌ Mappers (Riok.Mapperly requires partial classes)

**Examples**:
```csharp
// ✅ Settings - Always sealed
public sealed record WeatherSettings
{
    public int MinTemperature { get; init; } = -20;
    public int MaxTemperature { get; init; } = 55;
}

// ✅ Validator - Always sealed
public sealed class WeatherSettingsValidator : AbstractValidator<WeatherSettings>
{
    public WeatherSettingsValidator()
    {
        RuleFor(x => x.MinTemperature)
            .LessThan(x => x.MaxTemperature);
    }
}

// ✅ Service - Always sealed (performance + final implementation)
public sealed class WeatherService : IWeatherService
{
    // Implementation (testing done via interface)
}

// ✅ Client - Always sealed (performance + final implementation)
public sealed class StarWarsClient : IStarWarsClient
{
    public string Version => "v1";
    // Implementation (testing done via interface)
}

// ✅ Factory - Always sealed (performance + final implementation)
public sealed class StarWarsClientFactory : IStarWarsClientFactory
{
    // Implementation (testing done via interface)
}

// ✅ Request/Response DTOs - Recommended sealed
public sealed record PutWeatherForecastRequest(string SomeRandomParameter);
public sealed record GetWeatherForecastResponse(IEnumerable<WeatherForecastDto> Forecasts);

// ❌ Controller - Never sealed
public class WeatherController : ControllerBase { }

// ❌ Mapper - Never sealed (Riok.Mapperly uses partial)
[Mapper]
public partial class WeatherMapper { }
```

---

## XML Documentation Requirements - Web API Projects

### Required Documentation

**All Public Interfaces** (class + methods):
```csharp
/// <summary>
/// Contract for weather forecast operations.
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Generates random weather forecasts for the specified country.
    /// </summary>
    /// <param name="countryCode">The 2-letter country code (e.g., "US").</param>
    /// <returns>An array of weather forecasts.</returns>
    Task<WeatherForecast[]> GetForecastAsync(string countryCode);
}
```

**All Validators** (class + constructor):
```csharp
/// <summary>
/// Validator for <see cref="WeatherSettings"/> configuration.
/// </summary>
public sealed class WeatherSettingsValidator : AbstractValidator<WeatherSettings>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherSettingsValidator"/> class
    /// and configures validation rules for weather forecast settings.
    /// </summary>
    public WeatherSettingsValidator()
    {
        // Rules...
    }
}
```

**Settings with Comprehensive Examples**:
```csharp
/// <summary>
/// Configuration settings for weather forecast generation.
/// </summary>
/// <remarks>
/// <para>
/// Controls the parameters for generating random weather forecasts.
/// All settings can be configured via appsettings.json or environment variables.
/// </para>
/// </remarks>
/// <example>
/// Configuration in appsettings.json:
/// <code>
/// {
///   "WeatherSettings": {
///     "MinTemperature": -20,
///     "MaxTemperature": 55
///   }
/// }
/// </code>
/// 
/// Or using environment variables:
/// <code>
/// WeatherSettings__MinTemperature=-20
/// WeatherSettings__MaxTemperature=55
/// </code>
/// </example>
public sealed record WeatherSettings
{
    /// <summary>
    /// Gets or initializes the minimum temperature in Celsius.
    /// </summary>
    /// <value>
    /// Valid range: -100 to 100. Default is -20°C.
    /// Used as the lower bound for random temperature generation.
    /// </value>
    public int MinTemperature { get; init; } = -20;
}
```

---

## Common Libraries Integration

### Key Differences: Service Projects vs Common Libraries

| Aspect | Audabit.Service.Template (This Project) | Audabit.Common.* Libraries |
|--------|-------------------------------|---------------------------|
| **XML Documentation** | Optional (focus on clean code) | Required for all public APIs |
| **ConfigureAwait(false)** | Not required (ASP.NET Core) | Required on all awaits |
| **Target** | Web API application | Reusable NuGet packages |
| **Sealed Classes** | Services, Clients, Factories, Validators, Settings | Background services, Health checks, Validators, Settings |
| **Testing** | Unit + Integration tests | Unit tests only |
| **Package Distribution** | Api project packaged, WebApi deployed | All projects packaged to NuGet |
| **Dependencies** | Can depend on specific versions | Minimize dependencies |

### Audabit.Service.Template Usage of Common Libraries

**Infrastructure Libraries Used**:
- `Audabit.Common.ApiVersioning.AspNet` - API versioning
- `Audabit.Common.CorrelationId.AspNet` - Request correlation tracking
- `Audabit.Common.ExceptionHandling.AspNet` - Global exception handling
- `Audabit.Common.HealthChecks.AspNet` - Health check endpoints (startup, readiness, liveness)
- `Audabit.Common.HttpClient.AspNet` - Resilient HTTP clients with Polly
- `Audabit.Common.Observability` - Structured logging event system
- `Audabit.Common.Observability.AspNet` - ASP.NET Core observability extensions
- `Audabit.Common.Security.AspNet` - API key authentication
- `Audabit.Common.Serialization` - JSON serialization with sensitive data masking
- `Audabit.Common.ServiceInfo.AspNet` - Service metadata endpoint
- `Audabit.Common.Swagger.AspNet` - OpenAPI documentation
- `Audabit.Common.Validation.AspNet` - FluentValidation integration

**Registration Example** (see `Program.cs` for complete setup):
```csharp
// Infrastructure services
builder.Services.AddServiceInfo(serviceSettingsSection);
builder.Services.AddApiKeySecurity(apiKeySettingsSection);
builder.Services.AddObservability(serviceName).UseJsonConsoleLogging();
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddFluentValidationOnApis(typeof(Program));
builder.Services.AddSwaggerDocumentation(serviceName, apiKeyHeaderName, typeof(Program));
builder.Services.AddHealthChecks(healthChecksSettingsSection);

// HTTP clients with resilience
builder.Services
    .AddResilientHttpClients(httpClientSettingsSection)
    .AddResilientHttpClient<StarWarsClient>()
    .AddResilientHttpClient<StarWarsClientV2>();

// Middleware order (critical!)
app.UseExceptionMiddleware(serviceName);
app.UseCorrelationIdMiddleware();
app.UseApiKeyMiddleware(app.Environment.IsDevelopment());
app.UseHealthChecks();
```

---

**For detailed Common library development standards, see**:
- Root: `C:\\Users\\John.Schaap\\Desktop\\Audabit\\.github\\copilot-instructions.md`
- Each library's `.github/copilot-instructions.md` in their respective repositories

---

## Folder Structure - Feature-First Organization

### Feature-First Principles

The Audabit.Service.Template uses **feature-first** (vertical slice) organization instead of **type-first** (layer-based) organization. This means files are grouped by feature (StarWars, Weather) rather than by technical type (Controllers, Services, etc.).

**Benefits**:
- Related code stays together (easier navigation and maintenance)
- Features can be added/removed independently
- Better code locality and cohesion
- Clear feature boundaries

### Project Structure

```
Audabit.Service.Template.Api/
├── StarWars/
│   └── v1/
│       ├── Models/
│       │   └── StarWarsPersonDto.cs
│       └── Transport/
│           └── GetStarWarsPersonResponse.cs
└── Weather/
    └── v1/
        ├── Models/
        │   └── WeatherForecastDto.cs
        └── Transport/
            ├── GetWeatherForecastResponse.cs
            ├── PutWeatherForecastRequest.cs
            └── PutWeatherForecastResponse.cs

Audabit.Service.Template.App.Models/
├── StarWars/
│   └── StarWarsPerson.cs
└── Weather/
    └── WeatherForecast.cs

Audabit.Service.Template.App/
├── StarWars/
│   ├── Clients/
│   │   ├── Factories/
│   │   │   └── IStarWarsClientFactory.cs
│   │   └── IStarWarsClient.cs
│   └── Services/
│       ├── IStarWarsService.cs
│       └── StarWarsService.cs
└── Weather/
    ├── Services/
    │   ├── IWeatherService.cs
    │   └── WeatherService.cs
    ├── Settings/
    │   └── WeatherSettings.cs
    └── Validators/
        └── Settings/
            └── WeatherSettingsValidator.cs

Audabit.Service.Template.Infra/
├── StarWars/
│   ├── Clients/
│   │   ├── Factories/
│   │   │   └── StarWarsClientFactory.cs
│   │   ├── Mappers/
│   │   │   └── StarWarsClientMapper.cs
│   │   ├── Models/
│   │   │   └── StarWarsPersonClientModel.cs
│   │   ├── StarWarsClient.cs
│   │   └── StarWarsClientV2.cs
│   ├── Decorators/
│   │   ├── StarWarsServiceLoggingDecorator.cs
│   │   └── StarWarsClientLoggingDecorator.cs
│   └── Telemetry/
│       ├── StarWarsPersonRetrievingEvent.cs
│       ├── StarWarsPersonRetrievedEvent.cs
│       └── StarWarsPersonRetrievalFailedEvent.cs
└── Weather/
    ├── Decorators/
    │   └── WeatherServiceLoggingDecorator.cs
    └── Telemetry/
        ├── WeatherForecastGeneratingEvent.cs
        └── WeatherForecastGeneratedEvent.cs

Audabit.Service.Template.WebApi/
├── StarWars/
│   ├── Controllers/
│   │   └── v1/
│   │       └── StarWarsController.cs
│   └── Mappers/
│       └── StarWarsMapper.cs
├── Weather/
│   ├── Controllers/
│   │   └── v1/
│   │       └── WeatherController.cs
│   ├── Mappers/
│   │   └── WeatherMapper.cs
│   └── Validators/
│       └── v1/
│           └── Transport/
│               └── PutWeatherForecastRequestValidator.cs
└── Shared/
    └── SwaggerExamples/
        └── ExampleConstants.cs

Tests/Audabit.Service.Template.App.Tests.Unit/
├── StarWars/
│   └── Services/
│       └── StarWarsServiceTests.cs
└── Weather/
    ├── Services/
    │   └── WeatherServiceTests.cs
    └── Validators/
        └── Settings/
            └── WeatherSettingsValidatorTests.cs

Tests/Audabit.Service.Template.Infra.Tests.Unit/
├── StarWars/
│   ├── Clients/
│   │   ├── Factories/
│   │   │   └── StarWarsClientFactoryTests.cs
│   │   ├── Mappers/
│   │   │   └── StarWarsClientMapperTests.cs
│   │   ├── StarWarsClientTests.cs
│   │   └── StarWarsClientV2Tests.cs
│   └── Decorators/
│       ├── StarWarsServiceLoggingDecoratorTests.cs
│       └── StarWarsClientLoggingDecoratorTests.cs
└── Weather/
    └── Decorators/
        └── WeatherServiceLoggingDecoratorTests.cs

Tests/Audabit.Service.Template.WebApi.Tests.Unit/
├── StarWars/
│   ├── Controllers/
│   │   └── v1/
│   │       └── StarWarsControllerTests.cs
│   └── Mappers/
│       └── StarWarsMapperTests.cs
└── Weather/
    ├── Controllers/
    │   └── v1/
    │       └── WeatherControllerTests.cs
    ├── Mappers/
    │   └── WeatherMapperTests.cs
    └── Validators/
        └── v1/
            └── Transport/
                └── PutWeatherForecastRequestValidatorTests.cs

Tests/Audabit.Service.Template.Tests.Integration/
├── StarWars/
│   └── Controllers/
│       └── v1/
│           └── StarWarsControllerIntegrationTests.cs
└── Weather/
    └── Controllers/
        └── v1/
            └── WeatherControllerIntegrationTests.cs
```

### Namespace Convention

Namespaces follow the folder structure **exactly**:

**Examples**:
- `Audabit.Service.Template.Api.StarWars.v1.Models`
- `Audabit.Service.Template.App.Models.StarWars`
- `Audabit.Service.Template.App.StarWars.Clients` (interfaces only)
- `Audabit.Service.Template.App.StarWars.Clients.Factories` (interfaces only)
- `Audabit.Service.Template.App.StarWars.Services`
- `Audabit.Service.Template.Infra.StarWars.Clients` (implementations)
- `Audabit.Service.Template.Infra.StarWars.Clients.Factories` (implementations)
- `Audabit.Service.Template.Infra.StarWars.Clients.Mappers`
- `Audabit.Service.Template.Infra.StarWars.Decorators`
- `Audabit.Service.Template.Infra.StarWars.Telemetry`
- `Audabit.Service.Template.Infra.Weather.Decorators`
- `Audabit.Service.Template.Infra.Weather.Telemetry`
- `Audabit.Service.Template.WebApi.StarWars.Controllers.v1`
- `Audabit.Service.Template.WebApi.Weather.Mappers`
- `Audabit.Service.Template.WebApi.Shared.SwaggerExamples`

**Critical Rule**: Namespace must match the physical folder path from the project root.

---

**Last Updated**: January 27, 2026  
**Maintainer**: John Schaap  
**License**: Apache 2.0
