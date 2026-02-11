using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ValueObject.Core;
using ValueObjectTests.Fixtures;

namespace ValueObjectTests;

public class EntityFrameworkTests(SqlServerFixture fixture) : IClassFixture<SqlServerFixture>, IAsyncLifetime
{
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;

    private readonly ProductDbContext _context = new(fixture.Options);
    [Fact]
    public async Task Should_Save_And_Retrieve_Entity_With_ValueObjects()
    {
        // Arrange
        var id = new ProductId(Guid.NewGuid());
        var product = new Product
        {
            Id = id,
            Price = new Price(19.99m),
            ImageUrl = new ImageUrl("https://example.com/image.jpg"),
            Orders = [new Order
                {
                    ProductId = id,
                    Quantity = new Quantity(10),
                    Price = new Price(199.90m),
                    Product = null!,
                }
            ]
        };
        _context.Products.Add(product);
        
        // Act
        await _context.SaveChangesAsync(_ct);
        var retrievedProduct = await _context.Products
            .Include(p => p.Orders)
            .FirstAsync(p => p.Id == product.Id, _ct);
        
        // Assert
        Assert.NotNull(retrievedProduct);
        Assert.Equal(product.Id, retrievedProduct.Id);
        Assert.Equal(product.Price, retrievedProduct.Price);
        Assert.Equal(product.ImageUrl, retrievedProduct.ImageUrl);
        Assert.Single(retrievedProduct.Orders);
    }

    [Fact]
    public async Task EfModel_schema_matches_snapshot()
    {
        // Arrange & Act
        var snapshot = BuildSchemaSnapshot(_context.Model);

        var actualJson = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        var expectedJson = await File.ReadAllTextAsync("schema.snapshot.json", _ct);

        // Assert
        Assert.Equal(Normalize(expectedJson), Normalize(actualJson));
    }

    private static object BuildSchemaSnapshot(IModel model)
    {
        var entities = model.GetEntityTypes()
            .OrderBy(e => e.GetTableName())
            .Select(e => new
            {
                Table = e.GetTableName(),
                Schema = e.GetSchema(),
                ClrType = e.ClrType?.FullName,
                Columns = e.GetProperties()
                    .OrderBy(p => p.GetColumnName())
                    .Select(p => new
                    {
                        Column = p.GetColumnName(),
                        Type = p.ClrType.FullName,
                        Nullable = p.IsNullable,
                        MaxLength = p.GetMaxLength(),
                        Precision = p.GetPrecision(),
                        Scale = p.GetScale(),
                        Unicode = p.IsUnicode(),
                    }),
                PrimaryKey = e.FindPrimaryKey()?.Properties
                    .Select(p => p.GetColumnName())
                    .OrderBy(x => x),
                Indexes = e.GetIndexes()
                    .OrderBy(i => i.GetDatabaseName())
                    .Select(i => new
                    {
                        Name = i.GetDatabaseName(),
                        Unique = i.IsUnique,
                        Columns = i.Properties.Select(p => p.GetColumnName()).OrderBy(x => x)
                    }),
                ForeignKeys = e.GetForeignKeys()
                    .OrderBy(fk => fk.GetConstraintName())
                    .Select(fk => new
                    {
                        Name = fk.GetConstraintName(),
                        Columns = fk.Properties.Select(p => p.GetColumnName()).OrderBy(x => x),
                        PrincipalTable = fk.PrincipalEntityType.GetTableName(),
                        PrincipalColumns = fk.PrincipalKey.Properties.Select(p => p.GetColumnName()).OrderBy(x => x),
                        DeleteBehavior = fk.DeleteBehavior.ToString()
                    })
            });

        return new { Entities = entities };
    }

    private static string Normalize(string s) =>
        s.Replace("\r\n", "\n").Trim();

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
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
    public List<Order> Orders { get; set; } = [];
}

public readonly partial record struct OrderId(int Value) : IValueObject<int>;

public readonly partial record struct Quantity(int Value) : IValueObject<int>;

public class Order
{
    public OrderId Id { get; set; }
    public required ProductId ProductId { get; set; }
    public required Quantity Quantity { get; set; }
    public required Price Price { get; set; }
    public Product Product { get; set; } = null!;
}

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().ConfigureValueObjectProperties();
        modelBuilder.Entity<Order>().ConfigureValueObjectProperties();

        modelBuilder.Entity<Product>().Property(x => x.ImageUrl).HasMaxLength(128);
    }
}