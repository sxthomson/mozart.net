using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozart.Composition.Core.Abstractions
{
    public interface IComposeModel<T> : IComposeModel
    {
        Task<T> ComposeOfTAsync(IDictionary<string, object> parameters);
    }
}
