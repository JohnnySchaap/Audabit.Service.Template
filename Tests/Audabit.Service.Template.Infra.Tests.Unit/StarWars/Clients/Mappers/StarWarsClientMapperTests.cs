using Audabit.Service.Template.Infra.StarWars.Clients.Mappers;
using Audabit.Service.Template.Infra.StarWars.Clients.Models;
using Audabit.Service.Template.Infra.Tests.Unit.TestHelpers;

namespace Audabit.Service.Template.Infra.Tests.Unit.StarWars.Clients.Mappers;

public class StarWarsClientMapperTests
{
    public class StarWarsClientMapperTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly StarWarsClientMapper _mapper;

        public StarWarsClientMapperTestsBase()
        {
            _fixture = FixtureFactory.Create();
            _mapper = new StarWarsClientMapper();
        }
    }

    public class MapToDomain : StarWarsClientMapperTestsBase
    {
        [Fact]
        public void GivenValidClientModel_ShouldMapAllProperties()
        {
            // Arrange
            var clientModel = _fixture.Create<StarWarsPersonClientModel>();

            // Act
            var result = _mapper.MapToDomain(clientModel);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe(clientModel.Name);
            result.Height.ShouldBe(clientModel.Height);
        }

        [Theory, AutoData]
        public void GivenMultipleClientModels_ShouldMapEachCorrectly(string name1, string height1, string name2, string height2)
        {
            // Arrange
            var clientModel1 = new StarWarsPersonClientModel(name1, height1);
            var clientModel2 = new StarWarsPersonClientModel(name2, height2);

            // Act
            var result1 = _mapper.MapToDomain(clientModel1);
            var result2 = _mapper.MapToDomain(clientModel2);

            // Assert
            result1.Name.ShouldBe(name1);
            result1.Height.ShouldBe(height1);
            result2.Name.ShouldBe(name2);
            result2.Height.ShouldBe(height2);
        }

        [Theory, AutoData]
        public void GivenClientModelWithSpecialCharacters_ShouldMapCorrectly(string name)
        {
            // Arrange
            var clientModel = new StarWarsPersonClientModel(name, "172");

            // Act
            var result = _mapper.MapToDomain(clientModel);

            // Assert
            result.Name.ShouldBe(name);
            result.Height.ShouldBe("172");
        }
    }
}