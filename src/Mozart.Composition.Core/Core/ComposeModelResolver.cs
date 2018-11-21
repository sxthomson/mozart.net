using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mozart.Composition.Core.Abstractions;
using Mozart.Composition.Core.Extensions;

namespace Mozart.Composition.Core
{
    public class ComposeModelResolver : ICachedServiceResolver<Type, IComposeModel>
    {
        private readonly IDictionary<Type, IComposeModel> _composeModels =
            new ConcurrentDictionary<Type, IComposeModel>();

        public ComposeModelResolver(IEnumerable<IComposeModel> composeModels)
        {
            // Do the expensive reflection piece when this class is instantiated, ideally before serving requests
            foreach (var composeModel in composeModels)
            {
                var type = composeModel.GetType().GetImplementedGenericArgumentsForInterface(typeof(IComposeModel<>)).FirstOrDefault();
                if (type != null)
                {
                    if (_composeModels.ContainsKey(type))
                    {
                        throw new InvalidOperationException($"Duplicate IComposeModel implementations found for {type.Name}.  Only one implementation is supported per type.");
                    }
                    _composeModels.Add(type, composeModel);
                }
            }
        }

        public bool TryResolve(Type key, out IComposeModel service)
        {
            return _composeModels.TryGetValue(key, out service);
        }
    }
}
