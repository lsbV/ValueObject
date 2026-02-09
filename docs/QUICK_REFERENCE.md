# ValueObject Quick Reference

Fast lookup for common tasks and syntax.

## Installation

```bash
dotnet add package ValueObject
dotnet add package Microsoft.EntityFrameworkCore  # For EF Core
dotnet add package MongoDB.Driver                 # For MongoDB
```

## Basic Syntax

### Define a Value Object

```csharp
using ValueObject.Core;

// String-based
public partial record Email(string Value) : IValueObject<string>;

// Numeric
public partial record Age(int Value) : IValueObject<int>;

// GUID
public partial record UserId(Guid Value) : IValueObject<Guid>;

// MongoDB ObjectId
public partial record EntityId(ObjectId Value) : IValueObject<ObjectId>;

// Struct (stack-allocated)
public partial record struct Quantity(int Value) : IValueObject<int>;
```

## Generated Methods

### TryParse

```csharp
// With format provider
if (Age.TryParse("25", System.Globalization.CultureInfo.InvariantCulture, out var age))
{
    Console.WriteLine(age.Value);
}

// Without provider
if (Age.TryParse("25", out var age))
{
    Console.WriteLine(age.Value);
}
```

### Operators

```csharp
// Conversions
Guid id = userId;                      // implicit to underlying type
UserId userId2 = (UserId)id;          // explicit from underlying type

// Comparisons
if (age1 == age2) { }                 // value objects
if (age1 > 18) { }                    // value object vs primitive
if (age1 < age2) { }                  // numeric types only

// Equality
if (userId1 != userId2) { }
```

### Type Extensions (As Pattern)

```csharp
var str = "john@example.com";

// Access all string-based value objects
var email = str.As().Email;
var name = str.As().UserName;

// Same for other types
var guid = Guid.NewGuid();
var userId = guid.As().UserId;
var productId = guid.As().ProductId;
```

## ASP.NET Core Integration

### Minimal APIs

```csharp
app.MapGet("/users/{userId}", (UserId userId) => 
{
    return Results.Ok(userId.Value);
});

app.MapPost("/products/{productId}/quantity/{quantity}",
    (ProductId productId, Quantity quantity) =>
    {
        return Results.Ok(new { productId.Value, quantity.Value });
    });
```

### Model Binding

```csharp
[HttpPost]
public IActionResult CreateOrder([FromRoute] OrderId orderId, [FromBody] CreateOrderRequest request)
{
    // orderId is automatically parsed
    return Ok();
}
```

## Entity Framework Core

### Configuration

```csharp
public class AppDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .Property(o => o.OrderId)
            .HasConversion(new OrderIdValueConverter());

        // Or use the generated extension
        modelBuilder.Entity<Order>()
            .ConfigureValueObjectProperties();
    }
}
```

### Querying

```csharp
var orders = await context.Orders
    .Where(o => o.CustomerId == customerId)
    .Where(o => o.TotalPrice > 100)
    .ToListAsync();
```

## MongoDB

### Setup

```csharp
using ValueObject.Core;

// Register serializers at startup
MongoClassMaps.RegisterAll();

var client = new MongoClient("mongodb://localhost:27017");
var db = client.GetDatabase("myapp");
```

### Usage

```csharp
var collection = db.GetCollection<Order>("orders");

// Insert
var order = new Order
{
    OrderId = new OrderId(ObjectId.GenerateNewId()),
    CustomerId = new CustomerId(ObjectId.Parse("507f1f77bcf86cd799439011")),
};
await collection.InsertOneAsync(order);

// Query
var filter = Builders<Order>.Filter.Eq(o => o.CustomerId, customerId);
var order = await collection.Find(filter).FirstOrDefaultAsync();

// Update
var update = Builders<Order>.Update.Set(o => o.Status, OrderStatus.Shipped);
await collection.UpdateOneAsync(filter, update);
```

## Configuration

### Enable/Disable Features

```csharp
[assembly: ValueObjectSettings(
    generateMongoDbSerializer: true,
    generateEfCoreValueConverter: true
)]
```

Options:
- `true, true` - All features (default)
- `true, false` - MongoDB only
- `false, true` - EF Core only
- `false, false` - Minimal (parsing, operators only)

## Type Support

| Type | TryParse | Operators | Comparison | EF Core | MongoDB |
|------|----------|-----------|-----------|---------|---------|
| string | ✅ | ✅ | ❌ | ✅ | ✅ |
| int, long, short, etc. | ✅ | ✅ | ✅ | ✅ | ✅ |
| float, double, decimal | ✅ | ✅ | ✅ | ✅ | ✅ |
| bool | ✅ | ✅ | ❌ | ✅ | ✅ |
| Guid | ✅ | ✅ | ❌ | ✅ | ✅ |
| DateTime | ✅ | ✅ | ❌ | ✅ | ✅ |
| DateOnly | ✅ | ✅ | ❌ | ✅ | ❌ |
| ObjectId | ✅ | ✅ | ❌ | ✅ | ✅ |

## Common Patterns

### Validation

```csharp
public partial record Age(int Value) : IValueObject<int>
{
    public Age(int value) : this(ValidateAge(value)) { }

    private static int ValidateAge(int age)
    {
        if (age < 0 || age > 150)
            throw new ArgumentException("Invalid age");
        return age;
    }
}
```

### Factory Method

```csharp
public partial record Email(string Value) : IValueObject<string>;

public static partial class Email
{
    public static Email Create(string value)
    {
        if (!value.Contains("@"))
            throw new ArgumentException("Invalid email");
        return new Email(value);
    }
}
```

### Parsing from Configuration

```csharp
if (Port.TryParse(config["Server:Port"], out var port))
{
    builder.WebHost.UseUrls($"http://localhost:{port.Value}");
}
```

### Filtering

```csharp
var expensiveProducts = products
    .Where(p => p.Price > 100)
    .Where(p => p.StockQuantity >= 10);
```

## Best Practices

✅ Use meaningful names: `UserId`, not `StringValue`
✅ Use `record struct` for small types (stack-allocated)
✅ Use `record` for larger or complex types
✅ Keep value objects immutable (use `init`)
✅ Add validation in constructor
✅ Use factory methods for complex creation
✅ Extend in separate partial files
✅ Group value objects by domain concept

❌ Don't mutate value objects
❌ Don't nest complex logic in value objects
❌ Don't use generic names
❌ Don't repeat patterns across projects

## Troubleshooting

### Generated Code Not Appearing

```bash
# Clean and rebuild
dotnet clean
rm -rf obj
dotnet build
```

### TryParse Not Working

Verify:
- Value object has `partial` keyword
- Implements `IValueObject<T>`
- Type is imported: `using ValueObject.Core;`

### EF Core Converters Not Found

Verify:
- Configuration has `generateEfCoreValueConverter: true`
- Using correct converter class name: `{TypeName}ValueConverter`
- Project references `Microsoft.EntityFrameworkCore`

### MongoDB Serializers Not Registering

Verify:
- Called `MongoClassMaps.RegisterAll()` before using MongoDB
- Configuration has `generateMongoDbSerializer: true`
- Project references `MongoDB.Driver`

## File Locations

Generated files are in: `obj/Debug/net{version}/generated/ValueObject.SourceGenerator/`

File naming:
- `{TypeName}_TryParse.g.cs` - Parsing methods
- `{TypeName}_Operators.g.cs` - Operators
- `{TypeName}_ValueConverters.g.cs` - EF Core
- `{TypeName}_MongoDbSerializer.g.cs` - MongoDB
- `{SimpleType}As.g.cs` - Type extensions

## Documentation

- **User Guide**: [GETTING_STARTED.md](./docs/GETTING_STARTED.md)
- **Features**: See [docs/](./docs/) folder
- **Contributing**: [CONTRIBUTING.md](./docs/contribute/CONTRIBUTING.md)
- **Architecture**: [ARCHITECTURE.md](./docs/contribute/ARCHITECTURE.md)

## Links

- **GitHub**: https://github.com/yourusername/ValueObject
- **NuGet**: https://www.nuget.org/packages/ValueObject
- **Issues**: https://github.com/yourusername/ValueObject/issues
- **Docs**: See [INDEX.md](./docs/INDEX.md)

