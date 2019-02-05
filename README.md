# Mozart .NET

Mozart .NET - Strongly typed automatic model composition

# Overview

Mozart .NET was written in an attempt to help decouple your model composition where you have multiple micro-services which are all responsible for a separate part of your domain or own a separate component part of your final data model.

In a typical ASP.NET Core project we might have to have to resort to having a Controller with many different service layer dependencies, or push that down a layer to having a "fat" service with many data layer dependencies.  We can use an IoC container and inject the dependencies but ultimately we'll have a class that breaks single responsibility principle and is difficult to test succinctly.

Mozart .NET decouples your domains projects from your web application by automatically composing your composite models automatically through interception or an exposed service.  When you enable Mozart .NET's model composition for a model, at runtime, Mozart .NET will automatically orchestrate the creation of your model, resolving each property by typed composer that you as a developer have registered.  This will allow you to build separate composition projects which have their own NuGet package dependencies, their own dependency registration code and are ultimately much more portable and reusable.

# Getting Started

## Adding Mozart .NET

Firstly in your ASP.NET web project, we need to add the core Mozart .NET servicesin `Startup.cs` add the following to your `ConfigureServices` method.

```c#
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddMozartMvcComposition();
    ...
}
```

At startup, Mozart .NET will scan your assemblies looking for your composers (more on that later!) and wire up the services required to add the auto composition when a model is to be returned via an action. You can optionally provide an overridden assembly search pattern for where your composers are declared, by default, this is `*ModelComposition*.

Secondly, as Mozart .NET resolves a composer per type, a lot of reflection needs to take place.  In order to ensure this happens before your web application is ready to serve requests add the following code to where you're configuring the request pipeline.  In a new templated web application in .NET Core, that will be in the `Configure` method in `Startup.cs`.

```c#
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    ...
    app.WarmupMozartModelComposition(); // Resolve the Mozart .NET root level services
    ...
}
```

## Creating a composite model

For this example, we'll surface some information about a generic Product that could be used to power an e-Commerce site.  To see a completed example solution look in [the demo folder](/demo).

Create a composite `Product` model.

```c#
public class Product
{
    public ProductDetails Details { get; set; }

    public ProductPrice Price { get; set; }

    public ProductStock Stock { get; set; }
}
```

Types are important in Mozart .NET.  Each property within a composite model should be it's own type.  This allows us to register a class responsible for resolving that particular type at runtime.

Following this principle we add the classes referenced within the composite model as properties.  Ideally, these would exist within their own class libraries to add a clear separation of concerns and promote portability.

e.g.

```c#
public class ProductStock
{
    public int Units { get; set; }

    public bool IsLow { get; set; }
}
```


## Registering IComposeModel implementations

For each property within your composite model, you need to write a resolver.  You can do this by implementing the interface `IComposeViewModel<T>` where `T` denotes a property type within your composite. The base class `ComposeViewModel<T>` makes this easier by handling the generic vs. non-generic invocations for you.

e.g.

```c#
public class ProductStockComposer : ComposeModel<ProductStock>
{
    ...
    public override async Task<ProductStock> ComposeOfT(IDictionary<string, object> parameters)
    {
        // Code to surface a new instance of ProductStock based on the passed in parameters
    }
    ...
}
```

Within the demo app you'll see how `ProductStock` is resolved via a service and data access layer.  Both `ProductDetails` and `ProductPrice`'s composers represent the simplest possible implementation as a contrast.  What your implementations ultimately look like is up to you.

## Automatic Dependency Registration  (optional)

If you do want to follow best practice and keep each domain project separate then you may also want to keep dependency registration separate as well.  This minimises the footprint of each individual domain on your API gateway project.  Mozart .NET allows you to specify a convention within each scanned project whereby you can register dependencies using `IServiceCollection`.

To enable this feature firstly add the following to your `Startup.cs` class:

```c#
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.ScanForAndRegisterServicesForMozartByConvention(Configuration);
    ...
}
```

In each project all you need to do is register a static extension method on `IServiceCollection` called `RegisterServices` within a static class called `ServiceCollectionExtensions`.  The `IConfiguration` parameter is optional, use it if you want to leverage the `IOptions` pattern.

e.g.

```c#
public static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // The API project will have a configuration section at root level named "Stock"
        services.Configure<StockSubOptions>(configuration.GetSection("Stock"));

        // Dependent services for this domain's IComposeModel implementation
        services.AddSingleton<IReadOnlyRepository<int, int>, ProductStockRepository>();

        services.AddSingleton<IProductStockService, ProductStockService>();
    }
}
```

## Wiring up the Composition

### 1. Result Interception 

```c#
[Route("api/[controller]")]
[ApiController]
[MozartComposeModel]
public class ProductController : ControllerBase
{
    // GET: api/<controller>/{id}
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(Product))]
    [Route("{id}")]
    public IActionResult Get(int id)
    {
        return Ok();
    }
}
```
> Mozart .NET will unwrap the true return type from your controller action if it's a recognised ASP.NET Core return type, including anything wrapped in a `Task` or your own POCO types.  If you want to use some of the non-generic return types like `IActionResult` or `ActionResult` then you have to decorate your action with `ProducesResponseTypeAttribute` for a `200OK` as per Microsoft's documentation.

### 2. Injectable Service

```c#
[Route("api/[controller]")]
[ApiController]
public class ProductAsyncController : ControllerBase
{
    private readonly IMozartModelComposer<Product> _productModelComposer;

    public ProductAsyncController(IMozartModelComposer<Product> productModelComposer)
    {
        _productModelComposer = productModelComposer ?? throw new ArgumentNullException(nameof(productModelComposer));
    }

    // GET: api/<controller>/{id}/IActionResult
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(Product))]
    [ProducesResponseType(404)]
    [Route("{id}/IActionResult")]
    public async Task<IActionResult> GetProductIActionResult(int id)
    {
        var result = await _productModelComposer.BuildCompositeModelAsync(HttpContext.GetRouteData().Values);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
```

# Inspiration

This project was inspired by [Udi Dahan's](https://github.com/udidahan) excellent talk titled *"Own The Future NServiceBus Style"* which you can watch [here](https://www.youtube.com/watch?v=CCX8Sox6BNQ).  This is an attempt at creating a simple, read-only, strongly typed versiion of the framework he presented in this talk.