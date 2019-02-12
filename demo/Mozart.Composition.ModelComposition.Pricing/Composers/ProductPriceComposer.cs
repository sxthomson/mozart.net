using System.Collections.Generic;
using System.Threading.Tasks;
using Mozart.Composition.Core.Abstractions;
using Mozart.Composition.ModelComposition.Pricing.Models;

namespace Mozart.Composition.ModelComposition.Pricing.Composers
{
    public class ProductPriceComposer : ComposeModel<ProductPrice>
    {
        public override async Task<ProductPrice> ComposeOfTAsync(IDictionary<string, object> parameters)
        {
            return await Task.FromResult(new ProductPrice
            {
                Price = 12.0m
            });
        }
    }
}
