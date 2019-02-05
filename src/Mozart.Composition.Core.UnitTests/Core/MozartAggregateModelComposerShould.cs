using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Mozart.Composition.Core.Abstractions;
using Mozart.Composition.Core.UnitTests.Fakes;
using Shouldly;
using Xunit;

namespace Mozart.Composition.Core.UnitTests.Core
{
    public class MozartAggregateModelComposerShould
    {
        private readonly Mock<IComposeModel<FakeDomainClass1>> _mockFirstComposer;
        private readonly Mock<IComposeModel<FakeDomainClass2>> _mockSecondComposer;

        public MozartAggregateModelComposerShould()
        {
            _mockFirstComposer = new Mock<IComposeModel<FakeDomainClass1>>();
            _mockSecondComposer = new Mock<IComposeModel<FakeDomainClass2>>();
        }

        [Fact]
        public async Task CallComposeAsyncOnEachIComposeModelThatIsResolved()
        {
            // Arrange
            var suppliedDictionary = new Dictionary<string, object>();

            _mockFirstComposer.Setup(x => x.ComposeAsync(suppliedDictionary))
                .ReturnsAsync(new FakeDomainClass1());

            _mockSecondComposer.Setup(x => x.ComposeAsync(suppliedDictionary))
                .ReturnsAsync(new FakeDomainClass2());

            var mozartAggregateModelComposer = GetMozartAggregateModelComposer(new List<IComposeModel>
                {_mockFirstComposer.Object, _mockSecondComposer.Object});

            // Act
            await mozartAggregateModelComposer.BuildCompositeModelAsync(suppliedDictionary);

            // Assert
            _mockFirstComposer.Verify(x => x.ComposeAsync(suppliedDictionary), Times.Once);
            _mockSecondComposer.Verify(x => x.ComposeAsync(suppliedDictionary), Times.Once);
        }

        [Fact]
        public async Task NotCallComposeAsyncOnAnyIComposeModelThatIsNotRequiredForCompositeModel()
        {
            // Arrange
            var suppliedDictionary = new Dictionary<string, object>();

            _mockFirstComposer.Setup(x => x.ComposeAsync(suppliedDictionary))
                .ReturnsAsync(new FakeDomainClass1());

            _mockSecondComposer.Setup(x => x.ComposeAsync(suppliedDictionary))
                .ReturnsAsync(new FakeDomainClass2());

            var mockUnusedComposer = new Mock<IComposeModel<string>>();

            var mozartAggregateModelComposer = GetMozartAggregateModelComposer(new List<IComposeModel>
                {_mockFirstComposer.Object, _mockSecondComposer.Object, mockUnusedComposer.Object});

            // Act
            await mozartAggregateModelComposer.BuildCompositeModelAsync(suppliedDictionary);

            // Assert
            mockUnusedComposer.Verify(x => x.ComposeAsync(suppliedDictionary), Times.Never);
        }

        [Fact]
        public async Task ReturnAFullyConstructedCompositeModelWhenAllPropertyIComposeModelsCanBeResolved()
        {
            // Arrange
            var suppliedDictionary = new Dictionary<string, object>();
            var expectedFirstDomainObject = new FakeDomainClass1();
            var expectedSecondDomainObject = new FakeDomainClass2();

            _mockFirstComposer.Setup(x => x.ComposeAsync(suppliedDictionary))
                .ReturnsAsync(expectedFirstDomainObject);

            _mockSecondComposer.Setup(x => x.ComposeAsync(suppliedDictionary))
                .ReturnsAsync(expectedSecondDomainObject);

            var mozartAggregateModelComposer = GetMozartAggregateModelComposer(new List<IComposeModel>
                {_mockFirstComposer.Object, _mockSecondComposer.Object});

            // Act
            var compositeModel = await mozartAggregateModelComposer.BuildCompositeModelAsync(suppliedDictionary);

            // Assert
            compositeModel.ShouldNotBeNull();
            compositeModel.Domain1.ShouldBe(expectedFirstDomainObject);
            compositeModel.Domain2.ShouldBe(expectedSecondDomainObject);
        }

        [Fact]
        public async Task ReturnAPartiallyConstructedCompositeModelWhenSomePropertyIComposeModelsCanBeResolved()
        {
            // Arrange
            var suppliedDictionary = new Dictionary<string, object>();
            var expectedFirstDomainObject = new FakeDomainClass1();

            _mockFirstComposer.Setup(x => x.ComposeAsync(suppliedDictionary))
                .ReturnsAsync(expectedFirstDomainObject);

            var mozartAggregateModelComposer = GetMozartAggregateModelComposer(new List<IComposeModel>
                {_mockFirstComposer.Object});

            // Act
            var compositeModel = await mozartAggregateModelComposer.BuildCompositeModelAsync(suppliedDictionary);

            // Assert
            compositeModel.ShouldNotBeNull();
            compositeModel.Domain1.ShouldBe(expectedFirstDomainObject);
            compositeModel.Domain2.ShouldBeNull();
        }

        [Fact]
        public async Task ReturnDefaultIfNoComposeModelsCanBeResolved()
        {
            // Arrange
            var suppliedDictionary = new Dictionary<string, object>();

            var mozartAggregateModelComposer = GetMozartAggregateModelComposer(new List<IComposeModel>());

            // Act
            var compositeModel = await mozartAggregateModelComposer.BuildCompositeModelAsync(suppliedDictionary);

            // Assert
            compositeModel.ShouldBeNull();
        }

        private MozartAggregateModelComposer<FakeCompositeModel> GetMozartAggregateModelComposer(IEnumerable<IComposeModel> composeModels)
        {
            var modelComposerResolver = new ComposeModelResolver(composeModels);

            return new MozartAggregateModelComposer<FakeCompositeModel>(modelComposerResolver);
        }
    }
}
