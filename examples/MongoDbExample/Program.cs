using AspConfig;
using MongoDB.Driver;
using MongoDbExample;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ValueObject.Core;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new ValueObjectJsonConverterFactory());
    options.SerializerOptions.Converters.Add(new ObjectIdJsonConverter());
});

// MongoDB configuration
builder.AddMongoDBClient("mongodb");
MongoClassMaps.RegisterAll();
builder.Services.AddScoped<MongoDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapDefaultEndpoints();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/scalar")).WithName("Home");

// Products Endpoints
app.MapGet("/products", async (MongoDbContext db) =>
{
    var products = await db.Products.Find(FilterDefinition<Product>.Empty).ToListAsync();
    return Results.Ok(products);
})
.WithName("GetProducts");

app.MapGet("/products/{id}", async ([FromRoute] ProductId id, MongoDbContext db) =>
{
    var product = await db.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
    return product is null ? Results.NotFound() : Results.Ok(product);
})
.WithName("GetProductById");

app.MapPost("/products", async ([FromBody] CreateProductRequest request, MongoDbContext db) =>
{
    var product = new Product
    {
        Id = new ProductId(ObjectId.GenerateNewId()),
        Name = new ProductName(request.Name),
        Price = new Price(request.Price)
    };

    await db.Products.InsertOneAsync(product);

    return Results.Created($"/products/{product.Id.Value}", product);
})
.WithName("CreateProduct")
.Produces<Product>(StatusCodes.Status201Created);

app.Run();

public record CreateProductRequest(string Name, decimal Price);

