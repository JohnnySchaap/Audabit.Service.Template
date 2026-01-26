using Audabit.Common.ApiVersioning.AspNet.Extensions;
using Audabit.Common.CorrelationId.AspNet.Extensions;
using Audabit.Common.ExceptionHandling.AspNet.Extensions;
using Audabit.Common.HealthChecks.AspNet.Extensions;
using Audabit.Common.HealthChecks.AspNet.Settings;
using Audabit.Common.HttpClient.AspNet.Extensions;
using Audabit.Common.HttpClient.AspNet.Settings;
using Audabit.Common.Observability.AspNet.Extensions;
using Audabit.Common.Security.AspNet.Extensions;
using Audabit.Common.Security.AspNet.Settings;
using Audabit.Common.ServiceInfo.AspNet.Extensions;
using Audabit.Common.ServiceInfo.AspNet.Settings;
using Audabit.Common.Swagger.AspNet.Extensions;
using Audabit.Common.Validation.AspNet.Extensions;
using Audabit.Service.Template.App.StarWars.Clients;
using Audabit.Service.Template.App.StarWars.Clients.Factories;
using Audabit.Service.Template.App.StarWars.Services;
using Audabit.Service.Template.App.Weather.Services;
using Audabit.Service.Template.App.Weather.Settings;
using Audabit.Service.Template.Infra.StarWars.Clients;
using Audabit.Service.Template.Infra.StarWars.Clients.Factories;
using Audabit.Service.Template.Infra.StarWars.Clients.Mappers;
using Audabit.Service.Template.Infra.StarWars.Decorators;
using Audabit.Service.Template.Infra.Weather.Decorators;
using Audabit.Service.Template.WebApi.StarWars.Mappers;
using Audabit.Service.Template.WebApi.Weather.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure settings (Common libraries)
var apiKeySettingsSection = builder.Configuration.GetSection(nameof(ApiKeySettings));
var serviceSettingsSection = builder.Configuration.GetSection(nameof(ServiceSettings));
var healthChecksSettingsSection = builder.Configuration.GetSection(nameof(HealthChecksSettings));
var httpClientSettingsSection = builder.Configuration.GetSection(nameof(HttpClientSettings));
var serviceName = builder.Configuration.GetValue<string>($"{nameof(ServiceSettings)}:{nameof(ServiceSettings.ServiceName)}") ?? "UnknownService";
var apiKeyHeaderName = apiKeySettingsSection.GetValue<string>(nameof(ApiKeySettings.HeaderName)) ?? "X-API-Key";

// Infrastructure services (Common libraries)
builder.Services.AddServiceInfo(serviceSettingsSection);
builder.Services.AddApiKeySecurity(apiKeySettingsSection);
builder.Services.AddObservability(serviceName).UseJsonConsoleLogging();
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddFluentValidationOnApis(typeof(Program));
builder.Services.AddSwaggerDocumentation(serviceName, apiKeyHeaderName, typeof(Program));
builder.Services.AddHealthChecks(healthChecksSettingsSection);

// ASP.NET Core framework services
builder.Services.AddControllers();

// Application settings
var weatherSettingsSection = builder.Configuration.GetSection(nameof(WeatherSettings));
builder.Services
    .AddOptions<WeatherSettings>()
    .Bind(weatherSettingsSection)
    .ValidateWithFluentValidation();

// Application services
builder.Services.AddSingleton<WeatherMapper>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.Decorate<IWeatherService, WeatherServiceLoggingDecorator>();
builder.Services.AddSingleton<StarWarsMapper>();
builder.Services.AddSingleton<StarWarsClientMapper>();
builder.Services.AddScoped<IStarWarsService, StarWarsService>();
builder.Services.Decorate<IStarWarsService, StarWarsServiceLoggingDecorator>();
builder.Services.AddScoped<IStarWarsClient, StarWarsClient>();
builder.Services.AddScoped<IStarWarsClient, StarWarsClientV2>();
builder.Services.Decorate<IStarWarsClient, StarWarsClientLoggingDecorator>();
builder.Services.AddScoped<IStarWarsClientFactory, StarWarsClientFactory>();

// Application HTTP clients with Polly resilience policies
builder.Services
    .AddResilientHttpClients(httpClientSettingsSection)
    .AddResilientHttpClient<StarWarsClient>()
    .AddResilientHttpClient<StarWarsClientV2>();

var app = builder.Build();

// Configure the HTTP request pipeline (order matters!)
app.UseExceptionMiddleware(serviceName);
app.UseCorrelationIdMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithVersioning();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseApiKeyMiddleware(app.Environment.IsDevelopment());

app.UseHealthChecks();
app.MapControllers();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }