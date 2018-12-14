using Mozart.Composition.ModelComposition.Details.Models;
using Mozart.Composition.ModelComposition.Pricing.Models;
using Mozart.Composition.ModelComposition.Stock.Models;

namespace Mozart.Composition.AspNetCore.Mvc.Demo.Models
{
    public class Product
    {
        public ProductDetails Details { get; set; }

        public ProductPrice Price { get; set; }

        public ProductStock Stock { get; set; }
    }
}
