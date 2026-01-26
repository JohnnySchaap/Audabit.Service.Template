using System.Diagnostics.CodeAnalysis;
using Audabit.Common.Observability.Events;

namespace Audabit.Service.Template.Infra.StarWars.Telemetry;

[ExcludeFromCodeCoverage]
public sealed class StarWarsPersonRetrievalFailedEvent : ErrorEvent
{
    public StarWarsPersonRetrievalFailedEvent(Exception exception, int personId)
        : base(nameof(StarWarsPersonRetrievalFailedEvent), exception?.Message ?? string.Empty, exception: exception, null)
    {
        Properties.Add(nameof(personId), personId);
    }
}