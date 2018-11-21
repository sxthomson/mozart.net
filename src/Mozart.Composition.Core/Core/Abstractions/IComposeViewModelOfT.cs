using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozart.Composition.Core.Abstractions
{
    public interface IComposeViewModel<T> : IComposeViewModel
    {
        Task<T> ComposeViewModelOfT(IDictionary<string, object> parameters);
    }
}
