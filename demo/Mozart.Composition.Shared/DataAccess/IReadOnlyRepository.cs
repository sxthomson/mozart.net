using System.Threading.Tasks;

namespace Mozart.Composition.Shared.DataAccess
{
    public interface IReadOnlyRepository<TOut, in TIn>
    {
        Task<TOut> GetAsync(TIn key);
    }
}