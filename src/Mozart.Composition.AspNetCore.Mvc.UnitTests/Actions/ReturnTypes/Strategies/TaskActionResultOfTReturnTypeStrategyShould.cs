using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies;
using Shouldly;
using Xunit;

namespace Composition.Demo.ViewModelComposition.WebApi.UnitTests.Actions.ReturnTypes.Strategies
{
    public class TaskActionResultOfTReturnTypeStrategyShould
    {
        private readonly TaskActionResultOfTReturnTypeStrategy _taskActionResultReturnTypeStrategy;

        public TaskActionResultOfTReturnTypeStrategyShould()
        {
            _taskActionResultReturnTypeStrategy = new TaskActionResultOfTReturnTypeStrategy();
        }

        [Theory]
        [InlineData(typeof(Task<ActionResult<int>>))]
        [InlineData(typeof(Task<ActionResult<string>>))]
        public void MatchOnActionResultOfTTypeWrappedInTask(Type suppliedType)
        {
            _taskActionResultReturnTypeStrategy.Handles(suppliedType).ShouldBeTrue();
        }

        [Theory]
        [InlineData(typeof(Task<ActionResult>))]
        [InlineData(typeof(Task<IActionResult>))]
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

        [Theory]
        [InlineData(typeof(Task<ActionResult<string>>), typeof(string))]
        [InlineData(typeof(Task<ActionResult<int>>), typeof(int))]
        public void ReturnTheGenericTypeArgumentForAnActionResultOfT(Type suppliedType, Type expectedType)
        {
            var attributes = new List<Attribute>();
            _taskActionResultReturnTypeStrategy.GetUnwrappedReturnType(suppliedType, attributes).ShouldBe(expectedType);
        }

        [Theory]
        [InlineData(typeof(Task<ActionResult>))]
        [InlineData(typeof(Task<IActionResult>))]
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