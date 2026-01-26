using Audabit.Service.Template.Api.Weather.v1.Models;

namespace Audabit.Service.Template.Api.Weather.v1.Transport;

public record GetWeatherForecastResponse(IEnumerable<WeatherForecastDto> Forecasts);