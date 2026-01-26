using Audabit.Service.Template.Api.StarWars.v1.Transport;
using Audabit.Service.Template.App.Models.StarWars;
using Audabit.Service.Template.App.StarWars.Services;
using Audabit.Service.Template.WebApi.StarWars.Controllers.v1;
using Audabit.Service.Template.WebApi.StarWars.Mappers;
using Audabit.Service.Template.WebApi.Tests.Unit.TestHelpers;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.Template.WebApi.Tests.Unit.StarWars.Controllers.v1;

public class StarWarsControllerTests
{
    public class StarWarsControllerTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly IStarWarsService _starWarsService;
        protected readonly StarWarsMapper _mapper;
        protected readonly StarWarsController _controller;

        public StarWarsControllerTestsBase()
        {
            _fixture = FixtureFactory.Create();

            _starWarsService = Substitute.For<IStarWarsService>();
            _mapper = new StarWarsMapper();
            _controller = new StarWarsController(_starWarsService, _mapper);
        }
    }

    public class GetPerson : StarWarsControllerTestsBase
    {
        [Theory, AutoData]
        public async Task GivenPersonExists_ShouldReturnOkWithPerson(int id)
        {
            // Arrange
            var person = _fixture.Create<StarWarsPerson>();
            _starWarsService.GetPersonAsync(id, Arg.Any<CancellationToken>()).Returns(person);

            // Act
            var result = await _controller.GetPerson(id, CancellationToken.None);

            // Assert
            await _starWarsService.Received(1).GetPersonAsync(id, Arg.Any<CancellationToken>());

            result.ShouldBeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.ShouldBeOfType<GetStarWarsPersonResponse>();

            var response = (GetStarWarsPersonResponse)okResult.Value!;
            response.Person.ShouldNotBeNull();
            response.Person.Name.ShouldBe(person.Name);
            response.Person.Height.ShouldBe(person.Height);
        }

        [Theory, AutoData]
        public async Task GivenPersonDoesNotExist_ShouldReturnNotFound(int id)
        {
            // Arrange
            _starWarsService.GetPersonAsync(id, Arg.Any<CancellationToken>()).Returns((StarWarsPerson?)null);

            // Act
            var result = await _controller.GetPerson(id, CancellationToken.None);

            // Assert
            await _starWarsService.Received(1).GetPersonAsync(id, Arg.Any<CancellationToken>());

            result.ShouldBeOfType<NotFoundResult>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task GivenInvalidId_ShouldStillCallService(int id)
        {
            // Arrange
            _starWarsService.GetPersonAsync(id, Arg.Any<CancellationToken>()).Returns((StarWarsPerson?)null);

            // Act
            var result = await _controller.GetPerson(id, CancellationToken.None);

            // Assert
            await _starWarsService.Received(1).GetPersonAsync(id, Arg.Any<CancellationToken>());
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Theory, AutoData]
        public async Task GivenCancellationRequested_ShouldPassCancellationToken(int id)
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var person = _fixture.Create<StarWarsPerson>();
            _starWarsService.GetPersonAsync(id, cancellationToken).Returns(person);

            // Act
            var result = await _controller.GetPerson(id, cancellationToken);

            // Assert
            await _starWarsService.Received(1).GetPersonAsync(id, cancellationToken);
            result.ShouldBeOfType<OkObjectResult>();
        }
    }
}