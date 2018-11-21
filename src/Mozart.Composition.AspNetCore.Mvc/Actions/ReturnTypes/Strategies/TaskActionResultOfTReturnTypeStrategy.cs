using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozart.Composition.Core;
using Mozart.Composition.Core.Extensions;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies
{
    public class TaskActionResultOfTReturnTypeStrategy : ActionResultOfTReturnTypeStrategy
    {
        public override bool Handles(Type returnType)
        {
            return returnType.IsConcreteAndAssignableFrom(typeof(Task<>))
                   && base.Handles(returnType.GenericTypeArguments[0]);
        }

        public override Type GetUnwrappedReturnType(Type returnType, IEnumerable<Attribute> attributes)
        {
            if (!Handles(returnType))
            {
                throw new InvalidOperationException(
                    $"This class does not handle this return type. The return type is not of generic type {nameof(ActionResult)} wrapped up in a {nameof(Task)}.");
            }
            // Unwrap the return type from the Task and return base

            return base.GetUnwrappedReturnType(returnType.GenericTypeArguments[0], attributes);
        }
    }
}