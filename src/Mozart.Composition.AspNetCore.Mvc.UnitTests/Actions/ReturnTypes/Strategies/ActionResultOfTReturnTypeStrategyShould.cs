using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies;
using Shouldly;
using Xunit;

namespace Mozart.Composition.AspNetCore.Mvc.UnitTests.Actions.ReturnTypes.Strategies
{
    public class ActionResultOfTReturnTypeStrategyShould
    {
        private readonly ActionResultOfTReturnTypeStrategy _actionResultReturnTypeStrategy;

        public ActionResultOfTReturnTypeStrategyShould()
        {
            _actionResultReturnTypeStrategy = new ActionResultOfTReturnTypeStrategy();
        }
        
        [Theory]
        [InlineData(typeof(ActionResult<string>))]
        [InlineData(typeof(ActionResult<int>))]
        public void MatchOnActionResultOfTType(Type suppliedType)
        {
            _actionResultReturnTypeStrategy.Handles(suppliedType).ShouldBeTrue();
        }

        [Theory]
        [InlineData(typeof(Task<ActionResult>))]
        [InlineData(typeof(Task<IActionResult>))]
        [InlineData(typeof(Task<ActionResult<string>>))]
        [InlineData(typeof(Task<ActionResult<int>>))]
        [InlineData(typeof(ActionResult))]
        [InlineData(typeof(IActionResult))]
        [InlineData(typeof(ViewResult))]
        [InlineData(typeof(ObjectResult))]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        public void NotMatchOnOtherTypes(Type type)
        {
            _actionResultReturnTypeStrategy.Handles(type).ShouldBeFalse();
        }

        [Theory]
        [InlineData(typeof(ActionResult<string>), typeof(string))]
        [InlineData(typeof(ActionResult<int>), typeof(int))]
        public void ReturnTheGenericTypeArgumentForAnActionResultOfT(Type suppliedType, Type expectedType)
        {            
            _actionResultReturnTypeStrategy.GetUnwrappedReturnType(suppliedType, new List<Attribute>()).ShouldBe(expectedType);
        }

        [Theory]
        [InlineData(typeof(Task<ActionResult>))]
        [InlineData(typeof(Task<IActionResult>))]
        [InlineData(typeof(Task<ActionResult<string>>))]
        [InlineData(typeof(Task<ActionResult<int>>))]
        [InlineData(typeof(ActionResult))]
        [InlineData(typeof(IActionResult))]
        [InlineData(typeof(ViewResult))]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        public void ThrowAnInvalidOperationExceptionIfAnIncompatibleTypeIsPassedToGetUnwrappedReturnType(Type type)
        {        
            Should.Throw<InvalidOperationException>(() =>_actionResultReturnTypeStrategy.GetUnwrappedReturnType(type, new List<Attribute>()));
        }
    }
}

