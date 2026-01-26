using System.Text.Json.Serialization;
using Audabit.Service.Template.Api.StarWars.v1.Models;

namespace Audabit.Service.Template.Api.StarWars.v1.Transport;

public record GetStarWarsPersonResponse([property: JsonPropertyName("person")] StarWarsPersonDto? Person);