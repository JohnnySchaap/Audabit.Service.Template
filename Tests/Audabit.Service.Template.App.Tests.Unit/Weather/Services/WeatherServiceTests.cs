using Audabit.Service.Template.App.Weather.Services;
using Audabit.Service.Template.App.Weather.Settings;
using Microsoft.Extensions.Options;

namespace Audabit.Service.Template.App.Tests.Unit.Weather.Services;

public class WeatherServiceTests
{
    public class WeatherServiceTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly IOptions<WeatherSettings> _settings;
        protected readonly WeatherService _weatherService;

        public WeatherServiceTestsBase()
        {
            _fixture = new Fixture();
            _fixture.Customize<WeatherSettings>(composer => composer
                .With(s => s.MinTemperature, -_fixture.Create<int>())
                .With(s => s.MaxTemperature, _fixture.Create<int>())
                .With(s => s.ForecastDays, _fixture.Create<int>()));

            _settings = Substitute.For<IOptions<WeatherSettings>>();

            _weatherService = new WeatherService(_settings);
        }
    }

    public class GetForecastAsync : WeatherServiceTestsBase
    {
        [Fact]
        public async Task GivenNullCountryCode_ShouldThrowArgumentNullException()
        {
            // Arrange
            var settings = _fixture.Create<WeatherSettings>();
            _settings.Value.Returns(settings);
            string? countryCode = null;

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(async () =>
                await _weatherService.GetForecastAsync(countryCode!));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public async Task GivenEmptyOrWhitespaceCountryCode_ShouldThrowArgumentNullException(string countryCode)
        {
            // Arrange
            var settings = _fixture.Create<WeatherSettings>();
            _settings.Value.Returns(settings);

            // Act & Assert
            await Should.ThrowAsync<ArgumentException>(async () =>
                await _weatherService.GetForecastAsync(countryCode));
        }

        [Theory, AutoData]
        public async Task GivenValidCountryCode_ShouldReturnForecasts(string countryCode)
        {
            // Arrange
            var settings = _fixture.Create<WeatherSettings>();
            _settings.Value.Returns(settings);

            // Act
            var result = await _weatherService.GetForecastAsync(countryCode);

            // Assert
            result.ShouldNotBeNull();
            result.Length.ShouldBe(settings.ForecastDays);
        }
    }

}