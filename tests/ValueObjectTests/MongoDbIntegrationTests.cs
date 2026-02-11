using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Driver;
using ValueObject.Core;
using ValueObjectTests.Fixtures;

namespace ValueObjectTests;

public class MongoDbIntegrationTests(MongoDbFixture fixture) : IClassFixture<MongoDbFixture>, IAsyncLifetime
{
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly MongoDbTestContext _context = new(fixture.Client, fixture.DatabaseName);

    [Fact]
    public async Task Should_Save_And_Retrieve_Document_With_ValueObjects()
    {
        // Arrange
        var id = new MongoProductId(ObjectId.GenerateNewId());
        var product = new MongoProduct
        {
            Id = id,
            Name = new MongoProductName("Test Product"),
            Price = new MongoPrice(19.99m),
            Sku = new MongoSku("SKU-12345"),
            Orders = 
            [
                new MongoOrder
                {
                    Id = new MongoOrderId(ObjectId.GenerateNewId()),
                    ProductId = id,
                    Quantity = new MongoQuantity(10),
                    TotalPrice = new MongoPrice(199.90m)
                }
            ]
        };

        // Act
        await _context.Products.InsertOneAsync(product, cancellationToken: _ct);
        var retrievedProduct = await _context.Products
            .Find(p => p.Id == id)
            .FirstAsync(_ct);

        // Assert
        Assert.NotNull(retrievedProduct);
        Assert.Equal(product.Id, retrievedProduct.Id);
        Assert.Equal(product.Name, retrievedProduct.Name);
        Assert.Equal(product.Price, retrievedProduct.Price);
        Assert.Equal(product.Sku, retrievedProduct.Sku);
        Assert.Single(retrievedProduct.Orders);
        Assert.Equal(product.Orders[0].Quantity, retrievedProduct.Orders[0].Quantity);
    }

    [Fact]
    public async Task Should_Query_By_ValueObject()
    {
        // Arrange
        var sku = new MongoSku("QUERY-TEST-SKU");
        var product = new MongoProduct
        {
            Id = new MongoProductId(ObjectId.GenerateNewId()),
            Name = new MongoProductName("Query Test Product"),
            Price = new MongoPrice(29.99m),
            Sku = sku,
            Orders = []
        };
        await _context.Products.InsertOneAsync(product, cancellationToken: _ct);

        // Act
        var found = await _context.Products
            .Find(p => p.Sku == sku)
            .FirstOrDefaultAsync(_ct);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(product.Id, found.Id);
        Assert.Equal(sku, found.Sku);
    }

    [Fact]
    public async Task Should_Update_Document_With_ValueObjects()
    {
        // Arrange
        var id = new MongoProductId(ObjectId.GenerateNewId());
        var product = new MongoProduct
        {
            Id = id,
            Name = new MongoProductName("Original Name"),
            Price = new MongoPrice(10.00m),
            Sku = new MongoSku("UPDATE-SKU"),
            Orders = []
        };
        await _context.Products.InsertOneAsync(product, cancellationToken: _ct);

        // Act
        var newName = new MongoProductName("Updated Name");
        var newPrice = new MongoPrice(15.00m);
        var update = Builders<MongoProduct>.Update
            .Set(p => p.Name, newName)
            .Set(p => p.Price, newPrice);

        await _context.Products.UpdateOneAsync(
            p => p.Id == id,
            update,
            cancellationToken: _ct);

        var updated = await _context.Products
            .Find(p => p.Id == id)
            .FirstAsync(_ct);

        // Assert
        Assert.Equal(newName, updated.Name);
        Assert.Equal(newPrice, updated.Price);
    }

    [Fact]
    public async Task Should_Filter_By_Price_Range()
    {
        // Arrange
        var products = new[]
        {
            new MongoProduct
            {
                Id = new MongoProductId(ObjectId.GenerateNewId()),
                Name = new MongoProductName("Cheap Product"),
                Price = new MongoPrice(5.00m),
                Sku = new MongoSku("CHEAP-SKU"),
                Orders = []
            },
            new MongoProduct
            {
                Id = new MongoProductId(ObjectId.GenerateNewId()),
                Name = new MongoProductName("Expensive Product"),
                Price = new MongoPrice(100.00m),
                Sku = new MongoSku("EXPENSIVE-SKU"),
                Orders = []
            },
            new MongoProduct
            {
                Id = new MongoProductId(ObjectId.GenerateNewId()),
                Name = new MongoProductName("Mid Product"),
                Price = new MongoPrice(50.00m),
                Sku = new MongoSku("MID-SKU"),
                Orders = []
            }
        };

        await _context.Products.InsertManyAsync(products, cancellationToken: _ct);

        // Act
        var minPrice = new MongoPrice(20.00m);
        var maxPrice = new MongoPrice(80.00m);
        var filtered = await _context.Products
            .Find(p => p.Price >= minPrice && p.Price <= maxPrice)
            .ToListAsync(_ct);

        // Assert
        Assert.Single(filtered);
        Assert.Equal(new MongoPrice(50.00m), filtered[0].Price);
    }

    [Fact]
    public async Task MongoDb_schema_matches_snapshot()
    {
        // Arrange - Insert sample data to ensure collections exist
        var product = new MongoProduct
        {
            Id = new MongoProductId(ObjectId.GenerateNewId()),
            Name = new MongoProductName("Schema Test"),
            Price = new MongoPrice(1.00m),
            Sku = new MongoSku("SCHEMA-SKU"),
            Orders = []
        };
        await _context.Products.InsertOneAsync(product, cancellationToken: _ct);

        // Act
        var snapshot = await BuildSchemaSnapshot(_context);

        var actualJson = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        var expectedJson = await File.ReadAllTextAsync("mongodb.schema.snapshot.json", _ct);

        // Assert
        Assert.Equal(Normalize(expectedJson), Normalize(actualJson));
    }

    private static async Task<object> BuildSchemaSnapshot(MongoDbTestContext context)
    {
        var database = context.Database;
        var collections = await (await database.ListCollectionNamesAsync()).ToListAsync();

        var collectionSchemas = new List<object>();

        foreach (var collectionName in collections.OrderBy(x => x))
        {
            var collection = database.GetCollection<BsonDocument>(collectionName);
            
            // Get a sample document to infer schema
            var sampleDoc = await collection.Find(FilterDefinition<BsonDocument>.Empty)
                .Limit(1)
                .FirstOrDefaultAsync();

            if (sampleDoc == null) continue;
            var fields = sampleDoc.Elements
                .OrderBy(e => e.Name)
                .Select(e => new
                {
                    Field = e.Name,
                    BsonType = e.Value.BsonType.ToString()
                });

            collectionSchemas.Add(new
            {
                Collection = collectionName,
                Fields = fields
            });
        }

        return new { Collections = collectionSchemas };
    }

    private static string Normalize(string s) =>
        s.Replace("\r\n", "\n").Trim();

    public async ValueTask DisposeAsync()
    {
        // Clean up test data
        await _context.Products.DeleteManyAsync(FilterDefinition<MongoProduct>.Empty, _ct);
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }
}

// Value Objects for MongoDB tests
public readonly partial record struct MongoProductId(ObjectId Value) : IValueObject<ObjectId>;
public readonly partial record struct MongoProductName(string Value) : IValueObject<string>;
public readonly partial record struct MongoPrice(decimal Value) : IValueObject<decimal>;
public readonly partial record struct MongoSku(string Value) : IValueObject<string>;
public readonly partial record struct MongoOrderId(ObjectId Value) : IValueObject<ObjectId>;
public readonly partial record struct MongoQuantity(int Value) : IValueObject<int>;

// MongoDB Documents
public class MongoProduct
{
    public required MongoProductId Id { get; set; }
    public required MongoProductName Name { get; set; }
    public required MongoPrice Price { get; set; }
    public MongoSku? Sku { get; set; }
    public List<MongoOrder> Orders { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MongoOrder
{
    public required MongoOrderId Id { get; set; }
    public required MongoProductId ProductId { get; set; }
    public required MongoQuantity Quantity { get; set; }
    public required MongoPrice TotalPrice { get; set; }
    public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
}

// MongoDB Context
public class MongoDbTestContext(IMongoClient client, string databaseName)
{
    public IMongoDatabase Database { get; } = client.GetDatabase(databaseName);

    public IMongoCollection<MongoProduct> Products => Database.GetCollection<MongoProduct>("Products");
    public IMongoCollection<MongoOrder> Orders => Database.GetCollection<MongoOrder>("Orders");
}



