using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mozart.Composition.ModelComposition.Stock.Configuration;
using Mozart.Composition.ModelComposition.Stock.DataAccess;
using Mozart.Composition.ModelComposition.Stock.Services;
using Mozart.Composition.Shared.DataAccess;

namespace Mozart.Composition.ModelComposition.Stock.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StockSubOptions>(configuration.GetSection("Stock"));

            services.AddSingleton<IReadOnlyRepository<int, int>, ProductStockRepository>();

            services.AddSingleton<IProductStockService, ProductStockService>();
        }
    }
}
