using System.Diagnostics.CodeAnalysis;
using Audabit.Service.Template.Api.StarWars.v1.Models;
using Audabit.Service.Template.Api.StarWars.v1.Transport;
using Swashbuckle.AspNetCore.Filters;

namespace Audabit.Service.Template.WebApi.StarWars.Controllers.v1.SwaggerExamples;

[ExcludeFromCodeCoverage]
public class GetStarWarsPersonResponseExample : IMultipleExamplesProvider<GetStarWarsPersonResponse>
{
    public IEnumerable<SwaggerExample<GetStarWarsPersonResponse>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Luke Skywalker",
            new GetStarWarsPersonResponse(
                new StarWarsPersonDto(
                    "Luke Skywalker",
                    "172"
                )
            )
        );

        yield return SwaggerExample.Create(
            "Darth Vader",
            new GetStarWarsPersonResponse(
                new StarWarsPersonDto(
                    "Darth Vader",
                    "202"
                )
            )
        );

        yield return SwaggerExample.Create(
            "Leia Organa",
            new GetStarWarsPersonResponse(
                new StarWarsPersonDto(
                    "Leia Organa",
                    "150"
                )
            )
        );
    }
}