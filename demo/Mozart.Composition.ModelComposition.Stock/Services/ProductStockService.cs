using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Mozart.Composition.ModelComposition.Stock.Configuration;
using Mozart.Composition.ModelComposition.Stock.DataAccess;
using Mozart.Composition.ModelComposition.Stock.Models;
using Mozart.Composition.Shared.DataAccess;

namespace Mozart.Composition.ModelComposition.Stock.Services
{
    public class ProductStockService : IProductStockService
    {
        private readonly IOptions<StockSubOptions> _options;
        private readonly IReadOnlyRepository<int, int> _stockRepository;

        public ProductStockService(IReadOnlyRepository<int, int> stockRepository, IOptions<StockSubOptions> options)
        {
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<ProductStock> GetProductStockAsync(int id)
        {
            var stockUnits = await _stockRepository.GetAsync(id);
            var isLowStock = stockUnits <= _options.Value.LowStockThreshold;
            return new ProductStock {Units = stockUnits, IsLow = isLowStock};
        }
    }
}
