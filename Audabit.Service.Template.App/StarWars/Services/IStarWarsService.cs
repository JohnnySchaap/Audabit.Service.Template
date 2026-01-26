using Audabit.Service.Template.App.Models.StarWars;

namespace Audabit.Service.Template.App.StarWars.Services;

public interface IStarWarsService
{
    Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken cancellationToken = default);
}