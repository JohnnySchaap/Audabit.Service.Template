using Audabit.Service.Template.Api.StarWars.v1.Models;
using Audabit.Service.Template.App.Models.StarWars;
using Riok.Mapperly.Abstractions;

namespace Audabit.Service.Template.WebApi.StarWars.Mappers;

[Mapper]
public partial class StarWarsMapper
{
    [MapProperty(nameof(StarWarsPerson.Name), nameof(StarWarsPersonDto.Name))]
    [MapProperty(nameof(StarWarsPerson.Height), nameof(StarWarsPersonDto.Height))]
    public partial StarWarsPersonDto MapToDto(StarWarsPerson person);
}