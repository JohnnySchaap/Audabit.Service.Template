using System.Diagnostics.CodeAnalysis;
using Audabit.Common.Observability.Events;
using Audabit.Common.Serialization.Extensions;
using Audabit.Service.Template.App.Models.StarWars;

namespace Audabit.Service.Template.Infra.StarWars.Telemetry;

[ExcludeFromCodeCoverage]
public sealed class StarWarsPersonRetrievedEvent : LoggingEvent
{
    public StarWarsPersonRetrievedEvent(StarWarsPerson? person, int personId)
        : base(nameof(StarWarsPersonRetrievedEvent))
    {
        Properties.Add(nameof(person), person?.ToJson(maskSensitiveData: true) ?? "null");
        Properties.Add(nameof(personId), personId);
    }
}