using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Controllers;
using Mozart.Composition.AspNetCore.Mvc.Actions.Abstractions;
using Mozart.Composition.AspNetCore.Mvc.Actions.Predicates.Abstractions;
using Mozart.Composition.AspNetCore.Mvc.Results.Abstractions;
using Mozart.Composition.Core.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Results
{
    public class CompositionResultHandlerResolver : IServiceResolver<string, IHandleResult>
    {
        private readonly IDictionary<string, IHandleResult> _cachedResultHandlers =
            new ConcurrentDictionary<string, IHandleResult>();

        public CompositionResultHandlerResolver(
            IActionDescriptorReturnTypeProvider<ControllerActionDescriptor> controllerActionDescriptorReturnTypeProvider,
            IPredicate<ControllerActionDescriptor> controllerActionDescriptorPredicate,
            IServiceProvider serviceProvider)
        {
            // Get all the ID / unwrapped return types that match our action predicate
            var results = controllerActionDescriptorReturnTypeProvider.ResolveAll(controllerActionDescriptorPredicate.Predicate);

            // Cache the Handler<T> where T is the return type to the unique Action ID (heavy reflection work on startup instead of per request)
            foreach (var (id, returnType) in results)
            {
                var serviceLocatorType = typeof(IHandleResult<>).MakeGenericType(returnType);

                if (serviceProvider.GetService(serviceLocatorType) is IHandleResult handler)
                {
                    _cachedResultHandlers.Add(id, handler);
                }
                else
                {
                    // TODO Log a warning or throw?
                }
            }
        }

        public bool TryResolve(string key, out IHandleResult service)
        {
            return _cachedResultHandlers.TryGetValue(key, out service);
        }
    }
}
