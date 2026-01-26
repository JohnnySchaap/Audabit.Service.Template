using System.Diagnostics.CodeAnalysis;
using Audabit.Service.Template.Api.Weather.v1.Transport;
using Audabit.Service.Template.WebApi.Shared.SwaggerExamples;
using Swashbuckle.AspNetCore.Filters;

namespace Audabit.Service.Template.WebApi.Weather.Controllers.v1.SwaggerExamples;

[ExcludeFromCodeCoverage]
public class PutWeatherForecastResponseExample : IMultipleExamplesProvider<PutWeatherForecastResponse>
{
    public IEnumerable<SwaggerExample<PutWeatherForecastResponse>> GetExamples()
    {
        yield return SwaggerExample.Create(ExampleConstants.SuccessfulResponse, new PutWeatherForecastResponse());
    }
}