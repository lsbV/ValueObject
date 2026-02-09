using AspConfig;
using EfCoreExample;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ValueObject.Core;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.AddNpgsqlDbContext<AppDbContext>("postgres");
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new ValueObjectJsonConverterFactory());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapDefaultEndpoints();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/scalar")).WithName("Home");

app.MapGet("/products", async (AppDbContext db, [FromQuery] ProductName? name) =>
{
    var query = db.Products.AsQueryable();
    if (name is not null)
        query = query.Where(p => ((string)p.Name).Contains(name));
    
    var products = await query.ToListAsync();
    
    return Results.Ok(products);
})
.WithName("GetProducts");

app.MapGet("/products/{id}", async ([FromRoute] ProductId id, AppDbContext db) =>
{
    var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
    return product is null ? Results.NotFound() : Results.Ok(product);
})
.WithName("GetProductById");

app.MapPost("/products", async ([FromBody] CreateProductRequest request, AppDbContext db) =>
{
    var product = new Product
    {
        Id = new ProductId(Guid.NewGuid()),
        Name = request.Name.As.ProductName,
        Price = new Price(request.Price)
    };

    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/products/{product.Id.Value}", product);
})
.WithName("CreateProduct")
.Produces<Product>(StatusCodes.Status201Created);

app.Run();

public record CreateProductRequest(string Name, decimal Price);