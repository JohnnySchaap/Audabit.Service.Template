namespace Audabit.Service.Template.Api.Weather.v1.Models;

public record WeatherForecastDto(DateOnly Date, int TemperatureC, string? Summary);