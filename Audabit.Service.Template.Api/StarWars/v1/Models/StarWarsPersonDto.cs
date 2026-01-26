using System.Text.Json.Serialization;

namespace Audabit.Service.Template.Api.StarWars.v1.Models;

public record StarWarsPersonDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("height")] string Height);