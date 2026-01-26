using Audabit.Common.Serialization.Attributes;

namespace Audabit.Service.Template.App.Models.Weather;

public record WeatherForecast(
    DateOnly Date,
    int TemperatureC,
    string Summary,
    [property: SensitiveData] string PersonalInformation,
    SecretWeatherForecastObject ObjectWithSecret,
    [property: SensitiveData] SecretWeatherForecastObject SecretObject);

public record SecretWeatherForecastObject(
    string Data,
    [property: SensitiveData] string Secret);