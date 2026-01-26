using System.Net;
using System.Text.Json;
using Audabit.Common.Observability.Emitters;
using Audabit.Service.Template.Infra.StarWars.Clients;
using Audabit.Service.Template.Infra.StarWars.Clients.Mappers;
using Audabit.Service.Template.Infra.StarWars.Clients.Models;
using Audabit.Service.Template.Infra.Tests.Unit.TestHelpers;

namespace Audabit.Service.Template.Infra.Tests.Unit.StarWars.Clients;

public class StarWarsClientV2Tests
{
    public class StarWarsClientV2TestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly IEmitter<StarWarsClient> _emitter;
        protected readonly StarWarsClientMapper _mapper;
        protected readonly HttpMessageHandlerStub _messageHandler;
        protected readonly HttpClient _httpClient;
        protected readonly StarWarsClientV2 _starWarsClient;

        public StarWarsClientV2TestsBase()
        {
            _fixture = FixtureFactory.Create();
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _emitter = Substitute.For<IEmitter<StarWarsClient>>();
            _mapper = new StarWarsClientMapper();
            _messageHandler = new HttpMessageHandlerStub();
            _httpClient = new HttpClient(_messageHandler) { BaseAddress = new Uri("https://swapi.info/api/") };

            _httpClientFactory.CreateClient(nameof(StarWarsClientV2)).Returns(_httpClient);
            _starWarsClient = new StarWarsClientV2(_httpClientFactory, _emitter, _mapper);
        }
    }

    public class GetPersonAsync : StarWarsClientV2TestsBase
    {
        [Theory, AutoData]
        public async Task GivenPersonExists_ShouldReturnMappedPerson(int id, string name, string height)
        {
            // Arrange
            var clientModel = new StarWarsPersonClientModel(name, height);
            var jsonContent = JsonSerializer.Serialize(clientModel);
            _messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent)
            };

            // Act
            var result = await _starWarsClient.GetPersonAsync(id, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe(name);
            result.Height.ShouldBe(height);
        }

        [Theory, AutoData]
        public async Task GivenPersonNotFound_ShouldReturnNull(int id)
        {
            // Arrange
            _messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound);

            // Act
            var result = await _starWarsClient.GetPersonAsync(id, CancellationToken.None);

            // Assert
            result.ShouldBeNull();
        }

        [Theory, AutoData]
        public async Task GivenInternalServerError_ShouldThrowAndRaiseError(int id)
        {
            // Arrange
            _messageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            // Act & Assert
            await Should.ThrowAsync<HttpRequestException>(async () =>
                await _starWarsClient.GetPersonAsync(id, CancellationToken.None));
        }

        [Theory, AutoData]
        public async Task GivenBadRequest_ShouldThrowAndRaiseError(int id)
        {
            // Arrange
            _messageHandler.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            // Act & Assert
            await Should.ThrowAsync<HttpRequestException>(async () =>
                await _starWarsClient.GetPersonAsync(id, CancellationToken.None));
        }

        [Theory, AutoData]
        public async Task GivenNetworkException_ShouldThrowAndRaiseError(int id)
        {
            // Arrange
            _messageHandler.Exception = new HttpRequestException("Network error");

            // Act & Assert
            await Should.ThrowAsync<HttpRequestException>(async () =>
                await _starWarsClient.GetPersonAsync(id, CancellationToken.None));
        }

        [Theory, AutoData]
        public async Task GivenInvalidJson_ShouldThrow(int id)
        {
            // Arrange
            _messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("invalid json")
            };

            // Act & Assert
            await Should.ThrowAsync<JsonException>(async () =>
                await _starWarsClient.GetPersonAsync(id, CancellationToken.None));
        }

        [Theory, AutoData]
        public async Task GivenEmptyResponse_ShouldThrow(int id)
        {
            // Arrange
            _messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Empty)
            };

            // Act & Assert
            await Should.ThrowAsync<JsonException>(async () =>
                await _starWarsClient.GetPersonAsync(id, CancellationToken.None));
        }

        [Theory, AutoData]
        public async Task GivenValidRequest_ShouldCallCorrectEndpoint(int id)
        {
            // Arrange
            var clientModel = _fixture.Create<StarWarsPersonClientModel>();
            var jsonContent = JsonSerializer.Serialize(clientModel);
            _messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent)
            };

            // Act
            await _starWarsClient.GetPersonAsync(id, CancellationToken.None);

            // Assert
            _messageHandler.Request.ShouldNotBeNull();
            _messageHandler.Request.RequestUri.ShouldNotBeNull();
            _messageHandler.Request.RequestUri.ToString().ShouldContain($"people/{id}/");
        }

        [Fact]
        public void Version_ShouldBeV2()
        {
            // Assert
            _starWarsClient.Version.ShouldBe("v2");
        }
    }

    // Helper class to stub HttpMessageHandler
    public class HttpMessageHandlerStub : HttpMessageHandler
    {
        public HttpResponseMessage? Response { get; set; }
        public Exception? Exception { get; set; }
        public HttpRequestMessage? Request { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;

            if (Exception != null)
            {
                throw Exception;
            }

            return Task.FromResult(Response ?? new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}