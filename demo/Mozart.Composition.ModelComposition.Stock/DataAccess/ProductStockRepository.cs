using System.Threading.Tasks;
using Mozart.Composition.Shared.DataAccess;

namespace Mozart.Composition.ModelComposition.Stock.DataAccess
{
    public class ProductStockRepository : IReadOnlyRepository<int, int>
    {
        public async Task<int> GetAsync(int key)
        {
            return await Task.FromResult(20);
        }
    }
}
