using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozart.Composition.Core.Abstractions
{
    public interface IMozartModelComposer<TModel> where TModel : class, new()
    {
        Task<TModel> BuildCompositeModelAsync(IDictionary<string, object> parameters);
    }
}