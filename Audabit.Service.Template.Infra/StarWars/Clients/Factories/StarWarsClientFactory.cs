using Audabit.Service.Template.App.StarWars.Clients;
using Audabit.Service.Template.App.StarWars.Clients.Factories;

namespace Audabit.Service.Template.Infra.StarWars.Clients.Factories;

public sealed class StarWarsClientFactory(IEnumerable<IStarWarsClient> starWarsClients) : IStarWarsClientFactory
{
    public IStarWarsClient GetClient(int id)
    {
        // NOTE: This is just an example of business logic to choose the client version.
        // NOTE2: You could also use starWarsClients.OfType<StarWarsClient>().First() &  starWarsClients.OfType<StarWarsClientV2>().First()
        var version = (id % 10 == 2) ? "v2" : "v1";

        return starWarsClients.FirstOrDefault(client => client.Version == version)
            ?? throw new InvalidOperationException($"Star Wars client version '{version}' is not registered.");
    }
}