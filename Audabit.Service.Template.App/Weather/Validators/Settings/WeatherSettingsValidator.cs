using Audabit.Service.Template.App.Weather.Settings;
using FluentValidation;

namespace Audabit.Service.Template.App.Weather.Validators.Settings;

public sealed class WeatherSettingsValidator : AbstractValidator<WeatherSettings>
{
    public WeatherSettingsValidator()
    {
        RuleFor(x => x.MinTemperature)
            .GreaterThanOrEqualTo(-100)
            .WithMessage("WeatherSettings:MinTemperature must be at least -100°C")
            .LessThan(x => x.MaxTemperature)
            .WithMessage("WeatherSettings:MinTemperature must be less than MaxTemperature");

        RuleFor(x => x.MaxTemperature)
            .LessThanOrEqualTo(100)
            .WithMessage("WeatherSettings:MaxTemperature must not exceed 100°C")
            .GreaterThan(x => x.MinTemperature)
            .WithMessage("WeatherSettings:MaxTemperature must be greater than MinTemperature");

        RuleFor(x => x.ForecastDays)
            .GreaterThan(0)
            .WithMessage("WeatherSettings:ForecastDays must be greater than 0")
            .LessThanOrEqualTo(30)
            .WithMessage("WeatherSettings:ForecastDays must not exceed 30 days");
    }
}