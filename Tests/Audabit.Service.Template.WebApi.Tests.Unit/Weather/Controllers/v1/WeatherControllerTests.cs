using Audabit.Service.Template.Api.Weather.v1.Transport;
using Audabit.Service.Template.App.Models.Weather;
using Audabit.Service.Template.App.Weather.Services;
using Audabit.Service.Template.WebApi.Tests.Unit.TestHelpers;
using Audabit.Service.Template.WebApi.Weather.Controllers.v1;
using Audabit.Service.Template.WebApi.Weather.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.Template.WebApi.Tests.Unit.Weather.Controllers.v1;

public class WeatherControllerTests
{
    public class WeatherControllerTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly IWeatherService _weatherService;
        protected readonly WeatherMapper _mapper;
        protected readonly WeatherController _controller;

        public WeatherControllerTestsBase()
        {
            _fixture = FixtureFactory.Create();

            _weatherService = Substitute.For<IWeatherService>();
            _mapper = new WeatherMapper();
            _controller = new WeatherController(_weatherService, _mapper);
        }
    }

    public class GetWeatherForecastAsync : WeatherControllerTestsBase
    {
        [Fact]
        public async Task GivenNullCountryCode_ShouldThrowArgumentNullException()
        {
            // Arrange
            string? countryCode = null;

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(async () =>
                await _controller.GetWeatherForecastAsync(countryCode!));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public async Task GivenEmptyOrWhitespaceCountryCode_ShouldThrowArgumentNullException(string countryCode)
        {
            // Arrange

            // Act & Assert
            await Should.ThrowAsync<ArgumentException>(async () =>
                await _controller.GetWeatherForecastAsync(countryCode));
        }

        [Theory, AutoData]
        public async Task GivenValidCountryCode_ShouldReturnOkWithForecasts(string countryCode)
        {
            // Arrange
            var forecasts = _fixture.CreateMany<WeatherForecast>(5).ToArray();
            _weatherService.GetForecastAsync(countryCode).Returns(forecasts);

            // Act
            var result = await _controller.GetWeatherForecastAsync(countryCode);

            // Assert
            await _weatherService.Received(1).GetForecastAsync(countryCode);

            result.ShouldBeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.ShouldBeOfType<GetWeatherForecastResponse>();

            var response = (GetWeatherForecastResponse)okResult.Value!;
            response.Forecasts.Count().ShouldBe(5);
        }
    }

    public class PutWeatherForecastAsync : WeatherControllerTestsBase
    {
        [Fact]
        public async Task GivenNullCountryCode_ShouldThrowArgumentNullException()
        {
            // Arrange
            string? countryCode = null;
            var request = _fixture.Create<PutWeatherForecastRequest>();

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(async () =>
                await _controller.PutWeatherForecastAsync(countryCode!, request));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public async Task GivenEmptyOrWhitespaceCountryCode_ShouldThrowArgumentNullException(string countryCode)
        {
            // Arrange
            var request = _fixture.Create<PutWeatherForecastRequest>();

            // Act & Assert
            await Should.ThrowAsync<ArgumentException>(async () =>
                await _controller.PutWeatherForecastAsync(countryCode, request));
        }

        [Theory, AutoData]
        public async Task GivenNullRequest_ShouldThrowArgumentNullException(string countryCode)
        {
            // Arrange
            PutWeatherForecastRequest? request = null;

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(async () =>
                await _controller.PutWeatherForecastAsync(countryCode, request!));
        }

        [Theory, AutoData]
        public async Task GivenValidInput_ShouldReturnOkWithResponse(string countryCode)
        {
            // Arrange
            var request = _fixture.Create<PutWeatherForecastRequest>();

            // Act
            var result = await _controller.PutWeatherForecastAsync(countryCode, request);

            // Assert
            result.ShouldBeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.ShouldBeOfType<PutWeatherForecastResponse>();
        }
    }
}