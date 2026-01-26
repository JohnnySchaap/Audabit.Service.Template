using System.Diagnostics.CodeAnalysis;
using Audabit.Common.Observability.Events;
using Audabit.Common.Serialization.Extensions;
using Audabit.Service.Template.App.Models.Weather;

namespace Audabit.Service.Template.Infra.Weather.Telemetry;

[ExcludeFromCodeCoverage]
public sealed class WeatherForecastGeneratedEvent : LoggingEvent
{
    public WeatherForecastGeneratedEvent(WeatherForecast[] weatherForecasts, string countryCode)
        : base(nameof(WeatherForecastGeneratedEvent))
    {
        Properties.Add(nameof(weatherForecasts), weatherForecasts.ToJson(maskSensitiveData: true));
        Properties.Add(nameof(countryCode), countryCode);
    }
}