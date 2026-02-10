using Microsoft.EntityFrameworkCore;
using ValueObject.Core;

namespace ValueObjectTests;

public class EntityFrameworkTests
{
    private readonly ProductDbContext _context = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Should_Save_And_Retrieve_Entity_With_ValueObjects()
    {
        var product = new Product
        {
            Id = new ProductId(Guid.NewGuid()),
            Price = new Price(19.99m),
            ImageUrl = new ImageUrl("https://example.com/image.jpg")
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync(_ct);
        var retrievedProduct = await _context.Products.FirstAsync(p => p.Id == product.Id, _ct);
        Assert.NotNull(retrievedProduct);
        Assert.Equal(product.Id, retrievedProduct.Id);
        Assert.Equal(product.Price, retrievedProduct.Price);
        Assert.Equal(product.ImageUrl, retrievedProduct.ImageUrl);
    }
}

public readonly partial record struct Price(decimal Value) : IValueObject<decimal>;

public readonly partial record struct ProductId(Guid Value) : IValueObject<Guid>;

public readonly partial record struct ImageUrl(string Value) : IValueObject<string>;

public class Product
{
    public required ProductId Id { get; set; }
    public required Price Price { get; set; }
    public ImageUrl? ImageUrl { get; set; }
}

public readonly partial record struct OrderId(int Value) : IValueObject<int>;
public readonly partial record struct Quantity(int Value) : IValueObject<int>;

public class Order
{
    public required OrderId Id { get; set; }
    public required ProductId ProductId { get; set; }
    public required Quantity Quantity { get; set; }
    public required Price Price { get; set; }
    public Product Product { get; set; } = null!;
}

public class ProductDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("TestDb");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().ConfigureValueObjectProperties();
        modelBuilder.Entity<Order>().ConfigureValueObjectProperties();
    }
}