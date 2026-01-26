using Audabit.Common.Observability.Emitters;
using Audabit.Common.Observability.Extensions;
using Audabit.Service.Template.App.Models.Weather;
using Audabit.Service.Template.App.Weather.Services;
using Audabit.Service.Template.Infra.Weather.Telemetry;

namespace Audabit.Service.Template.Infra.Weather.Decorators;

public sealed class WeatherServiceLoggingDecorator(
    IWeatherService inner,
    IEmitter<WeatherServiceLoggingDecorator> emitter) : IWeatherService
{
    public async Task<WeatherForecast[]> GetForecastAsync(string countryCode)
    {
        // If you want to get the correlation ID in the service layer, you can retrieve it from Activity baggage
        // var correlationId = Activity.Current?.GetBaggageItem("X-Correlation-Id");

        emitter.RaiseDebug(new WeatherForecastGeneratingEvent(countryCode));

        var forecasts = await inner.GetForecastAsync(countryCode);

        emitter.RaiseDebug(new WeatherForecastGeneratedEvent(forecasts, countryCode));

        return forecasts;
    }
}