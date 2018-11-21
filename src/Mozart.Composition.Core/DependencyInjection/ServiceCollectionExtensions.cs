using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mozart.Composition.Core.Abstractions;
using Mozart.Composition.Core.Exceptions;
using Mozart.Composition.Core.Extensions;

namespace Mozart.Composition.Core.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IEnumerable<Type> AddViewModelComposition(this IServiceCollection services,
            string assemblySearchPattern = "*ViewModelComposition*.dll")
        {
            var fileNames = Directory.GetFiles(AppContext.BaseDirectory, assemblySearchPattern);

            services.AddServiceResolvers();

            return services.AddComposeViewModels(fileNames);
        }

        public static IEnumerable<Type> AddViewModelComposition(this IServiceCollection services,
            IEnumerable<string> fileNames, bool scanForServices = true)
        {
            services.AddServiceResolvers();
            return services.AddComposeViewModels(fileNames);
        }

        public static void AddDependentServicesForViewModelComposition(this IServiceCollection services, IConfiguration configuration, string assemblySearchPattern = "*ViewModelComposition*.dll")
        {
            var fileNames = Directory.GetFiles(AppContext.BaseDirectory, assemblySearchPattern);

            RegisterViewModelCompositionServices(services, configuration, fileNames);
        }

        private static IEnumerable<Type> AddComposeViewModels(this IServiceCollection services, IEnumerable<string> fileNames)
        {
            var composeViewModelTypes = new List<Type>();
            foreach (var fileName in fileNames)
            {
                var temp = AssemblyLoader.Load(fileName)
                    .GetTypes()
                    .Where(t => t.IsConcreteAndAssignableFrom(typeof(IComposeViewModel<>)));

                composeViewModelTypes.AddRange(temp);
            }

            foreach (var type in composeViewModelTypes)
            {
                // Get the implemented generic types so we can register the strongly typed service
                var genericArguments = type.GetImplementedGenericArgumentsForInterface(typeof(IComposeViewModel<>));
                var registeredType = typeof(IComposeViewModel<>).MakeGenericType(genericArguments.ToArray());
                services.AddSingleton(registeredType, type);
                services.AddSingleton(typeof(IComposeViewModel), type);
            }

            return composeViewModelTypes;
        }

        private static void AddServiceResolvers(this IServiceCollection services)
        {
            // This service is purely responsible for caching the non-generic IComposeViewModel service to a known type to prevent further reflection at runtime
            services.AddSingleton<IServiceResolver<Type, IComposeViewModel>, ComposeViewModelResolver>();
        }

        private static void RegisterViewModelCompositionServices(this IServiceCollection services, IConfiguration configuration, IEnumerable<string> fileNames)
        {
            const string serviceCollectionExtensions = "ServiceCollectionExtensions";
            const string registerServices = "RegisterServices";

            foreach (var fileName in fileNames)
            {
                var assembly = AssemblyLoader
                    .Load(fileName);
                var serviceCollectionExtensionsType = assembly
                    .GetTypes() // Is abstract && sealed helps indicate static class at IL level
                    .FirstOrDefault(t => t.IsClass && t.IsAbstract && t.IsSealed && t.Name == serviceCollectionExtensions);

                if (serviceCollectionExtensionsType != null)
                {
                    var registerServicesMethodInfo = serviceCollectionExtensionsType.GetMethod(registerServices);
                    if (registerServicesMethodInfo != null)
                    {
                        if (!registerServicesMethodInfo.IsStatic)
                        {
                            throw new RegisterServicesMethodTypeLoadException($"Assembly {assembly.FullName}'s implementation of {serviceCollectionExtensions}.{registerServices} is not static.");
                        }

                        var parameters = registerServicesMethodInfo.GetParameters();
                        switch (parameters.Length)
                        {
                            case 1 when parameters[0].ParameterType == typeof(IServiceCollection):
                                registerServicesMethodInfo.Invoke(null, new object[] { services });
                                break;
                            case 2 when parameters[0].ParameterType == typeof(IServiceCollection) && parameters[1].ParameterType == typeof(IConfiguration):
                                registerServicesMethodInfo.Invoke(null, new object[] { services, configuration });
                                break;
                            default:
                                throw new RegisterServicesMethodTypeLoadException($"Assembly {assembly.FullName}'s implementation of {serviceCollectionExtensions}.{registerServices} must have either a single input parameter of type {typeof(IServiceCollection).FullName} or with a second parameter of type ${typeof(IServiceCollection).FullName}.");
                        }
                    }
                }
            }
        }
    }
}
