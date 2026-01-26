using Audabit.Service.Template.App.Models.Weather;
using Audabit.Service.Template.App.Weather.Settings;
using Microsoft.Extensions.Options;

namespace Audabit.Service.Template.App.Weather.Services;

public sealed class WeatherService(IOptions<WeatherSettings> settings) : IWeatherService
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public async Task<WeatherForecast[]> GetForecastAsync(string countryCode)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(countryCode);
        await Task.CompletedTask;

        var forecastSettings = settings.Value;
        var forecasts = Enumerable
            .Range(1, forecastSettings.ForecastDays)
            .Select(index => new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(forecastSettings.MinTemperature, forecastSettings.MaxTemperature),
                Summaries[Random.Shared.Next(Summaries.Length)],
                "Sensitive Personal Info",
                new SecretWeatherForecastObject("SampleData", "SensitiveSecret"),
                new SecretWeatherForecastObject("These are some notes.", "These are some sensitive notes.")
            )).ToArray();

        return forecasts;
    }
}