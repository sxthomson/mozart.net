using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies;
using Mozart.Composition.AspNetCore.Mvc.Exceptions;
using Shouldly;
using Xunit;

namespace Mozart.Composition.AspNetCore.Mvc.UnitTests.Actions.ReturnTypes.Strategies
{
    public class ActionResultReturnTypeStrategyShould
    {
        private readonly ActionResultReturnTypeStrategy _actionResultReturnTypeStrategy;

        public ActionResultReturnTypeStrategyShould()
        {
            _actionResultReturnTypeStrategy = new ActionResultReturnTypeStrategy();
        }

        [Theory]
        [InlineData(typeof(ActionResult))]
        [InlineData(typeof(IActionResult))]
        public void MatchOnActionResultType(Type suppliedType)
        {
            _actionResultReturnTypeStrategy.Handles(suppliedType).ShouldBeTrue();
        }

        [Theory]
        [InlineData(typeof(Task<ActionResult>))]
        [InlineData(typeof(Task<IActionResult>))]
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
            _actionResultReturnTypeStrategy.Handles(type).ShouldBeFalse();
        }

        [Fact]
        public void ReturnTheTypeSpecifiedByProducesResponseTypeAttributeFor200OK()
        {
            var expectedType = typeof(string);
            var suppliedType = typeof(ActionResult);
            var attributes = new List<Attribute>{ new ProducesResponseTypeAttribute(expectedType, 200)};

            _actionResultReturnTypeStrategy.GetUnwrappedReturnType(suppliedType, attributes).ShouldBe(expectedType);
        }

        [Fact]       
        public void ThrowAMissingAttributeExceptionIfProducesResponseTypeAttributeFor200OKIsMissing()
        {
            var suppliedType = typeof(ActionResult);
            var attributes = new List<Attribute> {new ProducesResponseTypeAttribute(typeof(string), 400)};            
            
            Should.Throw<MissingExpectedAttributeException>(() =>_actionResultReturnTypeStrategy.GetUnwrappedReturnType(suppliedType, attributes));
        }

        [Fact]
        public void ThrowAMissingAttributeExceptionProducesResponseTypeIsMissing()
        {
            var suppliedType = typeof(ActionResult);
 
            Should.Throw<MissingExpectedAttributeException>(() => _actionResultReturnTypeStrategy.GetUnwrappedReturnType(suppliedType, new List<Attribute>()));
        }

        [Theory]
        [InlineData(typeof(Task<ActionResult>))]
        [InlineData(typeof(Task<IActionResult>))]
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
            Should.Throw<InvalidOperationException>(() => _actionResultReturnTypeStrategy.GetUnwrappedReturnType(type, new List<Attribute>()));
        }
    }
}

