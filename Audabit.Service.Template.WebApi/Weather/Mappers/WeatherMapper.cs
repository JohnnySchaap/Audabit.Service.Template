using Audabit.Service.Template.Api.Weather.v1.Models;
using Audabit.Service.Template.App.Models.Weather;
using Riok.Mapperly.Abstractions;

namespace Audabit.Service.Template.WebApi.Weather.Mappers;

[Mapper]
public partial class WeatherMapper
{
    [MapProperty(nameof(WeatherForecast.Date), nameof(WeatherForecastDto.Date))]
    [MapProperty(nameof(WeatherForecast.TemperatureC), nameof(WeatherForecastDto.TemperatureC))]
    [MapProperty(nameof(WeatherForecast.Summary), nameof(WeatherForecastDto.Summary))]
    [MapperIgnoreSource(nameof(WeatherForecast.PersonalInformation))]
    [MapperIgnoreSource(nameof(WeatherForecast.SecretObject))]
    [MapperIgnoreSource(nameof(WeatherForecast.ObjectWithSecret))]
    public partial WeatherForecastDto ToDto(WeatherForecast forecast);

    public partial IEnumerable<WeatherForecastDto> ToDto(IEnumerable<WeatherForecast> forecasts);
}