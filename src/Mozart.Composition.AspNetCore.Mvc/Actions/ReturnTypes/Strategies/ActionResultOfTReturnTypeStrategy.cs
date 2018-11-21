using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies.Abstractions;
using Mozart.Composition.Core;
using Mozart.Composition.Core.Extensions;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies
{
    public class ActionResultOfTReturnTypeStrategy : IActionReturnTypeStrategy
    {
        public virtual bool Handles(Type returnType)
        {
            return HandlesType(returnType);
        }

        public virtual Type GetUnwrappedReturnType(Type returnType, IEnumerable<Attribute> attributes)
        {
            if (!HandlesType(returnType))
            {
                throw new InvalidOperationException(
                    $"This class does not handle this return type.  The return type is not of generic type {nameof(ActionResult)}.");
            }
            return returnType.GenericTypeArguments[0];
        }

        // Separate method to allow for parent's unwrapping of types and passing through via overrides
        private static bool HandlesType(Type returnType)
        {
            return returnType.IsConcreteAndAssignableFrom(typeof(ActionResult<>));
        }
    }
}