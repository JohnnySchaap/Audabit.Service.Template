using Audabit.Service.Template.App.Models.StarWars;
using Audabit.Service.Template.WebApi.StarWars.Mappers;
using Audabit.Service.Template.WebApi.Tests.Unit.TestHelpers;

namespace Audabit.Service.Template.WebApi.Tests.Unit.StarWars.Mappers;

public class StarWarsMapperTests
{
    public class StarWarsMapperTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly StarWarsMapper _mapper;

        public StarWarsMapperTestsBase()
        {
            _fixture = FixtureFactory.Create();
            _mapper = new StarWarsMapper();
        }
    }

    public class MapToDto : StarWarsMapperTestsBase
    {
        [Fact]
        public void GivenValidPerson_ShouldMapAllProperties()
        {
            // Arrange
            var person = _fixture.Create<StarWarsPerson>();

            // Act
            var result = _mapper.MapToDto(person);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe(person.Name);
            result.Height.ShouldBe(person.Height);
        }

        [Theory, AutoData]
        public void GivenMultiplePeople_ShouldMapEachCorrectly(string name1, string height1, string name2, string height2)
        {
            // Arrange
            var person1 = new StarWarsPerson(name1, height1);
            var person2 = new StarWarsPerson(name2, height2);

            // Act
            var result1 = _mapper.MapToDto(person1);
            var result2 = _mapper.MapToDto(person2);

            // Assert
            result1.Name.ShouldBe(name1);
            result1.Height.ShouldBe(height1);
            result2.Name.ShouldBe(name2);
            result2.Height.ShouldBe(height2);
        }
    }
}