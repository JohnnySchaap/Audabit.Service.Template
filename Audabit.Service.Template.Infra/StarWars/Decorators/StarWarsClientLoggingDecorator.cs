using Audabit.Common.Observability.Emitters;
using Audabit.Common.Observability.Extensions;
using Audabit.Service.Template.App.Models.StarWars;
using Audabit.Service.Template.App.StarWars.Clients;
using Audabit.Service.Template.Infra.StarWars.Telemetry;

namespace Audabit.Service.Template.Infra.StarWars.Decorators;

public sealed class StarWarsClientLoggingDecorator(
    IStarWarsClient inner,
    IEmitter<StarWarsClientLoggingDecorator> emitter) : IStarWarsClient
{
    public string Version => inner.Version;

    public async Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var person = await inner.GetPersonAsync(id, cancellationToken);
            return person;
        }
        catch (Exception ex)
        {
            emitter.RaiseError(new StarWarsPersonRetrievalFailedEvent(ex, id));
            throw;
        }
    }
}