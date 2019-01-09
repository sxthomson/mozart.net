using System.Collections.Generic;
using System.Threading.Tasks;
using Mozart.Composition.Core.Abstractions;
using Mozart.Composition.ModelComposition.Details.Models;

namespace Mozart.Composition.ModelComposition.Details.Composers
{
    public class ProductPriceComposer : ComposeModel<ProductDetails>
    {
        public override async Task<ProductDetails> ComposeOfTAsync(IDictionary<string, object> parameters)
        {
            return new ProductDetails
            {
                Title = "Sample Product",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
            };
        }
    }
}
