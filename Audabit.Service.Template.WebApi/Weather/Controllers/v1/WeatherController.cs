using System.Net.Mime;
using Asp.Versioning;
using Audabit.Service.Template.Api.Weather.v1.Transport;
using Audabit.Service.Template.App.Weather.Services;
using Audabit.Service.Template.WebApi.Weather.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.Template.WebApi.Weather.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")] // If you want to have the version in the url
//[Route("[controller]")] // If you want to use header/query string versioning only
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class WeatherController(
    IWeatherService weatherService,
    WeatherMapper mapper) : ControllerBase
{
    /// <summary>
    /// Gets a 5-day weather forecast for the specified country.
    /// </summary>
    [HttpGet("forecast/{countryCode:length(2)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetWeatherForecastResponse))]
    public async Task<IActionResult> GetWeatherForecastAsync(string countryCode)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(countryCode);
        await Task.CompletedTask;

        var forecasts = await weatherService.GetForecastAsync(countryCode);
        var response = new GetWeatherForecastResponse(mapper.ToDto(forecasts));

        return Ok(response);
    }

    /// <summary>
    /// Updates weather forecast data for the specified country.
    /// </summary>
    [HttpPut("forecast/{countryCode:length(2)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PutWeatherForecastResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutWeatherForecastAsync(string countryCode, PutWeatherForecastRequest request)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(countryCode);
        ArgumentNullException.ThrowIfNull(request);
        await Task.CompletedTask;

        return Ok(new PutWeatherForecastResponse());
    }
}