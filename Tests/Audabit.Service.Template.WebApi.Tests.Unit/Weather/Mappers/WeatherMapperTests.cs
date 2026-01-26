using Audabit.Service.Template.App.Models.Weather;
using Audabit.Service.Template.WebApi.Tests.Unit.TestHelpers;
using Audabit.Service.Template.WebApi.Weather.Mappers;

namespace Audabit.Service.Template.WebApi.Tests.Unit.Weather.Mappers;

public class WeatherMapperTests
{
    public class WeatherMapperTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly WeatherMapper _mapper;

        public WeatherMapperTestsBase()
        {
            _fixture = FixtureFactory.Create();
            _mapper = new WeatherMapper();
        }
    }

    public class ToDto_Single : WeatherMapperTestsBase
    {
        [Fact]
        public void GivenValidForecast_ShouldMapAllProperties()
        {
            // Arrange
            var forecast = _fixture.Create<WeatherForecast>();

            // Act
            var result = _mapper.ToDto(forecast);

            // Assert
            result.ShouldNotBeNull();
            result.Date.ShouldBe(forecast.Date);
            result.TemperatureC.ShouldBe(forecast.TemperatureC);
            result.Summary.ShouldBe(forecast.Summary);
        }
    }

    public class ToDto_Collection : WeatherMapperTestsBase
    {
        [Fact]
        public void GivenEmptyCollection_ShouldReturnEmptyCollection()
        {
            // Arrange
            var forecasts = Enumerable.Empty<WeatherForecast>();

            // Act
            var result = _mapper.ToDto(forecasts);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Theory, AutoData]
        public void GivenCollectionWithItems_ShouldMapAllItems(int count)
        {
            // Arrange
            var forecasts = _fixture.CreateMany<WeatherForecast>(count).ToArray();

            // Act
            var result = _mapper.ToDto(forecasts).ToArray();

            // Assert
            result.ShouldNotBeNull();
            result.Length.ShouldBe(count);

            for (var i = 0; i < forecasts.Length; i++)
            {
                result[i].Date.ShouldBe(forecasts[i].Date);
                result[i].TemperatureC.ShouldBe(forecasts[i].TemperatureC);
                result[i].Summary.ShouldBe(forecasts[i].Summary);
            }
        }
    }
}