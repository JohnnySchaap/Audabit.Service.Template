using System.Net;
using System.Text.Json;
using Audabit.Service.Template.App.Models.StarWars;
using Audabit.Service.Template.App.StarWars.Clients;
using Audabit.Service.Template.Infra.StarWars.Clients.Mappers;
using Audabit.Service.Template.Infra.StarWars.Clients.Models;

namespace Audabit.Service.Template.Infra.StarWars.Clients;

public sealed class StarWarsClient(IHttpClientFactory httpClientFactory, StarWarsClientMapper mapper) : IStarWarsClient
{
    public string Version => "v1";

    public async Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken cancellationToken = default)
    {
        var httpClient = httpClientFactory.CreateClient(GetType().Name);

        var endpoint = $"people/{id}/";

        var response = await httpClient.GetAsync(endpoint, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var clientModel = JsonSerializer.Deserialize<StarWarsPersonClientModel>(content);
        var person = clientModel is not null ? mapper.MapToDomain(clientModel) : null;

        return person;
    }
}