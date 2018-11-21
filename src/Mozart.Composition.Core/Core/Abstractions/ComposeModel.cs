using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozart.Composition.Core.Abstractions
{
    public abstract class ComposeModel<T> : IComposeModel<T>
    {
        public async Task<object> Compose(IDictionary<string, object> parameters)
        {
            return await ComposeOfT(parameters);
        }

        public abstract Task<T> ComposeOfT(IDictionary<string, object> parameters);
    }
}
