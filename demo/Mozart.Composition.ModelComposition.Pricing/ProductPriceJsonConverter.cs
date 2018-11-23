using System;
using Mozart.Composition.ModelComposition.Pricing.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mozart.Composition.ModelComposition.Pricing
{
    public class ProductPriceJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken.FromObject(((ProductPrice) value).Price).WriteTo(writer);

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new ProductPrice
            {
                Price = decimal.Parse(JToken.Load(reader).ToString())
            };
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ProductPrice));
        }
    }
}
