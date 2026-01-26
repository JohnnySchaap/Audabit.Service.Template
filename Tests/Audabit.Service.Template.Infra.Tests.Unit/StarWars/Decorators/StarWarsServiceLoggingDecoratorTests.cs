using Audabit.Common.Observability.Emitters;
using Audabit.Common.Observability.Events;
using Audabit.Service.Template.App.Models.StarWars;
using Audabit.Service.Template.App.StarWars.Services;
using Audabit.Service.Template.Infra.StarWars.Decorators;
using Audabit.Service.Template.Infra.Tests.Unit.TestHelpers;

namespace Audabit.Service.Template.Infra.Tests.Unit.StarWars.Decorators;

public class StarWarsServiceLoggingDecoratorTests
{
    public class StarWarsServiceLoggingDecoratorTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly IStarWarsService _innerService;
        protected readonly IEmitter<StarWarsServiceLoggingDecorator> _emitter;
        protected readonly StarWarsServiceLoggingDecorator _decorator;

        public StarWarsServiceLoggingDecoratorTestsBase()
        {
            _fixture = FixtureFactory.Create();
            _innerService = Substitute.For<IStarWarsService>();
            _emitter = Substitute.For<IEmitter<StarWarsServiceLoggingDecorator>>();
            _decorator = new StarWarsServiceLoggingDecorator(_innerService, _emitter);
        }
    }

    public class GetPersonAsync : StarWarsServiceLoggingDecoratorTestsBase
    {
        [Theory, AutoData]
        public async Task GivenPersonExists_ShouldCallInnerServiceAndRaiseTelemetry(int id)
        {
            // Arrange
            var person = _fixture.Create<StarWarsPerson>();
            _innerService.GetPersonAsync(id, Arg.Any<CancellationToken>()).Returns(person);

            // Act
            var result = await _decorator.GetPersonAsync(id, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(person);

            await _innerService.Received(1).GetPersonAsync(id, Arg.Any<CancellationToken>());
            _emitter.Received(2).Raise(Arg.Any<IEvent>(), Arg.Any<Microsoft.Extensions.Logging.LogLevel>(), Arg.Any<Dictionary<string, object>?>());
        }

        [Theory, AutoData]
        public async Task GivenPersonDoesNotExist_ShouldCallInnerServiceAndRaiseTelemetry(int id)
        {
            // Arrange
            _innerService.GetPersonAsync(id, Arg.Any<CancellationToken>()).Returns((StarWarsPerson?)null);

            // Act
            var result = await _decorator.GetPersonAsync(id, CancellationToken.None);

            // Assert
            result.ShouldBeNull();

            await _innerService.Received(1).GetPersonAsync(id, Arg.Any<CancellationToken>());
            _emitter.Received(2).Raise(Arg.Any<IEvent>(), Arg.Any<Microsoft.Extensions.Logging.LogLevel>(), Arg.Any<Dictionary<string, object>?>());
        }

        [Theory, AutoData]
        public async Task GivenInnerServiceThrowsException_ShouldPropagateExceptionAndRaiseRetrievingEvent(int id)
        {
            // Arrange
            var exception = new HttpRequestException("Network error");
            _innerService.GetPersonAsync(id, Arg.Any<CancellationToken>())
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
            _innerService.GetPersonAsync(id, cancellationToken).Returns(person);

            // Act
            var result = await _decorator.GetPersonAsync(id, cancellationToken);

            // Assert
            await _innerService.Received(1).GetPersonAsync(id, cancellationToken);
            result.ShouldBe(person);
        }
    }
}