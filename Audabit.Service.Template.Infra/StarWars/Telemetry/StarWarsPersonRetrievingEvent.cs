using System.Diagnostics.CodeAnalysis;
using Audabit.Common.Observability.Events;

namespace Audabit.Service.Template.Infra.StarWars.Telemetry;

[ExcludeFromCodeCoverage]
public sealed class StarWarsPersonRetrievingEvent : LoggingEvent
{
    public StarWarsPersonRetrievingEvent(int personId)
        : base(nameof(StarWarsPersonRetrievingEvent))
    {
        Properties.Add(nameof(personId), personId);
    }
}