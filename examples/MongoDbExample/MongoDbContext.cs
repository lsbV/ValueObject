using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using ValueObject.Core;

namespace MongoDbExample;

/// <summary>
/// Example MongoDB context with a collection for Product entities using ValueObjects
/// </summary>
public sealed class MongoDbContext(IMongoClient client, string databaseName = "ValueObjectExample")
{
    private readonly IMongoDatabase _database = client.GetDatabase(databaseName);

    public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");

    public async Task EnsureIndexesAsync()
    {
        // Create indexes for better query performance
        await Products.Indexes.CreateOneAsync(
            new CreateIndexModel<Product>(Builders<Product>.IndexKeys.Ascending(p => p.Id))
        );
    }
}

// Value Objects
public readonly partial record struct ProductId(ObjectId Value) : IValueObject<ObjectId>;
public readonly partial record struct ProductName(string Value) : IValueObject<string>;
public readonly partial record struct Price(decimal Value) : IValueObject<decimal>;

// MongoDB Documents
public class Product
{
    [BsonId]
    public required ProductId Id { get; set; }
    public required ProductName Name { get; set; }
    public required Price Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


