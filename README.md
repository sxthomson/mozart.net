# Mozart .NET
Mozart .NET - Model composition via strongly typed aggregation

# Overview
Mozart .NET was written in an attempt to help decouple your model composition where you have multiple micro-services which all all responsible for a separate part of your domain.

In a typical ASP.NET project we might have to have to resort to having a Controller with many different service layer dependencies, or push that down a layer to having a fat service with many data layer dependencies.  We can use an IoC container and inject the dependencies but ultimately we'll have a class that breaks single responsibility principle and is difficult to test succinctly.

Mozart .NET decouples your domains from your web application by automatically composing your composite models automatically through a single action filter.  When you enable Mozart .NET's model composition for a model, at runtime, Mozart .NET will automatically orchestrate the creation of your model, resolving each property by typed composer.  This will allow you to build separate composition projects which have their own package dependencies, their own dependency registration which are completely standalone.

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
    app.WarmupMozartModelComposition();
    ...
}
```

## Creating a composite model

For this example, we'll surface some information about a generic Product that could be used to power an e-Commerce site.  To see a completed example solution that follows best practices look in [the demo folder](/demo).

We need a model to return and a routeable controller that's decorated with the `MozartComposeModelAttribute`:

```c#
public class Product
{
    // We'll add some properties later!
}
```

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
Note: Mozart .NET w
