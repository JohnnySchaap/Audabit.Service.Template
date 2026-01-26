using Audabit.Service.Template.App.StarWars.Clients;
using Audabit.Service.Template.Infra.StarWars.Clients.Factories;
using Audabit.Service.Template.Infra.Tests.Unit.TestHelpers;

namespace Audabit.Service.Template.Infra.Tests.Unit.StarWars.Clients.Factories;

public class StarWarsClientFactoryTests
{
    public class StarWarsClientFactoryTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly IStarWarsClient _clientV1;
        protected readonly IStarWarsClient _clientV2;
        protected readonly List<IStarWarsClient> _clients;
        protected readonly StarWarsClientFactory _factory;

        public StarWarsClientFactoryTestsBase()
        {
            _fixture = FixtureFactory.Create();
            _clientV1 = Substitute.For<IStarWarsClient>();
            _clientV2 = Substitute.For<IStarWarsClient>();

            _clientV1.Version.Returns("v1");
            _clientV2.Version.Returns("v2");

            _clients = [_clientV1, _clientV2];
            _factory = new StarWarsClientFactory(_clients);
        }
    }

    public class GetClient : StarWarsClientFactoryTestsBase
    {
        [Theory]
        [InlineData(2, "v2")]
        [InlineData(12, "v2")]
        [InlineData(22, "v2")]
        [InlineData(102, "v2")]
        [InlineData(1, "v1")]
        [InlineData(3, "v1")]
        [InlineData(10, "v1")]
        [InlineData(15, "v1")]
        [InlineData(100, "v1")]
        public void GivenId_ShouldReturnCorrectVersionClient(int id, string expectedVersion)
        {
            // Arrange
            var expectedClient = expectedVersion == "v2" ? _clientV2 : _clientV1;

            // Act
            var result = _factory.GetClient(id);

            // Assert
            result.ShouldBe(expectedClient);
            result.Version.ShouldBe(expectedVersion);
        }

        [Theory]
        [InlineData(1, "v1")]
        [InlineData(2, "v2")]
        public void GivenClientNotRegistered_ShouldThrowInvalidOperationException(int id, string expectedVersion)
        {
            // Arrange
            var registeredClient = expectedVersion == "v1" ? _clientV2 : _clientV1;
            var clientsWithoutRequired = new List<IStarWarsClient> { registeredClient };
            var factory = new StarWarsClientFactory(clientsWithoutRequired);

            // Act & Assert
            var exception = Should.Throw<InvalidOperationException>(() => factory.GetClient(id));
            exception.Message.ShouldContain(expectedVersion);
            exception.Message.ShouldContain("not registered");
        }

        [Fact]
        public void GivenEmptyClientList_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var emptyClients = new List<IStarWarsClient>();
            var factory = new StarWarsClientFactory(emptyClients);

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => factory.GetClient(1));
        }
    }
}