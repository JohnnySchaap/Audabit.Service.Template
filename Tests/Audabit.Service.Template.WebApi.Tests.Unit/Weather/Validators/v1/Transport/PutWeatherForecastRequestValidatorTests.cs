using Audabit.Service.Template.Api.Weather.v1.Transport;
using Audabit.Service.Template.WebApi.Tests.Unit.TestHelpers;
using Audabit.Service.Template.WebApi.Weather.Validators.v1.Transport;
using FluentValidation.TestHelper;

namespace Audabit.Service.Template.WebApi.Tests.Unit.Weather.Validators.v1.Transport;

public class PutWeatherForecastRequestValidatorTests
{
    public class PutWeatherForecastRequestValidatorTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly PutWeatherForecastRequestValidator _validator;

        public PutWeatherForecastRequestValidatorTestsBase()
        {
            _fixture = FixtureFactory.Create();
            _validator = new PutWeatherForecastRequestValidator();
        }
    }

    public class SomeRandomParameter : PutWeatherForecastRequestValidatorTestsBase
    {
        [Fact]
        public void GivenNullValue_ShouldHaveValidationError()
        {
            // Arrange
            var request = _fixture.Build<PutWeatherForecastRequest>()
                .With(x => x.SomeRandomParameter, () => null!)
                .Create();

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SomeRandomParameter)
                .WithErrorMessage("SomeRandomParameter is required");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void GivenEmptyOrWhitespace_ShouldHaveValidationError(string value)
        {
            // Arrange
            var request = _fixture.Build<PutWeatherForecastRequest>()
                .With(x => x.SomeRandomParameter, value)
                .Create();

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SomeRandomParameter)
                .WithErrorMessage("SomeRandomParameter is required");
        }

        [Fact]
        public void GivenValueExceeding50Characters_ShouldHaveValidationError()
        {
            // Arrange
            var request = _fixture.Build<PutWeatherForecastRequest>()
                .With(x => x.SomeRandomParameter, new string(_fixture.Create<char>(), 51))
                .Create();

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SomeRandomParameter)
                .WithErrorMessage("SomeRandomParameter must not exceed 50 characters");
        }

        [Theory, AutoData]
        public void GivenValidValue_ShouldNotHaveValidationError(string value)
        {
            // Arrange
            var validValue = value.Length > 50 ? value[..50] : value;
            var request = _fixture.Build<PutWeatherForecastRequest>()
                .With(x => x.SomeRandomParameter, validValue)
                .Create();

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.SomeRandomParameter);
        }
    }
}