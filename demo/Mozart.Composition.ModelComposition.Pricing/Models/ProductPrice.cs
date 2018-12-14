using Newtonsoft.Json;

namespace Mozart.Composition.ModelComposition.Pricing.Models
{
    [JsonConverter(typeof(ProductPriceJsonConverter))]
    public class ProductPrice
    {
        public decimal Price { get; set; }
    }
}
