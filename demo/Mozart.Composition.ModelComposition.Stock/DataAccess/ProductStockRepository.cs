using System.Threading.Tasks;
using Mozart.Composition.Shared.DataAccess;

namespace Mozart.Composition.ModelComposition.Stock.DataAccess
{
    public class ProductStockRepository : IReadOnlyRepository<int, int>
    {
        public Task<int> GetAsync(int key)
        {
            return Task.FromResult(20);
        }
    }
}
