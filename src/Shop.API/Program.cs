using FluentValidation;
using FluentValidation.AspNetCore;
using Shop.BasketDomain.Application.Services;
using Shop.BasketDomain.Domain.Factories;
using Shop.BasketDomain.Domain.Repositories;
using Shop.BasketDomain.Domain.Services;
using Shop.BasketDomain.Infrastructure.Repositories;
using Shop.ProductDomain.Application.Services;
using Shop.ProductDomain.Domain.Repositories;
using Shop.ProductDomain.Infrastructure.Repositories;
using Shop.ShippingDomain.Application.Services;
using Shop.ShippingDomain.Domain.Repositories;
using Shop.ShippingDomain.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Configure FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Basket Services
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddSingleton<BasketFactory>();
builder.Services.AddSingleton<IPriceCalculationService, PriceCalculationService>();
builder.Services.AddSingleton<ITaxCalculationService, TaxCalculationService>();
builder.Services.AddSingleton<IDiscountCodeService, DiscountCodeService>();
builder.Services.AddSingleton<IBasketRepository, InMemoryBasketRepository>();
builder.Services.AddSingleton<IDiscountCodeRepository, InMemoryDiscountCodeRepository>();

// Register Product Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();

// Register Shipping Services
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddSingleton<IShippingRepository, InMemoryShippingRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Online Shopping Basket API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
    app.UseDeveloperExceptionPage();
}
else
{
    // Optional: custom error endpoint
    app.UseExceptionHandler("/error");
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
