using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozart.Composition.Core.Abstractions
{
    public interface IComposeModel
    { 
        Task<object> ComposeAsync(IDictionary<string, object> parameters);
    }
}
