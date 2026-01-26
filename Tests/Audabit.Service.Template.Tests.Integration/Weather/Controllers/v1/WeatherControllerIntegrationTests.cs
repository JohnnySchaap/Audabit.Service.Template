using System.Net;
using System.Net.Http.Json;
using Audabit.Service.Template.Api.Weather.v1.Transport;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Audabit.Service.Template.Tests.Integration.Weather.Controllers.v1;

[Collection(nameof(WebApplicationFactoryCollection))]
public class WeatherControllerIntegrationTests
{

    public class WeatherControllerIntegrationTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly HttpClient _client;

        public WeatherControllerIntegrationTestsBase(WebApplicationFactory<Program> factory)
        {
            _fixture = new Fixture();
            _fixture.Customize<PutWeatherForecastRequest>(composer => composer
                .With(x => x.SomeRandomParameter, "ValidParameter"));

            _client = factory.CreateClient();
        }
    }

    [Collection(nameof(WebApplicationFactoryCollection))]
    public class GetWeatherForecastAsync(WebApplicationFactory<Program> factory)
        : WeatherControllerIntegrationTestsBase(factory)
    {
        [Theory]
        [InlineData("US")]
        [InlineData("NL")]
        [InlineData("GB")]
        public async Task GivenValidCountryCode_ShouldReturnForecasts(string countryCode)
        {
            // Act
            var response = await _client.GetAsync($"/v1/Weather/forecast/{countryCode}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetWeatherForecastResponse>();
            result.ShouldNotBeNull();
            result.Forecasts.ShouldNotBeNull();
            result.Forecasts.ShouldNotBeEmpty();
            result.Forecasts.Count().ShouldBe(5);
        }

        [Theory]
        [InlineData("X")]    // Too short
        [InlineData("USA")]  // Too long
        [InlineData("ABC")]  // Too long
        public async Task GivenInvalidCountryCodeLength_ShouldReturn404(string countryCode)
        {
            // Act
            var response = await _client.GetAsync($"/v1/Weather/forecast/{countryCode}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Theory, AutoData]
        public async Task GivenCorrelationId_ShouldReturnSameCorrelationId(string correlationId)
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/v1/Weather/forecast/US");
            request.Headers.Add("X-Correlation-Id", correlationId);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.Headers.TryGetValues("X-Correlation-Id", out var values);
            values.ShouldNotBeNull();
            values.ShouldContain(correlationId);
        }
    }

    [Collection(nameof(WebApplicationFactoryCollection))]
    public class PutWeatherForecastAsync(WebApplicationFactory<Program> factory)
        : WeatherControllerIntegrationTestsBase(factory)
    {
        [Fact]
        public async Task GivenValidRequest_ShouldReturnOk()
        {
            // Arrange
            var request = _fixture.Create<PutWeatherForecastRequest>();

            // Act
            var response = await _client.PutAsJsonAsync("/v1/Weather/forecast/US", request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<PutWeatherForecastResponse>();
            result.ShouldNotBeNull();
        }

        [Theory]
        [InlineData("X")]    // Too short
        [InlineData("USA")]  // Too long
        public async Task GivenInvalidCountryCodeLength_ShouldReturn404(string countryCode)
        {
            // Arrange
            var request = _fixture.Create<PutWeatherForecastRequest>();

            // Act
            var response = await _client.PutAsJsonAsync($"/v1/Weather/forecast/{countryCode}", request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenNullRequest_ShouldReturn400()
        {
            // Act
            var response = await _client.PutAsJsonAsync("/v1/Weather/forecast/US", (PutWeatherForecastRequest?)null);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Theory, AutoData]
        public async Task GivenCorrelationId_ShouldReturnSameCorrelationId(PutWeatherForecastRequest weatherRequest, string correlationId)
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Put, "/v1/Weather/forecast/US")
            {
                Content = JsonContent.Create(weatherRequest)
            };
            request.Headers.Add("X-Correlation-Id", correlationId);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.Headers.TryGetValues("X-Correlation-Id", out var values);
            values.ShouldNotBeNull();
            values.ShouldContain(correlationId);
        }
    }
}