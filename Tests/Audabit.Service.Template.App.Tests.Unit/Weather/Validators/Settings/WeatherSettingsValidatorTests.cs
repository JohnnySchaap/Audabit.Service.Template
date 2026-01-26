using Audabit.Service.Template.App.Tests.Unit.TestHelpers;
using Audabit.Service.Template.App.Weather.Settings;
using Audabit.Service.Template.App.Weather.Validators.Settings;
using FluentValidation.TestHelper;

namespace Audabit.Service.Template.App.Tests.Unit.Weather.Validators.Settings;

public class WeatherSettingsValidatorTests
{
    public class WeatherSettingsValidatorTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly WeatherSettingsValidator _validator;

        public WeatherSettingsValidatorTestsBase()
        {
            _fixture = FixtureFactory.Create();
            _validator = new WeatherSettingsValidator();
        }
    }

    public class MinTemperature : WeatherSettingsValidatorTestsBase
    {
        [Fact]
        public void GivenValueLessThanMinus100_ShouldHaveValidationError()
        {
            // Arrange
            var settings = _fixture.Build<WeatherSettings>()
                .With(x => x.MinTemperature, -101)
                .Create();

            // Act
            var result = _validator.TestValidate(settings);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MinTemperature)
                .WithErrorMessage("WeatherSettings:MinTemperature must be at least -100°C");
        }

        [Fact]
        public void GivenValueGreaterThanOrEqualToMaxTemperature_ShouldHaveValidationError()
        {
            // Arrange
            var settings = _fixture.Build<WeatherSettings>()
                .With(x => x.MinTemperature, 50)
                .With(x => x.MaxTemperature, 50)
                .Create();

            // Act
            var result = _validator.TestValidate(settings);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MinTemperature)
                .WithErrorMessage("WeatherSettings:MinTemperature must be less than MaxTemperature");
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(-50)]
        [InlineData(0)]
        [InlineData(49)]
        public void GivenValidValue_ShouldNotHaveValidationError(int minTemp)
        {
            // Arrange
            var settings = _fixture.Build<WeatherSettings>()
                .With(x => x.MinTemperature, minTemp)
                .With(x => x.MaxTemperature, minTemp + _fixture.Create<int>())
                .Create();

            // Act
            var result = _validator.TestValidate(settings);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.MinTemperature);
        }
    }

    public class MaxTemperature : WeatherSettingsValidatorTestsBase
    {
        [Fact]
        public void GivenValueGreaterThan100_ShouldHaveValidationError()
        {
            // Arrange
            var settings = _fixture.Build<WeatherSettings>()
                .With(x => x.MaxTemperature, 101)
                .Create();

            // Act
            var result = _validator.TestValidate(settings);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MaxTemperature)
                .WithErrorMessage("WeatherSettings:MaxTemperature must not exceed 100°C");
        }

        [Fact]
        public void GivenValueLessThanOrEqualToMinTemperature_ShouldHaveValidationError()
        {
            // Arrange
            var settings = _fixture.Build<WeatherSettings>()
                .With(x => x.MinTemperature, 50)
                .With(x => x.MaxTemperature, 50)
                .Create();

            // Act
            var result = _validator.TestValidate(settings);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MaxTemperature)
                .WithErrorMessage("WeatherSettings:MaxTemperature must be greater than MinTemperature");
        }

        [Theory]
        [InlineData(-19)]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(100)]
        public void GivenValidValue_ShouldNotHaveValidationError(int maxTemp)
        {
            // Arrange
            var settings = _fixture.Build<WeatherSettings>()
                .With(x => x.MinTemperature, maxTemp - _fixture.Create<int>())
                .With(x => x.MaxTemperature, maxTemp)
                .Create();

            // Act
            var result = _validator.TestValidate(settings);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.MaxTemperature);
        }
    }

    public class ForecastDays : WeatherSettingsValidatorTestsBase
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void GivenValueLessThanOrEqualToZero_ShouldHaveValidationError(int days)
        {
            // Arrange
            var settings = _fixture.Build<WeatherSettings>()
                .With(x => x.ForecastDays, days)
                .Create();

            // Act
            var result = _validator.TestValidate(settings);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ForecastDays)
                .WithErrorMessage("WeatherSettings:ForecastDays must be greater than 0");
        }

        [Fact]
        public void GivenValueGreaterThan30_ShouldHaveValidationError()
        {
            // Arrange
            var settings = _fixture.Build<WeatherSettings>()
                .With(x => x.ForecastDays, 31)
                .Create();

            // Act
            var result = _validator.TestValidate(settings);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ForecastDays)
                .WithErrorMessage("WeatherSettings:ForecastDays must not exceed 30 days");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(15)]
        [InlineData(30)]
        public void GivenValidValue_ShouldNotHaveValidationError(int days)
        {
            // Arrange
            var settings = _fixture.Build<WeatherSettings>()
                .With(x => x.ForecastDays, days)
                .Create();

            // Act
            var result = _validator.TestValidate(settings);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.ForecastDays);
        }
    }

    public class ValidSettings : WeatherSettingsValidatorTestsBase
    {
        [Fact]
        public void GivenAllValidProperties_ShouldNotHaveAnyValidationErrors()
        {
            // Arrange
            var minTemp = Random.Shared.Next(-100, 50);
            var maxTemp = Random.Shared.Next(minTemp + 1, 100);
            var forecastDays = Random.Shared.Next(1, 30);
            var settings = _fixture.Build<WeatherSettings>()
                .With(x => x.MinTemperature, minTemp)
                .With(x => x.MaxTemperature, maxTemp)
                .With(x => x.ForecastDays, forecastDays)
                .Create();

            // Act
            var result = _validator.TestValidate(settings);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}