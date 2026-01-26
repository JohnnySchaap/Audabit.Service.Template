using System.Diagnostics.CodeAnalysis;
using Audabit.Common.Observability.Events;

namespace Audabit.Service.Template.Infra.Weather.Telemetry;

[ExcludeFromCodeCoverage]
public sealed class WeatherForecastGeneratingEvent : LoggingEvent
{
    public WeatherForecastGeneratingEvent(string countryCode)
        : base(nameof(WeatherForecastGeneratingEvent))
    {
        Properties.Add(nameof(countryCode), countryCode);
    }
}