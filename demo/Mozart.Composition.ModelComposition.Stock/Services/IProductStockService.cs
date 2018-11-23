using System.Threading.Tasks;
using Mozart.Composition.ModelComposition.Stock.Models;

namespace Mozart.Composition.ModelComposition.Stock.Services
{
    public interface IProductStockService
    {
        Task<ProductStock> GetProductStockAsync(int id);
    }
}
