using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies.Abstractions;
using Shouldly;
using Xunit;

namespace Mozart.Composition.AspNetCore.Mvc.UnitTests.Actions.ReturnTypes
{
    public class ActionReturnTypeResolverShould
    {
        private readonly ActionReturnTypeResolver _actionReturnTypeResolver;
        private readonly IList<Mock<IActionReturnTypeStrategy>> _actionReturnTypeStrategyMocks;
        private readonly Mock<IDefaultActionReturnTypeStrategy> _defaultActionReturnTypeStrategy;

        public ActionReturnTypeResolverShould()
        {
            _actionReturnTypeStrategyMocks = new List<Mock<IActionReturnTypeStrategy>>
            {
                new Mock<IActionReturnTypeStrategy>(),
                new Mock<IActionReturnTypeStrategy>()
            };
            _defaultActionReturnTypeStrategy = new Mock<IDefaultActionReturnTypeStrategy>();
            _actionReturnTypeResolver = new ActionReturnTypeResolver(
                _actionReturnTypeStrategyMocks.Select(x => x.Object), _defaultActionReturnTypeStrategy.Object);
        }

        [Fact]
        public void ThrowAnArgumentNullExceptionIfNullIActionReturnTypeStrategiesIsPassedInConstructor()
        {
            Should.Throw<ArgumentNullException>(() =>
                new ActionReturnTypeResolver(null, new DefaultActionResultReturnTypeStrategy()));
        }

        [Fact]
        public void ThrowAnArgumentNullExceptionIfNullIDefaultActionReturnTypeStrategyIsPassedInConstructor()
        {
            Should.Throw<ArgumentNullException>(() =>
                new ActionReturnTypeResolver(new List<IActionReturnTypeStrategy>(), null));
        }

        [Fact]
        public void CallHandlesOnceWithTheSuppliedType()
        {
            // Arrange
            var suppliedType = typeof(string);
            _actionReturnTypeStrategyMocks[0].Setup(x => x.Handles(suppliedType)).Returns(true);

            // Act
            _actionReturnTypeResolver.ResolveUnwrappedReturnType(suppliedType, new List<Attribute>());

            // Assert
            _actionReturnTypeStrategyMocks[0].Verify(x => x.Handles(suppliedType), Times.Once);
        }

        [Fact]
        public void NotCallTheSecondStrategyIfTheFirstStrategyCanHandleTheSuppliedType()
        {
            // Arrange
            var suppliedType = typeof(IActionResult);
            _actionReturnTypeStrategyMocks[0].Setup(x => x.Handles(suppliedType)).Returns(true);
            _actionReturnTypeStrategyMocks[1].Setup(x => x.Handles(suppliedType)).Returns(false);

            // Act
            _actionReturnTypeResolver.ResolveUnwrappedReturnType(suppliedType, new List<Attribute>());

            // Assert
            _actionReturnTypeStrategyMocks[1].Verify(x => x.Handles(suppliedType), Times.Never);
        }

        [Fact]
        public void CallsTheSecondStrategyHandlesMethodOnceIfTheFirstStrategyCannotHandleTheSuppliedType()
        {
            // Arrange
            var suppliedType = typeof(IActionResult);
            _actionReturnTypeStrategyMocks[0].Setup(x => x.Handles(suppliedType)).Returns(false);
            _actionReturnTypeStrategyMocks[1].Setup(x => x.Handles(suppliedType)).Returns(true);

            // Act
            _actionReturnTypeResolver.ResolveUnwrappedReturnType(suppliedType, new List<Attribute>());

            // Assert
            _actionReturnTypeStrategyMocks[1].Verify(x => x.Handles(suppliedType), Times.Once);
        }

        [Theory]
        [InlineData(true, false, 1, 0)]
        [InlineData(false, true, 0, 1)]
        [InlineData(true, true, 1, 0)]
        public void ResolveReturnTypeBasedOnTheFirstStrategyThatCanHandleSuppliedType(bool firstHandles,
            bool secondHandles, int firstResolveCalledTimes, int secondResolveCalledTimes)
        {
            // Arrange
            var suppliedType = typeof(IActionResult);
            var suppliedAttributes = new List<Attribute>();

            _actionReturnTypeStrategyMocks[0].Setup(x => x.Handles(suppliedType)).Returns(firstHandles);
            _actionReturnTypeStrategyMocks[1].Setup(x => x.Handles(suppliedType)).Returns(secondHandles);

            // Act
            _actionReturnTypeResolver.ResolveUnwrappedReturnType(suppliedType, suppliedAttributes);

            // Assert
            _actionReturnTypeStrategyMocks[0].Verify(x => x.GetUnwrappedReturnType(suppliedType, suppliedAttributes), Times.Exactly(firstResolveCalledTimes));
            _actionReturnTypeStrategyMocks[1].Verify(x => x.GetUnwrappedReturnType(suppliedType, suppliedAttributes), Times.Exactly(secondResolveCalledTimes));
        }

        [Theory]
        [InlineData(true, false, typeof(string))]
        [InlineData(false, true, typeof(int))]
        [InlineData(true, true, typeof(string))]
        public void ReturnTheResolvedTypeFromTheStrategyThatSatisfiesTheSuppliedType(bool firstHandles,
            bool secondHandles, Type expectedType)
        {
            // Arrange
            var suppliedType = typeof(IActionResult);
            var suppliedAttributes = new List<Attribute>();

            _actionReturnTypeStrategyMocks[0].Setup(x => x.Handles(suppliedType)).Returns(firstHandles);
            _actionReturnTypeStrategyMocks[1].Setup(x => x.Handles(suppliedType)).Returns(secondHandles);

            _actionReturnTypeStrategyMocks[0].Setup(x => x.GetUnwrappedReturnType(suppliedType, suppliedAttributes)).Returns(typeof(string));
            _actionReturnTypeStrategyMocks[1].Setup(x => x.GetUnwrappedReturnType(suppliedType, suppliedAttributes)).Returns(typeof(int));

            // Act
            var result = _actionReturnTypeResolver.ResolveUnwrappedReturnType(suppliedType, suppliedAttributes);

            // Assert
            result.ShouldBe(expectedType);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public void NotCallTheDefaultStrategyImplementationIfAnyStrategySatisfyTheSuppliedType(bool firstHandles, bool secondHandles)
        {
            // Arrange
            var suppliedType = typeof(IActionResult);
            var suppliedAttributes = new List<Attribute>();

            _actionReturnTypeStrategyMocks[0].Setup(x => x.Handles(suppliedType)).Returns(firstHandles);
            _actionReturnTypeStrategyMocks[1].Setup(x => x.Handles(suppliedType)).Returns(secondHandles);

            // Act
            _actionReturnTypeResolver.ResolveUnwrappedReturnType(suppliedType, suppliedAttributes);

            // Assert
            _defaultActionReturnTypeStrategy.Verify(x => x.GetUnwrappedReturnType(It.IsAny<Type>(), It.IsAny<IEnumerable<Attribute>>()), Times.Never);
        }

        [Fact]
        public void CallTheDefaultStrategyImplementationIfNoStrategiesSatisfyTheSuppliedType()
        {
            // Arrange
            var suppliedType = typeof(IActionResult);
            var suppliedAttributes = new List<Attribute>();

            _actionReturnTypeStrategyMocks[0].Setup(x => x.Handles(suppliedType)).Returns(false);
            _actionReturnTypeStrategyMocks[1].Setup(x => x.Handles(suppliedType)).Returns(false);

            // Act
            _actionReturnTypeResolver.ResolveUnwrappedReturnType(suppliedType, suppliedAttributes);

            // Assert
            _defaultActionReturnTypeStrategy.Verify(x => x.GetUnwrappedReturnType(suppliedType, suppliedAttributes), Times.Once);
        }

        [Fact]
        public void ReturnTheResolvedTypeFromTheDefaultStrategyIfNoStrategiesSatisfyTheSuppliedType()
        {
            // Arrange
            var suppliedType = typeof(IActionResult);
            var suppliedAttributes = new List<Attribute>();
            var expectedType = typeof(DateTime);

            _actionReturnTypeStrategyMocks[0].Setup(x => x.Handles(suppliedType)).Returns(false);
            _actionReturnTypeStrategyMocks[1].Setup(x => x.Handles(suppliedType)).Returns(false);

            _defaultActionReturnTypeStrategy.Setup(x => x.GetUnwrappedReturnType(suppliedType, suppliedAttributes))
                .Returns(expectedType);

            // Act
            var result =_actionReturnTypeResolver.ResolveUnwrappedReturnType(suppliedType, suppliedAttributes);

            // Assert
            result.ShouldBe(expectedType);
        }
    }
}
