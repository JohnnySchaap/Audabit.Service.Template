using Audabit.Service.Template.App.Models.Weather;

namespace Audabit.Service.Template.App.Weather.Services;

public interface IWeatherService
{
    Task<WeatherForecast[]> GetForecastAsync(string countryCode);
}