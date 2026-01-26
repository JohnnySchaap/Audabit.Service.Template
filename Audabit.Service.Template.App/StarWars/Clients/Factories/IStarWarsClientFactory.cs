namespace Audabit.Service.Template.App.StarWars.Clients.Factories;

public interface IStarWarsClientFactory
{
    IStarWarsClient GetClient(int id);
}