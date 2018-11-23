using System;
using Moq;
using Mozart.Composition.AspNetCore.Mvc.Actions.Filters;
using Mozart.Composition.AspNetCore.Mvc.Results.Abstractions;
using Mozart.Composition.Core.Abstractions;
using Shouldly;
using Xunit;

namespace Mozart.Composition.AspNetCore.Mvc.UnitTests.Actions.Filters
{
    public class MozartComposeModelAttributeShould
    {
        private readonly MozartComposeModelAttribute _mozartComposeModelAttribute;

        public MozartComposeModelAttributeShould()
        {
            _mozartComposeModelAttribute = new MozartComposeModelAttribute();
        }

        [Fact]
        public void ThrowAnArgumentNullExceptionIfNullServiceProviderIsPassedWhenCreatingViaMvcPipeline()
        {            
            Should.Throw<ArgumentNullException>(() =>
                _mozartComposeModelAttribute.CreateInstance(null));
        }

        [Fact]
        public void ThrowAnArgumentNullExceptionIfICachedServiceResolverIsNotRegisteredInServiceProviderWhenCreatingViaMvcPipeline()
        {
            // Arrange
            var mockServiceProvider = new Mock<IServiceProvider>();

            // Act / Assert
            Should.Throw<ArgumentNullException>(() =>
                _mozartComposeModelAttribute.CreateInstance(mockServiceProvider.Object));
        }

        [Fact]
        public void UseServiceCollectionToResolveTheCachedServiceResolverWhenCreatingViaMvcPipeline()
        {
            // Arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            var cachedServiceResolverMock = new Mock<ICachedServiceResolver<string, IHandleResult>>();
            mockServiceProvider.Setup(x => x.GetService(typeof(ICachedServiceResolver<string, IHandleResult>)))
                .Returns(cachedServiceResolverMock.Object);

            // Act
            _mozartComposeModelAttribute.CreateInstance(mockServiceProvider.Object);

            // Assert
            mockServiceProvider.Verify(x => x.GetService(typeof(ICachedServiceResolver<string, IHandleResult>)), Times.Once);
        }

        [Fact]
        public void ReturnAnInstanceOfCompositionResultActionFilterWhenCreatingViaMvcPipeline()
        {
            // Arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            var cachedServiceResolverMock = new Mock<ICachedServiceResolver<string, IHandleResult>>();
            mockServiceProvider.Setup(x => x.GetService(typeof(ICachedServiceResolver<string, IHandleResult>)))
                .Returns(cachedServiceResolverMock.Object);

            // Act
            var result = _mozartComposeModelAttribute.CreateInstance(mockServiceProvider.Object);

            // Assert
            result.ShouldBeOfType<CompositionResultActionFilter>();
        }

        [Fact]
        public void BeReusable()
        {
            _mozartComposeModelAttribute.IsReusable.ShouldBeTrue();
        }
    }
}
