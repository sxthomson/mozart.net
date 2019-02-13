using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozart.Composition.Core.Abstractions
{
    public abstract class ComposeModel<T> : IComposeModel<T>
    {
        public async Task<object> ComposeAsync(IDictionary<string, object> parameters)
        {
            // Implicitly cast down to object to save more reflection at runtime
            return await ComposeOfTAsync(parameters);
        }

        public abstract Task<T> ComposeOfTAsync(IDictionary<string, object> parameters);
    }
}
