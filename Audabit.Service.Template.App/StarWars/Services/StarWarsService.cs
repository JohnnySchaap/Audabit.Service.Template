using Audabit.Service.Template.App.Models.StarWars;
using Audabit.Service.Template.App.StarWars.Clients.Factories;

namespace Audabit.Service.Template.App.StarWars.Services;

public sealed class StarWarsService(IStarWarsClientFactory starWarsClientFactory) : IStarWarsService
{
    public async Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = starWarsClientFactory.GetClient(id);
        var person = await client.GetPersonAsync(id, cancellationToken);

        return person;
    }
}