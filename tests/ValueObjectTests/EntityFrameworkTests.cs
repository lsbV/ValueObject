using Microsoft.EntityFrameworkCore;
using ValueObject.Core;

namespace ValueObjectTests;

public class EntityFrameworkTests
{
    private readonly ProductDbContext context = new();
    private readonly CancellationToken ct = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Should_Save_And_Retrieve_Entity_With_ValueObjects()
    {
        var product = new Product
        {
            Id = new ProductId(Guid.NewGuid()),
            Price = new Price(19.99m),
            ImageUrl = new ImageUrl("https://example.com/image.jpg")
        };
        context.Products.Add(product);
        await context.SaveChangesAsync(ct);
        var retrievedProduct = await context.Products.FirstAsync(p => p.Id == product.Id, ct);
        Assert.NotNull(retrievedProduct);
        Assert.Equal(product.Id, retrievedProduct!.Id);
        Assert.Equal(product.Price, retrievedProduct.Price);
        Assert.Equal(product.ImageUrl, retrievedProduct.ImageUrl);
    }
}

public partial record Price(decimal Value) : IValueObject<decimal>;

public partial record ProductId(Guid Value) : IValueObject<Guid>;

public partial record ImageUrl(string Value) : IValueObject<string>;

public class Product
{
    public required ProductId Id { get; set; }
    public required Price Price { get; set; }
    public ImageUrl? ImageUrl { get; set; }
}

public class ProductDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("TestDb");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().ConfigureValueObjectProperties();
    }
}