using Audabit.Service.Template.App.Models.StarWars;
using Audabit.Service.Template.App.StarWars.Clients;
using Audabit.Service.Template.App.StarWars.Clients.Factories;
using Audabit.Service.Template.App.StarWars.Services;
using Audabit.Service.Template.App.Tests.Unit.TestHelpers;

namespace Audabit.Service.Template.App.Tests.Unit.StarWars.Services;

public class StarWarsServiceTests
{
    public class StarWarsServiceTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly IStarWarsClient _starWarsClient;
        protected readonly IStarWarsClientFactory _starWarsClientFactory;
        protected readonly StarWarsService _starWarsService;

        public StarWarsServiceTestsBase()
        {
            _fixture = FixtureFactory.Create();
            _starWarsClient = Substitute.For<IStarWarsClient>();
            _starWarsClientFactory = Substitute.For<IStarWarsClientFactory>();
            _starWarsClientFactory.GetClient(Arg.Any<int>()).Returns(_starWarsClient);
            _starWarsService = new StarWarsService(_starWarsClientFactory);
        }
    }

    public class GetPersonAsync : StarWarsServiceTestsBase
    {
        [Theory, AutoData]
        public async Task GivenPersonExists_ShouldReturnPersonAndRaiseTelemetry(int id)
        {
            // Arrange
            var person = _fixture.Create<StarWarsPerson>();
            _starWarsClient.GetPersonAsync(id, Arg.Any<CancellationToken>()).Returns(person);

            // Act
            var result = await _starWarsService.GetPersonAsync(id, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(person);

            await _starWarsClient.Received(1).GetPersonAsync(id, Arg.Any<CancellationToken>());
        }

        [Theory, AutoData]
        public async Task GivenPersonDoesNotExist_ShouldReturnNullAndRaiseTelemetry(int id)
        {
            // Arrange
            _starWarsClient.GetPersonAsync(id, Arg.Any<CancellationToken>()).Returns((StarWarsPerson?)null);

            // Act
            var result = await _starWarsService.GetPersonAsync(id, CancellationToken.None);

            // Assert
            result.ShouldBeNull();

            await _starWarsClient.Received(1).GetPersonAsync(id, Arg.Any<CancellationToken>());
        }

        [Theory, AutoData]
        public async Task GivenClientThrowsException_ShouldPropagateExceptionAndRaiseRetrievingEvent(int id)
        {
            // Arrange
            var exception = new HttpRequestException("Network error");
            _starWarsClient.GetPersonAsync(id, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<StarWarsPerson?>(exception));

            // Act & Assert
            await Should.ThrowAsync<HttpRequestException>(async () =>
                await _starWarsService.GetPersonAsync(id, CancellationToken.None));
        }

        [Theory, AutoData]
        public async Task GivenCancellationRequested_ShouldPassCancellationToken(int id)
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var person = _fixture.Create<StarWarsPerson>();
            _starWarsClient.GetPersonAsync(id, cancellationToken).Returns(person);

            // Act
            var result = await _starWarsService.GetPersonAsync(id, cancellationToken);

            // Assert
            await _starWarsClient.Received(1).GetPersonAsync(id, cancellationToken);
            result.ShouldBe(person);
        }
    }
}