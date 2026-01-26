using System.Text.Json.Serialization;

namespace Audabit.Service.Template.Infra.StarWars.Clients.Models;

public record StarWarsPersonClientModel(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("height")] string Height);