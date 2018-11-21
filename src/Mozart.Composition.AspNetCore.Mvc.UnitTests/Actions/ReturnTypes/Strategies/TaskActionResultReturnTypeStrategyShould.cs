using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies;
using Mozart.Composition.AspNetCore.Mvc.Exceptions;
using Shouldly;
using Xunit;

namespace Composition.Demo.ViewModelComposition.WebApi.UnitTests.Actions.ReturnTypes.Strategies
{
    public class TaskActionResultReturnTypeStrategyShould
    {
        private readonly TaskActionResultReturnTypeStrategy _taskActionResultReturnTypeStrategy;

        public TaskActionResultReturnTypeStrategyShould()
        {
            _taskActionResultReturnTypeStrategy = new TaskActionResultReturnTypeStrategy();
        }

        [Theory]        
        [InlineData(typeof(Task<ActionResult>))]
        [InlineData(typeof(Task<IActionResult>))]
        public void MatchOnActionResultTypeWrappedInTask(Type suppliedType)
        {
            _taskActionResultReturnTypeStrategy.Handles(suppliedType).ShouldBeTrue();
        }

        [Fact]
        public void MatchOnIActionResultTypeWrappedInTask()
        {
            _taskActionResultReturnTypeStrategy.Handles(typeof(Task<IActionResult>)).ShouldBeTrue();
        }

        [Theory]
        [InlineData(typeof(Task<ActionResult<string>>))]
        [InlineData(typeof(Task<ActionResult<int>>))]
        [InlineData(typeof(ActionResult<string>))]
        [InlineData(typeof(ActionResult<int>))]        
        [InlineData(typeof(ViewResult))]
        [InlineData(typeof(ObjectResult))]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        
        public void NotMatchOnOtherTypes(Type type)
        {
            _taskActionResultReturnTypeStrategy.Handles(type).ShouldBeFalse();
        }

        [Fact]
        public void ReturnTheTypeSpecifiedByProducesResponseTypeAttributeFor200OK()
        {
            var expectedType = typeof(string);
            var suppliedType = typeof(Task<ActionResult>);
            var attributes = new List<Attribute>{ new ProducesResponseTypeAttribute(expectedType, 200)};

            _taskActionResultReturnTypeStrategy.GetUnwrappedReturnType(suppliedType, attributes).ShouldBe(expectedType);
        }

        [Fact]       
        public void ThrowAMissingAttributeExceptionIfProducesResponseTypeAttributeFor200OKIsMissing()
        {
            var suppliedType = typeof(Task<ActionResult>);
            var attributes = new List<Attribute> {new ProducesResponseTypeAttribute(typeof(string), 400)};            
            
            Should.Throw<MissingExpectedAttributeException>(() =>_taskActionResultReturnTypeStrategy.GetUnwrappedReturnType(suppliedType, attributes));
        }

        [Fact]
        public void ThrowAMissingAttributeExceptionProducesResponseTypeIsMissing()
        {
            var suppliedType = typeof(Task<ActionResult>);

            Should.Throw<MissingExpectedAttributeException>(() => _taskActionResultReturnTypeStrategy.GetUnwrappedReturnType(suppliedType, new List<Attribute>()));
        }

        [Theory]
        [InlineData(typeof(Task<ActionResult<string>>))]
        [InlineData(typeof(Task<ActionResult<int>>))]
        [InlineData(typeof(ActionResult<string>))]
        [InlineData(typeof(ActionResult<int>))]
        [InlineData(typeof(ViewResult))]
        [InlineData(typeof(ObjectResult))]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        public void ThrowAnInvalidOperationExceptionIfAnIncompatibleTypeIsPassedToGetUnwrappedReturnType(Type type)
        {
            Should.Throw<InvalidOperationException>(() => _taskActionResultReturnTypeStrategy.GetUnwrappedReturnType(type, new List<Attribute>()));
        }
    }
}

