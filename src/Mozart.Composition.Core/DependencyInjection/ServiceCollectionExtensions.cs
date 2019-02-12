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
        public static IEnumerable<Type> AddMozartModelComposition(this IServiceCollection services,
            string assemblySearchPattern = "*ModelComposition*.dll")
        {
            var fileNames = Directory.GetFiles(AppContext.BaseDirectory, assemblySearchPattern);

            services.AddServiceResolvers();

            return services.RegisterComposeModels(fileNames);
        }

        public static IEnumerable<Type> AddMozartModelComposition(this IServiceCollection services,
            IEnumerable<string> fileNames, bool scanForServices = true)
        {
            services.AddServiceResolvers();
            return services.RegisterComposeModels(fileNames);
        }

        public static void ScanForAndRegisterServicesForMozartByConvention(this IServiceCollection services, IConfiguration configuration, string assemblySearchPattern = "*ModelComposition*.dll")
        {
            var fileNames = Directory.GetFiles(AppContext.BaseDirectory, assemblySearchPattern);

            RegisterModelCompositionServices(services, configuration, fileNames);
        }

        private static IEnumerable<Type> RegisterComposeModels(this IServiceCollection services, IEnumerable<string> fileNames)
        {
            var composeModelTypes = new List<Type>();
            foreach (var fileName in fileNames)
            {
                var temp = AssemblyLoader.Load(fileName)
                    .GetTypes()
                    .Where(t => t.IsConcreteAndAssignableFrom(typeof(IComposeModel<>)));

                composeModelTypes.AddRange(temp);
            }

            foreach (var type in composeModelTypes)
            {
                // Get the implemented generic types so we can register the strongly typed service
                var genericArguments = type.GetImplementedGenericArgumentsForInterface(typeof(IComposeModel<>));
                var registeredType = typeof(IComposeModel<>).MakeGenericType(genericArguments.ToArray());
                services.AddSingleton(registeredType, type);
                services.AddSingleton(typeof(IComposeModel), type);
            }

            return composeModelTypes;
        }

        private static void AddServiceResolvers(this IServiceCollection services)
        {
            // This service is purely responsible for caching the non-generic IComposeModel service to a known type to prevent further reflection at runtime
            services.AddSingleton<ICachedServiceResolver<Type, IComposeModel>, ComposeModelResolver>();
        }

        private static void RegisterModelCompositionServices(this IServiceCollection services, IConfiguration configuration, IEnumerable<string> fileNames)
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
