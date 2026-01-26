using Audabit.Service.Template.App.Models.StarWars;
using Audabit.Service.Template.Infra.StarWars.Clients.Models;
using Riok.Mapperly.Abstractions;

namespace Audabit.Service.Template.Infra.StarWars.Clients.Mappers;

[Mapper]
public partial class StarWarsClientMapper
{
    [MapProperty(nameof(StarWarsPersonClientModel.Name), nameof(StarWarsPerson.Name))]
    [MapProperty(nameof(StarWarsPersonClientModel.Height), nameof(StarWarsPerson.Height))]
    public partial StarWarsPerson MapToDomain(StarWarsPersonClientModel clientModel);
}