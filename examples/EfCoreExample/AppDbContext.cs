using Microsoft.EntityFrameworkCore;
using ValueObject.Core;

namespace EfCoreExample;

/// <summary>
/// Example DbContext with a Product entity using ValueObjects
/// </summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>
    /// Example DbContext with a Product entity using ValueObjects
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { 
        Database.EnsureCreated();
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().ConfigureValueObjectProperties();
    }
}

// Value Objects
public readonly partial record struct ProductId(Guid Value) : IValueObject<Guid>;
public readonly partial record struct ProductName(string Value) : IValueObject<string>;
public readonly partial record struct Price(decimal Value) : IValueObject<decimal>;

// Entities
public class Product
{
    public required ProductId Id { get; set; }
    public required ProductName Name { get; set; }
    public required Price Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


