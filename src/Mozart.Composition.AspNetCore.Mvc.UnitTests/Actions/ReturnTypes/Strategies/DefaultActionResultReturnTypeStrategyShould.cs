using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies;
using Shouldly;
using Xunit;

namespace Mozart.Composition.AspNetCore.Mvc.UnitTests.Actions.ReturnTypes.Strategies
{
    public class DefaultActionResultReturnTypeStrategyShould
    {
        private readonly DefaultActionResultReturnTypeStrategy _actionResultReturnTypeStrategy;

        public DefaultActionResultReturnTypeStrategyShould()
        {
            _actionResultReturnTypeStrategy = new DefaultActionResultReturnTypeStrategy();
        }
        
        [Theory]
        [InlineData(typeof(Task<ActionResult>), typeof(ActionResult))]
        [InlineData(typeof(Task<IActionResult>), typeof(IActionResult))]
        [InlineData(typeof(Task<int>), typeof(int))]
        [InlineData(typeof(Task<DateTime>), typeof(DateTime))]
        [InlineData(typeof(Task<string>), typeof(string))]
        [InlineData(typeof(Task<object>), typeof(object))]
        public void ReturnTheGenericTypeArgumentForAnyTaskWrappedType(Type suppliedType, Type expectedType)
        {            
            _actionResultReturnTypeStrategy.GetUnwrappedReturnType(suppliedType, new List<Attribute>()).ShouldBe(expectedType);
        }

        [Theory]
        [InlineData(typeof(ActionResult), typeof(ActionResult))]
        [InlineData(typeof(IActionResult), typeof(IActionResult))]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(DateTime), typeof(DateTime))]
        [InlineData(typeof(string), typeof(string))]
        [InlineData(typeof(object), typeof(object))]
        public void ReturnTheSuppliedTypeIfItsNotWrappedInATask(Type suppliedType, Type expectedType)
        {
            _actionResultReturnTypeStrategy.GetUnwrappedReturnType(suppliedType, new List<Attribute>()).ShouldBe(expectedType);
        }
    }
}

