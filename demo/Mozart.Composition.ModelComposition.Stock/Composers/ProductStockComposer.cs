using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mozart.Composition.Core.Abstractions;
using Mozart.Composition.ModelComposition.Stock.Models;
using Mozart.Composition.ModelComposition.Stock.Services;
using Mozart.Composition.Shared.Extensions;

namespace Mozart.Composition.ModelComposition.Stock.Composers
{
    public class ProductStockComposer : ComposeModel<ProductStock>
    {
        private readonly IProductStockService _productStockService;

        public ProductStockComposer(IProductStockService productStockService)
        {
            _productStockService = productStockService ?? throw new ArgumentNullException(nameof(productStockService));
        }

        public override async Task<ProductStock> ComposeOfT(IDictionary<string, object> parameters)
        {
            var id = parameters.GetId();

            return await _productStockService.GetProductStockAsync(id);
        }
    }
}
