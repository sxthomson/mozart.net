using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mozart.Composition.Core.Abstractions;
using Mozart.Composition.Core.Extensions;

namespace Mozart.Composition.Core
{
    public class ComposeViewModelResolver : IServiceResolver<Type, IComposeViewModel>
    {
        private readonly IDictionary<Type, IComposeViewModel> _composeViewModels =
            new ConcurrentDictionary<Type, IComposeViewModel>();

        public ComposeViewModelResolver(IEnumerable<IComposeViewModel> composeViewModels)
        {
            // Do the expensive reflection piece when this class is instantiated, ideally before serving requests
            foreach (var composeViewModel in composeViewModels)
            {
                var type = composeViewModel.GetType().GetImplementedGenericArgumentsForInterface(typeof(IComposeViewModel<>)).FirstOrDefault();
                if (type != null)
                {
                    if (_composeViewModels.ContainsKey(type))
                    {
                        throw new InvalidOperationException($"Duplicate IComposeViewModel implementations found for {type.Name}.  Only one implementation is supported per type.");
                    }
                    _composeViewModels.Add(type, composeViewModel);
                }
            }
        }

        public bool TryResolve(Type key, out IComposeViewModel service)
        {
            return _composeViewModels.TryGetValue(key, out service);
        }
    }
}
