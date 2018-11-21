using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Mozart.Composition.AspNetCore.Mvc.Actions;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Abstractions;
using Mozart.Composition.AspNetCore.Mvc.UnitTests.Fakes;
using Shouldly;
using Xunit;

namespace Mozart.Composition.AspNetCore.Mvc.UnitTests.Actions
{
    public class ControllerActionDescriptorReturnTypeProviderShould
    {
        private readonly ControllerActionDescriptorReturnTypeProvider _controllerActionDescriptorReturnTypeProvider;
        private readonly Mock<IActionDescriptorCollectionProvider> _actionDescriptorCollectionProviderMock;
        private readonly Mock<IActionReturnTypeResolver> _actionReturnTypeResolverMock;

        public ControllerActionDescriptorReturnTypeProviderShould()
        {
            _actionDescriptorCollectionProviderMock = new Mock<IActionDescriptorCollectionProvider>();
            _actionReturnTypeResolverMock = new Mock<IActionReturnTypeResolver>();

            // Have the IActionReturnTypeResolver return the input type as return type to simplify the tests here
            _actionReturnTypeResolverMock
                .Setup(x => x.ResolveUnwrappedReturnType(It.IsAny<Type>(), It.IsAny<IEnumerable<Attribute>>()))
                .Returns<Type, IEnumerable<Attribute>>((type, attributes) => type);

            _controllerActionDescriptorReturnTypeProvider = new ControllerActionDescriptorReturnTypeProvider(
                _actionDescriptorCollectionProviderMock.Object,
                _actionReturnTypeResolverMock.Object); 
        }

        [Fact]
        public void ThrowAnArgumentNullExceptionIfNullIActionDescriptorCollectionProviderIsPassedInConstructor()
        {
            Should.Throw<ArgumentNullException>(() =>
                new ControllerActionDescriptorReturnTypeProvider(null, _actionReturnTypeResolverMock.Object));
        }

        [Fact]
        public void ThrowAnArgumentNullExceptionIfNullIActionReturnTypeResolverIsPassedInConstructor()
        {
            Should.Throw<ArgumentNullException>(() =>
                new ControllerActionDescriptorReturnTypeProvider(_actionDescriptorCollectionProviderMock.Object, null));
        }

        [Theory]
        [InlineData("ActionWithNoAttributesReturnTaskString")]
        [InlineData("ActionWithHttpGetAttributeReturnTaskString")]
        [InlineData("ActionWithNoAttributesReturnTaskDateTime")]
        [InlineData("ActionWithHttpGetAttributeReturnTaskDateTime")]
        public void CallActionReturnTypeResolverWithReturnTypeAndAttributesOfTheSuppliedControllerActionDescriptorIfAnAlwaysTruePredicateIsSupplied(string methodName)
        {
            // Arrange
            var methodInfo = typeof(FakeController).GetMethod(methodName);
            var controllerActionDescriptor = new ControllerActionDescriptor
            {
                ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                MethodInfo = methodInfo
            };

            var expectedReturnType = controllerActionDescriptor.MethodInfo.ReturnType;
            var expectedAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes<Attribute>(true);

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(new List<ActionDescriptor> { controllerActionDescriptor }, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(x => true).ToList();

            // Assert
            _actionReturnTypeResolverMock.Verify(x => x.ResolveUnwrappedReturnType(expectedReturnType, It.Is<IEnumerable<Attribute>>(a => a.SequenceEqual(expectedAttributes))), Times.Once);
        }

        [Theory]
        [InlineData("ActionWithNoAttributesReturnTaskString")]
        [InlineData("ActionWithHttpGetAttributeReturnTaskString")]
        [InlineData("ActionWithNoAttributesReturnTaskDateTime")]
        [InlineData("ActionWithHttpGetAttributeReturnTaskDateTime")]
        public void ReturnACollectionWithASingleResultForASingleControllerActionDescriptorIfAnAlwaysTruePredicateIsSupplied(string methodName)
        {
            // Arrange
            var methodInfo = typeof(FakeController).GetMethod(methodName);
            var controllerActionDescriptor = new ControllerActionDescriptor
            {
                ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                MethodInfo = methodInfo
            };

            var expectedId = controllerActionDescriptor.Id; // Read-only, generated within it's constructor

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(new List<ActionDescriptor> { controllerActionDescriptor }, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(x => true).ToList();

            // Assert
            result.Count.ShouldBe(1);
        }

        [Theory]
        [InlineData("ActionWithNoAttributesReturnTaskString")]
        [InlineData("ActionWithHttpGetAttributeReturnTaskString")]
        [InlineData("ActionWithNoAttributesReturnTaskDateTime")]
        [InlineData("ActionWithHttpGetAttributeReturnTaskDateTime")]
        public void ReturnTheCorrectIdForASingleControllerActionDescriptorIfAnAlwaysTruePredicateIsSupplied(string methodName)
        {
            // Arrange
            var methodInfo = typeof(FakeController).GetMethod(methodName);
            var controllerActionDescriptor = new ControllerActionDescriptor
            {
                ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                MethodInfo = methodInfo
            };

            var expectedId = controllerActionDescriptor.Id; // Read-only, generated within it's constructor

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(new List<ActionDescriptor> {controllerActionDescriptor}, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(x => true).ToList();

            // Assert
            result.First().Id.ShouldBe(expectedId);
        }

        [Theory]
        [InlineData("ActionWithNoAttributesReturnTaskString")]
        [InlineData("ActionWithHttpGetAttributeReturnTaskString")]
        [InlineData("ActionWithNoAttributesReturnTaskDateTime")]
        [InlineData("ActionWithHttpGetAttributeReturnTaskDateTime")]
        public void ReturnTheCorrectReturnTypeForASingleControllerActionDescriptorIfAnAlwaysTruePredicateIsSupplied(string methodName)
        {
            // Arrange
            var methodInfo = typeof(FakeController).GetMethod(methodName);
            var controllerActionDescriptor = new ControllerActionDescriptor
            {
                ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                MethodInfo = methodInfo
            };

            var expectedReturnType = controllerActionDescriptor.MethodInfo.ReturnType; // Read-only, generated within it's constructor

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(new List<ActionDescriptor> { controllerActionDescriptor }, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(x => true).ToList();

            // Assert
            result.First().ReturnType.ShouldBe(expectedReturnType);
        }

        [Theory]
        [InlineData("ActionWithNoAttributesReturnTaskString")]
        [InlineData("ActionWithHttpGetAttributeReturnTaskString")]
        [InlineData("ActionWithNoAttributesReturnTaskDateTime")]
        [InlineData("ActionWithHttpGetAttributeReturnTaskDateTime")]
        public void NotCallActionReturnTypeResolverWithReturnTypeAndAttributesOfTheSuppliedControllerActionDescriptorIfAnAlwaysFalsePredicateIsSupplied(string methodName)
        {
            // Arrange
            var methodInfo = typeof(FakeController).GetMethod(methodName);
            var controllerActionDescriptor = new ControllerActionDescriptor
            {
                ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                MethodInfo = methodInfo
            };

            var expectedReturnType = controllerActionDescriptor.MethodInfo.ReturnType;
            var expectedAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes<Attribute>(true);

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(new List<ActionDescriptor> { controllerActionDescriptor }, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(x => false).ToList();

            // Assert
            _actionReturnTypeResolverMock.Verify(x => x.ResolveUnwrappedReturnType(expectedReturnType, It.Is<IEnumerable<Attribute>>(a => a.SequenceEqual(expectedAttributes))), Times.Never);
        }

        [Theory]
        [InlineData("ActionWithNoAttributesReturnTaskString")]
        [InlineData("ActionWithHttpGetAttributeReturnTaskString")]
        [InlineData("ActionWithNoAttributesReturnTaskDateTime")]
        [InlineData("ActionWithHttpGetAttributeReturnTaskDateTime")]
        public void ReturnAnEmptyCollectionIfAnAlwaysFalsePredicateIsSupplied(string methodName)
        {
            // Arrange
            var methodInfo = typeof(FakeController).GetMethod(methodName);
            var controllerActionDescriptor = new ControllerActionDescriptor
            {
                ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                MethodInfo = methodInfo
            };            

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(new List<ActionDescriptor> { controllerActionDescriptor }, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(x => false).ToList();

            // Assert
            result.Count.ShouldBe(0);            
        }

        [Fact]
        public void ReturnACollectionWithSameCountAsSuppliedControllerActionDescriptorsIfAnAlwaysTruePredicateIsSupplied()
        {
            // Arrange
            var methodInfos = typeof(FakeController).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var controllerActionDescriptors = new List<ControllerActionDescriptor>();
            foreach (var methodInfo in methodInfos)
            {
                var controllerActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                    MethodInfo = methodInfo
                };
                controllerActionDescriptors.Add(controllerActionDescriptor);
            }

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(controllerActionDescriptors, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(x => true).ToList();

            // Assert
            result.Count.ShouldBe(controllerActionDescriptors.Count);
        }

        [Fact]
        public void CallActionReturnTypeResolverWithReturnTypeAndAttributesOfTheSuppliedControllerActionDescriptorsIfAnAlwaysTruePredicateIsSupplied()
        {
            // Arrange
            var methodInfos = typeof(FakeController).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var controllerActionDescriptors = new List<ControllerActionDescriptor>();
            foreach (var methodInfo in methodInfos)
            {
                var controllerActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                    MethodInfo = methodInfo
                };
                controllerActionDescriptors.Add(controllerActionDescriptor);
            }

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(controllerActionDescriptors, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(x => true).ToList();

            // Assert
            foreach (var controllerActionDescriptor in controllerActionDescriptors)
            {
                var expectedReturnType = controllerActionDescriptor.MethodInfo.ReturnType;
                var expectedAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes(true);
                _actionReturnTypeResolverMock.Verify(x => x.ResolveUnwrappedReturnType(expectedReturnType, It.Is<IEnumerable<Attribute>>(a => a.SequenceEqual(expectedAttributes))), Times.Once);
            }
        }

        [Fact]
        public void ReturnAllCorrectIdsForACollectionOfControllerActionDescriptorsIfAnAlwaysTruePredicateIsSupplied()
        {
            // Arrange
            var methodInfos = typeof(FakeController).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var controllerActionDescriptors = new List<ControllerActionDescriptor>();
            foreach (var methodInfo in methodInfos)
            {
                var controllerActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                    MethodInfo = methodInfo
                };
                controllerActionDescriptors.Add(controllerActionDescriptor);
            }            

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(controllerActionDescriptors, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(x => true).ToList();

            // Assert
            for (var i = 0; i < controllerActionDescriptors.Count; i++)
            {                
                result[i].Id.ShouldBe(controllerActionDescriptors[i].Id);
            }
        }

        [Fact]
        public void ReturnTheCorrectReturnTypesForACollectionOfControllerActionDescriptorsIfAnAlwaysTruePredicateIsSupplied()
        {
            // Arrange
            var methodInfos = typeof(FakeController).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var controllerActionDescriptors = new List<ControllerActionDescriptor>();
            foreach (var methodInfo in methodInfos)
            {
                var controllerActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                    MethodInfo = methodInfo
                };
                controllerActionDescriptors.Add(controllerActionDescriptor);
            }

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(controllerActionDescriptors, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(x => true).ToList();

            // Assert
            for (var i = 0; i < controllerActionDescriptors.Count; i++)
            {
                result[i].ReturnType.ShouldBe(controllerActionDescriptors[i].MethodInfo.ReturnType);
            }
        }

        [Fact]
        public void ReturnAnEmptyCollectionForACollectionOfControllerActionDescriptorsIfAnAlwaysFalsePredicateIsSupplied()
        {
            // Arrange
            var methodInfos = typeof(FakeController).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var controllerActionDescriptors = new List<ControllerActionDescriptor>();
            foreach (var methodInfo in methodInfos)
            {
                var controllerActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                    MethodInfo = methodInfo
                };
                controllerActionDescriptors.Add(controllerActionDescriptor);
            }

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(controllerActionDescriptors, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(x => false).ToList();

            // Assert
            result.Count.ShouldBe(0);
        }

        [Fact]
        public void NotCallActionReturnTypeResolverWithReturnTypeAndAttributesOfTheSuppliedControllerActionDescriptorsIfAnAlwaysFalsePredicateIsSupplied()
        {
            // Arrange
            var methodInfos = typeof(FakeController).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var controllerActionDescriptors = new List<ControllerActionDescriptor>();
            foreach (var methodInfo in methodInfos)
            {
                var controllerActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                    MethodInfo = methodInfo
                };
                controllerActionDescriptors.Add(controllerActionDescriptor);
            }

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(controllerActionDescriptors, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(x => false).ToList();

            // Assert
            _actionReturnTypeResolverMock.Verify(x => x.ResolveUnwrappedReturnType(It.IsAny<Type>(), It.IsAny<IEnumerable<Attribute>>()), Times.Never);
        }

        [Fact]
        public void ReturnACollectionWithExpectedCountWithPredicateSatisfyingControllerActionDescriptorsIfAnAttributePredicateIsSupplied()
        {
            // Arrange
            var methodInfos = typeof(FakeController).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var controllerActionDescriptors = new List<ControllerActionDescriptor>();            

            foreach (var methodInfo in methodInfos)
            {
                var controllerActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                    MethodInfo = methodInfo
                };
                controllerActionDescriptors.Add(controllerActionDescriptor);
            }

            Func<ControllerActionDescriptor, bool> predicate = x =>
                x.MethodInfo.GetCustomAttributes<HttpGetAttribute>().Any();


            var expectedCalls = controllerActionDescriptors.Count(predicate);

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(controllerActionDescriptors, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(predicate).ToList();

            // Assert
            result.Count.ShouldBe(expectedCalls);
        }

        [Fact]
        public void CallActionReturnTypeResolverWithReturnTypeAndAttributesOfTheAttributePredicateSatisfyingControllerActionDescriptorsIfAnAttributePredicateIsSupplied()
        {
            // Arrange
            var methodInfos = typeof(FakeController).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var controllerActionDescriptors = new List<ControllerActionDescriptor>();

            foreach (var methodInfo in methodInfos)
            {
                var controllerActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                    MethodInfo = methodInfo
                };
                controllerActionDescriptors.Add(controllerActionDescriptor);
            }

            Func<ControllerActionDescriptor, bool> predicate = x =>
                x.MethodInfo.GetCustomAttributes<HttpGetAttribute>().Any();


            var expectedCalls = controllerActionDescriptors.Count(predicate);

            _actionDescriptorCollectionProviderMock.Setup(x => x.ActionDescriptors).Returns(
                new ActionDescriptorCollection(controllerActionDescriptors, 1));

            // Act
            var result = _controllerActionDescriptorReturnTypeProvider.ResolveAll(predicate).ToList();

            // Assert
            _actionReturnTypeResolverMock.Verify(x => x.ResolveUnwrappedReturnType(It.IsAny<Type>(), It.IsAny<IEnumerable<Attribute>>()), Times.Exactly(expectedCalls));
        }
    }
}
