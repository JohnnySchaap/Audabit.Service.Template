using Audabit.Service.Template.Api.Weather.v1.Transport;
using FluentValidation;

namespace Audabit.Service.Template.WebApi.Weather.Validators.v1.Transport;

public sealed class PutWeatherForecastRequestValidator : AbstractValidator<PutWeatherForecastRequest>
{
    public PutWeatherForecastRequestValidator()
    {
        RuleFor(x => x.SomeRandomParameter)
            .NotEmpty()
            .WithMessage("SomeRandomParameter is required")
            .MaximumLength(50)
            .WithMessage("SomeRandomParameter must not exceed 50 characters");

        // TODO: If you have a nested object, you can do something like: RuleFor(x => x.HeadQuarters).SetValidator(new AddressValidator());
    }
}