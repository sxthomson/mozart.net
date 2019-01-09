using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mozart.Composition.Core.Abstractions;

namespace Mozart.Composition.Core
{
    public class MozartAggregateModelComposer<TModel> : IMozartModelComposer<TModel> where TModel: class, new ()
    {
        private readonly ICachedServiceResolver<Type, IComposeModel> _modelComposerResolver;
        private readonly IEnumerable<PropertyInfo> _cachedPropertyInfos;

        public MozartAggregateModelComposer(ICachedServiceResolver<Type, IComposeModel> modelComposerResolver)
        {
            // The custom resolver that will get our view model composition services by property type
            _modelComposerResolver = modelComposerResolver;

            // Do the expensive reflection piece when this class is instantiated (as a singleton), ideally before serving requests
            _cachedPropertyInfos = typeof(TModel).GetProperties().Where(x => x.CanRead && x.CanWrite);
        }

        public async Task<TModel> BuildCompositeModelAsync(IDictionary<string, object> parameters)
        {           
            var result = new TModel();

            var pendingTasks = new List<Task>();

            foreach (var cachedPropertyInfo in _cachedPropertyInfos)
            {
                // Find an IComposeModel that can service this particular property type
                if (!_modelComposerResolver.TryResolve(cachedPropertyInfo.PropertyType, out var composer))
                {
                    //Log a warning 
                    continue;
                }

                pendingTasks.Add(AssignPropertyFromComposer(composer, cachedPropertyInfo, result, parameters));
            }

            if (pendingTasks.Count == 0)
            {
                return null;
            }

            await Task.WhenAll(pendingTasks);
            return result;
        }

        private static async Task AssignPropertyFromComposer(IComposeModel composer, PropertyInfo cachedPropertyInfo,
            TModel result, IDictionary<string, object> routeParameters)
        {
            var propertyValue = await composer.ComposeAsync(routeParameters);
            cachedPropertyInfo.SetValue(result, propertyValue);
        }
    }
}
