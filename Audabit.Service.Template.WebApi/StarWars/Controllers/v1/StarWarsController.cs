using System.Net.Mime;
using Asp.Versioning;
using Audabit.Service.Template.Api.StarWars.v1.Transport;
using Audabit.Service.Template.App.StarWars.Services;
using Audabit.Service.Template.WebApi.StarWars.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.Template.WebApi.StarWars.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")] // If you want to have the version in the url
//[Route("[controller]")] // If you want to use header/query string versioning only
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class StarWarsController(IStarWarsService starWarsService, StarWarsMapper mapper) : ControllerBase
{
    /// <summary>
    /// Gets a Star Wars character by ID.
    /// </summary>
    /// <param name="id">The character ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The character details.</returns>
    [HttpGet("Person/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetStarWarsPersonResponse))]
    public async Task<IActionResult> GetPerson(int id, CancellationToken cancellationToken)
    {
        var person = await starWarsService.GetPersonAsync(id, cancellationToken);

        if (person == null)
        {
            return NotFound();
        }

        var response = new GetStarWarsPersonResponse(mapper.MapToDto(person));

        return Ok(response);
    }
}