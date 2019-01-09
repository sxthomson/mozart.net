using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Mozart.Composition.AspNetCore.Mvc.Actions;
using Mozart.Composition.AspNetCore.Mvc.Actions.Abstractions;
using Mozart.Composition.AspNetCore.Mvc.Actions.Filters;
using Mozart.Composition.AspNetCore.Mvc.Actions.Predicates;
using Mozart.Composition.AspNetCore.Mvc.Actions.Predicates.Abstractions;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Abstractions;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies;
using Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies.Abstractions;
using Mozart.Composition.AspNetCore.Mvc.Results;
using Mozart.Composition.AspNetCore.Mvc.Results.Abstractions;
using Mozart.Composition.Core.Abstractions;
using Mozart.Composition.Core.DependencyInjection;
using Mozart.Composition.Core.Extensions;

namespace Mozart.Composition.AspNetCore.Mvc.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMozartMvcComposition(this IServiceCollection serviceCollection, string assemblySearchPattern = "*ModelComposition*.dll")
        {
            var fileNames = Directory.GetFiles(AppContext.BaseDirectory, assemblySearchPattern);
            
            // Register the IComposeModel<> types
            var composeModelTypes = serviceCollection.AddMozartModelComposition(fileNames);

            // Register a default CompositionResultHandler to handle our composite view models 
            serviceCollection.AddSingleton(typeof(IHandleResult<>), typeof(CompositionResultHandler<>));

            // Register an EntireResultHandler for each IComposeModel<T> - this allows each handler to be invoked directly if we only want to return a single property
            foreach (var type in composeModelTypes)
            {
                // Get the implemented generic types so we can register the strongly typed service
                var genericArguments = type.GetImplementedGenericArgumentsForInterface(typeof(IComposeModel<>)).ToArray();
                var registeredType = typeof(IHandleResult<>).MakeGenericType(genericArguments);
                var handlerConcreteType = typeof(EntireResultHandler<>).MakeGenericType(genericArguments);
                serviceCollection.AddSingleton(registeredType, handlerConcreteType);                
            }
            
            RegisterResultHandlerResolver(serviceCollection);

            RegisterActionReturnTypeStrategyServices(serviceCollection);

            return serviceCollection;
        }

        private static void RegisterResultHandlerResolver(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IActionDescriptorReturnTypeProvider<ControllerActionDescriptor>, ControllerActionDescriptorReturnTypeProvider>();
            serviceCollection.AddSingleton<IPredicate<ControllerActionDescriptor>, ControllerActionDescriptorHttpGetPredicateWrapper>();
            serviceCollection.AddSingleton<ICachedServiceResolver<string, IHandleResult>, CompositionResultHandlerResolver>();
        }

        private static void RegisterActionReturnTypeStrategyServices(IServiceCollection serviceCollection)
        {
            // Register the default IActionReturnTypeStrategy types from this assembly
            var types = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(t => t.IsConcreteAndAssignableFrom(typeof(IActionReturnTypeStrategy)));

            foreach (var type in types)
            {
                serviceCollection.AddSingleton(typeof(IActionReturnTypeStrategy), type);
            }

            serviceCollection.AddSingleton<IDefaultActionReturnTypeStrategy, DefaultActionResultReturnTypeStrategy>();
            serviceCollection.AddSingleton<IActionReturnTypeResolver, ActionReturnTypeResolver>();
        }
    }
}
