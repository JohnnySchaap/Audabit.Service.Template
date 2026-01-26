namespace Audabit.Service.Template.WebApi.Tests.Unit.TestHelpers;

/// <summary>
/// Factory for creating configured AutoFixture instances for unit tests.
/// </summary>
public static class FixtureFactory
{
    /// <summary>
    /// Creates a new AutoFixture instance configured for testing.
    /// </summary>
    /// <returns>A configured <see cref="Fixture"/> instance with custom DateOnly generation.</returns>
    /// <remarks>
    /// Configures DateOnly to generate dates 1-10 days in the future for realistic weather forecast testing.
    /// </remarks>
    public static Fixture Create()
    {
        var fixture = new Fixture();

        fixture.Customize<DateOnly>(composer => composer
            .FromFactory(() => DateOnly.FromDateTime(DateTime.Now.AddDays(Random.Shared.Next(1, 10)))));

        return fixture;
    }
}