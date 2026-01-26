using System.Net;
using System.Net.Http.Json;
using Audabit.Service.Template.Api.StarWars.v1.Transport;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Audabit.Service.Template.Tests.Integration.StarWars.Controllers.v1;

[Collection(nameof(WebApplicationFactoryCollection))]
public class StarWarsControllerIntegrationTests
{

    public class StarWarsControllerIntegrationTestsBase(WebApplicationFactory<Program> factory)
    {
        protected readonly Fixture _fixture = new();
        protected readonly HttpClient _client = factory.CreateClient();
    }

    [Collection(nameof(WebApplicationFactoryCollection))]
    public class GetPerson(WebApplicationFactory<Program> factory)
        : StarWarsControllerIntegrationTestsBase(factory)
    {
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(10)]
        public async Task GivenValidIdForV1_ShouldReturnPerson(int id)
        {
            // Act
            var response = await _client.GetAsync($"/v1/StarWars/Person/{id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetStarWarsPersonResponse>();
            result.ShouldNotBeNull();
            result.Person.ShouldNotBeNull();
            result.Person.Name.ShouldNotBeNullOrEmpty();
            result.Person.Height.ShouldNotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(12)]
        [InlineData(22)]
        public async Task GivenValidIdForV2_ShouldReturnPerson(int id)
        {
            // Act
            var response = await _client.GetAsync($"/v1/StarWars/Person/{id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetStarWarsPersonResponse>();
            result.ShouldNotBeNull();
            result.Person.ShouldNotBeNull();
            result.Person.Name.ShouldNotBeNullOrEmpty();
            result.Person.Height.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task GivenNonExistentId_ShouldReturn404()
        {
            // Arrange
            var nonExistentId = 99999;

            // Act
            var response = await _client.GetAsync($"/v1/StarWars/Person/{nonExistentId}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GivenInvalidId_ShouldReturn404OrBadRequest(int id)
        {
            // Act
            var response = await _client.GetAsync($"/v1/StarWars/Person/{id}");

            // Assert
            response.StatusCode.ShouldBeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
        }

        [Theory, AutoData]
        public async Task GivenCorrelationId_ShouldReturnSameCorrelationId(string correlationId)
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/v1/StarWars/Person/1");
            request.Headers.Add("X-Correlation-Id", correlationId);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.Headers.TryGetValues("X-Correlation-Id", out var values);
            values.ShouldNotBeNull();
            values.ShouldContain(correlationId);
        }

        [Fact]
        public async Task GivenApiVersionInHeader_ShouldWork()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/v1/StarWars/Person/1");
            request.Headers.Add("api-version", "1.0");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}