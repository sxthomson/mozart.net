using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozart.Composition.Core.Abstractions
{
    public abstract class ComposeViewModelBase<T> : IComposeViewModel<T>
    {
        public async Task<object> ComposeViewModel(IDictionary<string, object> parameters)
        {
            return await ComposeViewModelOfT(parameters);
        }

        public abstract Task<T> ComposeViewModelOfT(IDictionary<string, object> parameters);
    }
}
