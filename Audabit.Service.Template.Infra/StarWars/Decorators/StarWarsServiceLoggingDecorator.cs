using Audabit.Common.Observability.Emitters;
using Audabit.Common.Observability.Extensions;
using Audabit.Service.Template.App.Models.StarWars;
using Audabit.Service.Template.App.StarWars.Services;
using Audabit.Service.Template.Infra.StarWars.Telemetry;

namespace Audabit.Service.Template.Infra.StarWars.Decorators;

public sealed class StarWarsServiceLoggingDecorator(
    IStarWarsService inner,
    IEmitter<StarWarsServiceLoggingDecorator> emitter) : IStarWarsService
{
    public async Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken cancellationToken = default)
    {
        // If you want to get the correlation ID in the service layer, you can retrieve it from Activity baggage
        // var correlationId = Activity.Current?.GetBaggageItem("X-Correlation-Id");

        emitter.RaiseDebug(new StarWarsPersonRetrievingEvent(id));

        var person = await inner.GetPersonAsync(id, cancellationToken);

        emitter.RaiseDebug(new StarWarsPersonRetrievedEvent(person, id));

        return person;
    }
}