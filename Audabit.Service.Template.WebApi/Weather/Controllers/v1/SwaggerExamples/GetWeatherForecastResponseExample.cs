using System.Diagnostics.CodeAnalysis;
using Audabit.Service.Template.Api.Weather.v1.Transport;
using Audabit.Service.Template.WebApi.Shared.SwaggerExamples;
using Swashbuckle.AspNetCore.Filters;

namespace Audabit.Service.Template.WebApi.Weather.Controllers.v1.SwaggerExamples;

[ExcludeFromCodeCoverage]
public class GetWeatherForecastResponseExample : IMultipleExamplesProvider<GetWeatherForecastResponse>
{
    public IEnumerable<SwaggerExample<GetWeatherForecastResponse>> GetExamples()
    {
        yield return SwaggerExample.Create(
            ExampleConstants.SuccessfulResponse,
            new GetWeatherForecastResponse(
                [
                    new(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 15, "Cool"),
                    new(DateOnly.FromDateTime(DateTime.Now.AddDays(2)), 20, "Mild"),
                    new(DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 25, "Warm"),
                    new(DateOnly.FromDateTime(DateTime.Now.AddDays(4)), 18, "Mild"),
                    new(DateOnly.FromDateTime(DateTime.Now.AddDays(5)), 12, "Chilly")
                ]
            )
        );
    }
}