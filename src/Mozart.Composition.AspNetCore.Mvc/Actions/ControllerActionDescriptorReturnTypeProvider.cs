using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Mozart.Composition.AspNetCore.Mvc.Actions.Abstractions;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Actions
{
    public class ControllerActionDescriptorReturnTypeProvider : IActionDescriptorReturnTypeProvider<ControllerActionDescriptor>
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly IActionReturnTypeResolver _actionReturnTypeResolver;

        public ControllerActionDescriptorReturnTypeProvider(
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            IActionReturnTypeResolver actionReturnTypeResolver)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider ?? throw new ArgumentNullException(nameof(actionDescriptorCollectionProvider));
            _actionReturnTypeResolver = actionReturnTypeResolver ?? throw new ArgumentNullException(nameof(actionReturnTypeResolver));
        }

        public IEnumerable<(string Id, Type ReturnType)> ResolveAll(Func<ControllerActionDescriptor, bool> predicate)
        {
            // Resolve each time this is called as IActionDescriptorCollectionProvider's collection can change at runtime
            var controllerActionDescriptors = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                .OfType<ControllerActionDescriptor>()
                .Where(predicate);

            foreach (var descriptor in controllerActionDescriptors)
            {
                var methodInfo = descriptor.MethodInfo;
                var actionReturnType = _actionReturnTypeResolver.ResolveUnwrappedReturnType(methodInfo.ReturnType, methodInfo.GetCustomAttributes<Attribute>(true));
                yield return (descriptor.Id, actionReturnType);
            }
        }
    }
}
