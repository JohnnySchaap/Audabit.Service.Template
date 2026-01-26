using Audabit.Common.Observability.Emitters;
using Audabit.Common.Observability.Events;
using Audabit.Service.Template.App.Models.StarWars;
using Audabit.Service.Template.App.StarWars.Clients;
using Audabit.Service.Template.Infra.StarWars.Decorators;
using Audabit.Service.Template.Infra.Tests.Unit.TestHelpers;

namespace Audabit.Service.Template.Infra.Tests.Unit.StarWars.Decorators;

public class StarWarsClientLoggingDecoratorTests
{
    public class StarWarsClientLoggingDecoratorTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly IStarWarsClient _innerClient;
        protected readonly IEmitter<StarWarsClientLoggingDecorator> _emitter;
        protected readonly StarWarsClientLoggingDecorator _decorator;

        public StarWarsClientLoggingDecoratorTestsBase()
        {
            _fixture = FixtureFactory.Create();
            _innerClient = Substitute.For<IStarWarsClient>();
            _innerClient.Version.Returns("v1");
            _emitter = Substitute.For<IEmitter<StarWarsClientLoggingDecorator>>();
            _decorator = new StarWarsClientLoggingDecorator(_innerClient, _emitter);
        }
    }

    public class Version : StarWarsClientLoggingDecoratorTestsBase
    {
        [Fact]
        public void ShouldReturnInnerClientVersion()
        {
            // Act
            var result = _decorator.Version;

            // Assert
            result.ShouldBe("v1");
        }
    }

    public class GetPersonAsync : StarWarsClientLoggingDecoratorTestsBase
    {
        [Theory, AutoData]
        public async Task GivenPersonExists_ShouldCallInnerClientWithoutRaisingError(int id)
        {
            // Arrange
            var person = _fixture.Create<StarWarsPerson>();
            _innerClient.GetPersonAsync(id, Arg.Any<CancellationToken>()).Returns(person);

            // Act
            var result = await _decorator.GetPersonAsync(id, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(person);

            await _innerClient.Received(1).GetPersonAsync(id, Arg.Any<CancellationToken>());
            _emitter.DidNotReceive().Raise(Arg.Any<IEvent>(), Arg.Any<Microsoft.Extensions.Logging.LogLevel>(), Arg.Any<Dictionary<string, object>?>());
        }

        [Theory, AutoData]
        public async Task GivenPersonDoesNotExist_ShouldCallInnerClientWithoutRaisingError(int id)
        {
            // Arrange
            _innerClient.GetPersonAsync(id, Arg.Any<CancellationToken>()).Returns((StarWarsPerson?)null);

            // Act
            var result = await _decorator.GetPersonAsync(id, CancellationToken.None);

            // Assert
            result.ShouldBeNull();

            await _innerClient.Received(1).GetPersonAsync(id, Arg.Any<CancellationToken>());
            _emitter.DidNotReceive().Raise(Arg.Any<IEvent>(), Arg.Any<Microsoft.Extensions.Logging.LogLevel>(), Arg.Any<Dictionary<string, object>?>());
        }

        [Theory, AutoData]
        public async Task GivenInnerClientThrowsException_ShouldRaiseErrorAndPropagateException(int id)
        {
            // Arrange
            var exception = new HttpRequestException("Network error");
            _innerClient.GetPersonAsync(id, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<StarWarsPerson?>(exception));

            // Act & Assert
            await Should.ThrowAsync<HttpRequestException>(async () =>
                await _decorator.GetPersonAsync(id, CancellationToken.None));

            _emitter.Received(1).Raise(Arg.Any<IEvent>(), Arg.Any<Microsoft.Extensions.Logging.LogLevel>(), Arg.Any<Dictionary<string, object>?>());
        }

        [Theory, AutoData]
        public async Task GivenCancellationRequested_ShouldPassCancellationToken(int id)
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var person = _fixture.Create<StarWarsPerson>();
            _innerClient.GetPersonAsync(id, cancellationToken).Returns(person);

            // Act
            var result = await _decorator.GetPersonAsync(id, cancellationToken);

            // Assert
            await _innerClient.Received(1).GetPersonAsync(id, cancellationToken);
            result.ShouldBe(person);
        }
    }
}