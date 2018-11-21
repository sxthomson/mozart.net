using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies.Abstractions;
using Mozart.Composition.Core;
using Mozart.Composition.Core.Extensions;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies
{
    public class DefaultActionResultReturnTypeStrategy : IDefaultActionReturnTypeStrategy
    {
        public Type GetUnwrappedReturnType(Type returnType, IEnumerable<Attribute> attributes)
        {
            if (returnType.IsConcreteAndAssignableFrom(typeof(Task<>)))
            {
                return returnType.GenericTypeArguments[0];
            }

            return returnType;
        }
    }
}