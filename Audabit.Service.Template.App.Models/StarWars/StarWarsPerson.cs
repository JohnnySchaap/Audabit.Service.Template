using Audabit.Common.Serialization.Attributes;

namespace Audabit.Service.Template.App.Models.StarWars;

public record StarWarsPerson(
    [property: SensitiveData] string Name,
    string Height);