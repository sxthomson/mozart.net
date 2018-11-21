using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozart.Composition.Core.Abstractions
{
    public interface IComposeViewModel
    { 
        Task<object> ComposeViewModel(IDictionary<string, object> parameters);
    }
}
