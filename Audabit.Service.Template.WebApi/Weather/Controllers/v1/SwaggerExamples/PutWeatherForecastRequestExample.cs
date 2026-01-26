using System.Diagnostics.CodeAnalysis;
using Audabit.Service.Template.Api.Weather.v1.Transport;
using Swashbuckle.AspNetCore.Filters;

namespace Audabit.Service.Template.WebApi.Weather.Controllers.v1.SwaggerExamples;

[ExcludeFromCodeCoverage]
public class PutWeatherForecastRequestExample : IExamplesProvider<PutWeatherForecastRequest>
{
    public PutWeatherForecastRequest GetExamples()
    {
        return new PutWeatherForecastRequest("example parameter value");
    }
}