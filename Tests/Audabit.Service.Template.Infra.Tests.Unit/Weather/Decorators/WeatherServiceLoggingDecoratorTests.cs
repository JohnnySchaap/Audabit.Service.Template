using Audabit.Common.Observability.Emitters;
using Audabit.Common.Observability.Events;
using Audabit.Service.Template.App.Models.Weather;
using Audabit.Service.Template.App.Weather.Services;
using Audabit.Service.Template.Infra.Weather.Decorators;

namespace Audabit.Service.Template.Infra.Tests.Unit.Weather.Decorators;

public class WeatherServiceLoggingDecoratorTests
{
    public class WeatherServiceLoggingDecoratorTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly IWeatherService _innerService;
        protected readonly IEmitter<WeatherServiceLoggingDecorator> _emitter;
        protected readonly WeatherServiceLoggingDecorator _decorator;

        public WeatherServiceLoggingDecoratorTestsBase()
        {
            _fixture = new Fixture();
            _innerService = Substitute.For<IWeatherService>();
            _emitter = Substitute.For<IEmitter<WeatherServiceLoggingDecorator>>();
            _decorator = new WeatherServiceLoggingDecorator(_innerService, _emitter);
        }
    }

    public class GetForecastAsync : WeatherServiceLoggingDecoratorTestsBase
    {
        [Theory, AutoData]
        public async Task GivenValidCountryCode_ShouldCallInnerServiceAndRaiseTelemetry(string countryCode)
        {
            // Arrange
            var forecasts = new[]
            {
                new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 20, "Sunny", "Info1", new SecretWeatherForecastObject("Data1", "Secret1"), new SecretWeatherForecastObject("Data2", "Secret2")),
                new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 22, "Cloudy", "Info2", new SecretWeatherForecastObject("Data3", "Secret3"), new SecretWeatherForecastObject("Data4", "Secret4"))
            };
            _innerService.GetForecastAsync(countryCode).Returns(forecasts);

            // Act
            var result = await _decorator.GetForecastAsync(countryCode);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(forecasts);

            await _innerService.Received(1).GetForecastAsync(countryCode);
            _emitter.Received(2).Raise(Arg.Any<IEvent>(), Arg.Any<Microsoft.Extensions.Logging.LogLevel>(), Arg.Any<Dictionary<string, object>?>());
        }

        [Theory, AutoData]
        public async Task GivenInnerServiceThrowsException_ShouldPropagateExceptionAndRaiseGeneratingEvent(string countryCode)
        {
            // Arrange
            var exception = new InvalidOperationException("Service error");
            _innerService.GetForecastAsync(countryCode)
                .Returns(Task.FromException<WeatherForecast[]>(exception));

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(async () =>
                await _decorator.GetForecastAsync(countryCode));

            _emitter.Received(1).Raise(Arg.Any<IEvent>(), Arg.Any<Microsoft.Extensions.Logging.LogLevel>(), Arg.Any<Dictionary<string, object>?>());
        }

        [Theory, AutoData]
        public async Task GivenEmptyForecastArray_ShouldCallInnerServiceAndRaiseTelemetry(string countryCode)
        {
            // Arrange
            var forecasts = Array.Empty<WeatherForecast>();
            _innerService.GetForecastAsync(countryCode).Returns(forecasts);

            // Act
            var result = await _decorator.GetForecastAsync(countryCode);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();

            await _innerService.Received(1).GetForecastAsync(countryCode);
            _emitter.Received(2).Raise(Arg.Any<IEvent>(), Arg.Any<Microsoft.Extensions.Logging.LogLevel>(), Arg.Any<Dictionary<string, object>?>());
        }
    }
}