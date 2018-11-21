using System;
using System.Collections.Generic;
using System.Linq;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Abstractions;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes
{
    public class ActionReturnTypeResolver : IActionReturnTypeResolver
    {
        private readonly IEnumerable<IActionReturnTypeStrategy> _actionReturnTypeStrategies;
        private readonly IDefaultActionReturnTypeStrategy _defaultTypeStrategy;

        public ActionReturnTypeResolver(IEnumerable<IActionReturnTypeStrategy> actionReturnTypeStrategies, IDefaultActionReturnTypeStrategy defaultStrategy)
        {
            _actionReturnTypeStrategies = actionReturnTypeStrategies ?? throw new ArgumentNullException(nameof(actionReturnTypeStrategies));
            _defaultTypeStrategy = defaultStrategy ?? throw new ArgumentNullException(nameof(defaultStrategy));
        }

        public Type ResolveUnwrappedReturnType(Type returnType, IEnumerable<Attribute> attributes)
        {
            var strategy = _actionReturnTypeStrategies.FirstOrDefault(x => x.Handles(returnType));

            // If there is no strategy to service this type then use the default implementation
            return strategy == null ? _defaultTypeStrategy.GetUnwrappedReturnType(returnType, attributes) : strategy.GetUnwrappedReturnType(returnType, attributes);
        }
    }
}
