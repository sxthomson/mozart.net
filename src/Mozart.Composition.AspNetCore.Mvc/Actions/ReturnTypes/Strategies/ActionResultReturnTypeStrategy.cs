using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies.Abstractions;
using Mozart.Composition.AspNetCore.Mvc.Exceptions;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies
{
    public class ActionResultReturnTypeStrategy : IActionReturnTypeStrategy
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
                    $"This class does not handle this return type.  The return type is not of type {nameof(ActionResult)} or {nameof(IActionResult)}.");
            }

            var producesResponseAttribute = attributes.OfType<ProducesResponseTypeAttribute>().FirstOrDefault(att => att.StatusCode == 200);
            if (producesResponseAttribute == null)
            {
                throw new MissingExpectedAttributeException($"Missing {nameof(ProducesResponseTypeAttribute)} attribute for 200 OK status.");
            }
            return producesResponseAttribute.Type;
        }

        // Separate method to allow for parent's unwrapping of types and passing through via overrides
        private static bool HandlesType(Type returnType)
        {
            return returnType == typeof(ActionResult) || returnType == typeof(IActionResult);
        }
    }
}