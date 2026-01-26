using Audabit.Service.Template.App.Models.StarWars;

namespace Audabit.Service.Template.App.StarWars.Clients;

public interface IStarWarsClient
{
    abstract string Version { get; }
    Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken cancellationToken = default);
}