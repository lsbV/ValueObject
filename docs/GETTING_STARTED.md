# Getting Started with ValueObject

This guide walks you through the fundamentals of using the ValueObject source generator, from basic setup to practical real-world examples.

## Table of Contents

1. [Installation](#installation)
2. [Core Concepts](#core-concepts)
3. [Creating Your First Value Object](#creating-your-first-value-object)
4. [Generated Methods Overview](#generated-methods-overview)
5. [Common Patterns](#common-patterns)
6. [Best Practices](#best-practices)

## Installation

### Prerequisites

- .NET 10.0 or later
- C# 12 or later
- Visual Studio 2022, Rider, or VS Code

### Install the Package

```bash
dotnet add package ValueObject
```

Or manually add to your `.csproj`:

```xml
<ItemGroup>
    <PackageReference Include="ValueObject" Version="1.0.0" />
</ItemGroup>
```

## Core Concepts

### What is a Value Object?

A **value object** is a domain-driven design pattern that wraps a primitive or complex type to provide type safety and domain meaning. Instead of passing raw `string` or `Guid` around your codebase, you create domain-specific types like `UserId`, `Email`, or `ProductName`.

### Why Use Value Objects?

```csharp
// ❌ Without value objects - easy to mix up
public void UpdateUser(Guid userId, string name, string email)
{
    // Easy to accidentally swap parameters at call site!
    _userService.Update(email, userId, name); // Wrong parameter order!
}

// ✅ With value objects - type-safe
public void UpdateUser(UserId userId, UserName name, UserEmail email)
{
    // Compiler ensures correct types and order
    _userService.Update(email, userId, name); // Type error - caught at compile time!
}
```

### The IValueObject Interface

All value objects must implement the `IValueObject<T>` interface:

```csharp
namespace ValueObject.Core;

public interface IValueObject<TValue>
{
    public abstract TValue Value { get; init; }
}
```

This simple interface tells the source generator:
- Your type is a value object
- It wraps a value of type `TValue`
- The value is immutable (using `init`)

## Creating Your First Value Object

### Step 1: Declare a Partial Record

```csharp
using ValueObject.Core;

public partial record UserId(Guid Value) : IValueObject<Guid>;
```

Key points:
- Must be `partial` (generator adds code to this partial)
- Can be `record` or `record struct`
- Accepts the wrapped value as a primary constructor parameter
- Implements `IValueObject<T>` where `T` is the wrapped type

### Step 2: Use the Generated Methods

The generator automatically creates these methods:

```csharp
// Parsing from strings (for APIs, CLI, etc.)
if (UserId.TryParse("550e8400-e29b-41d4-a716-446655440000", out var userId))
{
    Console.WriteLine($"Parsed: {userId.Value}");
}

// Type conversions
Guid id = userId; // implicit conversion
var anotherUserId = (UserId)"550e8400-e29b-41d4-a716-446655440000"; // explicit conversion

// Comparisons
if (userId1 == userId2) { /* ... */ }
if (userId1 != userId2) { /* ... */ }
```

## Generated Methods Overview

The generator creates different methods based on the wrapped type. Here's what you get:

### All Value Objects Get

**Operators**
```csharp
// Implicit conversion to underlying type
public static implicit operator Guid(UserId v) => v.Value;

// Explicit conversion from underlying type
public static explicit operator UserId(Guid v) => new(v);

// Equality operators
public static bool operator ==(UserId left, Guid right) => left.Value == right;
public static bool operator !=(UserId left, Guid right) => left.Value != right;
public static bool operator ==(Guid left, UserId right) => left == right.Value;
public static bool operator !=(Guid left, UserId right) => left != right.Value;
```

**TryParse Methods**
```csharp
// Parse from string with format provider
public static bool TryParse(string? value, IFormatProvider? provider, out UserId result)

// Parse from string (uses null provider)
public static bool TryParse(string? value, out UserId result)
```

### Numeric Types Also Get

For `int`, `long`, `double`, `decimal`, etc.:

```csharp
// Comparison operators
public static bool operator <(Age left, Age right) => left.Value < right.Value;
public static bool operator <=(Age left, Age right) => left.Value <= right.Value;
public static bool operator >(Age left, Age right) => left.Value > right.Value;
public static bool operator >=(Age left, Age right) => left.Value >= right.Value;
```

### Database Integration (When Enabled)

**Entity Framework Core**
```csharp
public class UserIdValueConverter : ValueConverter<UserId, Guid>
{
    // Automatically converts between UserId and Guid in EF Core
}

public class UserIdNullableValueConverter : ValueConverter<UserId?, Guid?>
{
    // Handles nullable value objects
}

public static class ValueObjectEfCoreExtensions
{
    public static void ConfigureValueObjectProperties(this EntityTypeBuilder<User> entity)
    {
        entity.Property(e => e.UserId).HasConversion(new UserIdValueConverter());
    }
}
```

**MongoDB**
```csharp
public class UserIdSerializer : StructSerializerBase<UserId>
{
    // Custom BSON serialization for UserId
}

public static class MongoClassMaps
{
    public static void RegisterAll()
    {
        BsonSerializer.RegisterSerializer(new UserIdSerializer());
    }
}
```

## Common Patterns

### Pattern 1: Simple String-Wrapped Value

```csharp
public partial record Email(string Value) : IValueObject<string>;

// Usage
if (Email.TryParse(userInput, out var email))
{
    await _emailService.SendTo(email);
}
```

**Generated parsing**:
- Accepts any non-empty string
- No additional validation (add that in your domain layer if needed)

### Pattern 2: Numeric ID with Operators

```csharp
public partial record UserId(int Value) : IValueObject<int>;

// Usage - natural arithmetic
var id = new UserId(42);
if (id > 100) { /* ... */ }
if (id >= userId2) { /* ... */ }
```

**Generated parsing**:
- Uses `int.TryParse` with `NumberStyles.Any`
- Supports negative numbers
- Respects format provider for localization

### Pattern 3: GUID with Database Integration

```csharp
public partial record ProductId(Guid Value) : IValueObject<Guid>;

// Usage in Entity Framework
modelBuilder.Entity<Product>()
    .HasKey(p => p.ProductId)
    .Property(p => p.ProductId)
    .HasConversion(new ProductIdValueConverter());
```

**Generated parsing**:
- Uses `Guid.TryParse` with format provider
- Supports any standard Guid format
- Generates both regular and nullable converters for EF Core

### Pattern 4: MongoDB ObjectId

```csharp
using MongoDB.Bson;

public partial record EntityId(ObjectId Value) : IValueObject<ObjectId>;

// Usage in MongoDB
var collection = db.GetCollection<Entity>();
var entity = await collection.Find(e => e.EntityId == entityId).FirstOrDefaultAsync();
```

**Generated parsing**:
- Uses `ObjectId.Parse` with exception handling
- MongoDB serializer generated automatically
- No exceptions in TryParse—returns false on invalid input

## Common Patterns

### API Route Parameters (Minimal APIs)

```csharp
public partial record ProductId(Guid Value) : IValueObject<Guid>;
public partial record OrderQuantity(int Value) : IValueObject<int>;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Value objects are automatically parsed from route parameters!
app.MapGet("/products/{productId}/order/{quantity}", 
    (ProductId productId, OrderQuantity quantity) =>
    {
        // productId and quantity are already parsed and type-safe
        return Results.Ok(new { productId.Value, quantity.Value });
    });

app.Run();
```

**How it works**: 
ASP.NET Core calls `TryParse` to convert route parameters. The generated `TryParse` methods make your value objects compatible out of the box.

### Query String Parsing

```csharp
public partial record PageSize(int Value) : IValueObject<int>;
public partial record SortBy(string Value) : IValueObject<string>;

app.MapGet("/products", (HttpContext context) =>
{
    var queryParams = context.Request.Query;
    
    if (PageSize.TryParse(queryParams["pageSize"], out var pageSize))
    {
        // Use pageSize
    }
    
    if (SortBy.TryParse(queryParams["sortBy"], out var sortBy))
    {
        // Use sortBy
    }
});
```

### Comparison and Filtering

```csharp
public partial record Price(decimal Value) : IValueObject<decimal>;
public partial record Discount(decimal Value) : IValueObject<decimal>;

var prices = products.Select(p => p.Price).ToList();

// Natural comparison operators
var expensive = prices.Where(p => p > 100).ToList();
var discounted = prices.Where(p => p < 50).ToList();

// Comparison with primitives
if (price > 99.99m) { /* ... */ } // operator works both ways
```

### Type Conversions with Extensions

```csharp
public partial record UserId(Guid Value) : IValueObject<Guid>;

// The generator creates type extension classes like "GuidAs"
var userId = new UserId(Guid.NewGuid());

// Convert to underlying type
Guid id = userId; // implicit conversion

// Use with LINQ
var ids = userIds.Select(id => (Guid)id).ToList();
```

## Best Practices

### 1. Use Meaningful Type Names

```csharp
// ✅ Good - describes what the value represents
public partial record EmailAddress(string Value) : IValueObject<string>;
public partial record DrivingLicenseNumber(string Value) : IValueObject<string>;

// ❌ Poor - generic names lose domain meaning
public partial record StringValue(string Value) : IValueObject<string>;
public partial record Text(string Value) : IValueObject<string>;
```

### 2. Keep Value Objects Immutable

```csharp
// ✅ Good - using init
public partial record Age(int Value) : IValueObject<int>;

// ❌ Bad - using set makes it mutable
public partial record Age(int Value) { set; } // Don't do this!
```

### 3. Use Structs for Small Values

```csharp
// ✅ Good - struct for small types saves allocations
public partial record struct UserId(Guid Value) : IValueObject<Guid>;
public partial record struct Quantity(int Value) : IValueObject<int>;

// Acceptable - record for larger or more complex types
public partial record FullAddress(string Street, string City, string Country) : IValueObject<string>;
```

### 4. Extend with Domain Logic in Separate Files

```csharp
// Id.cs - auto-generated by source generator
public partial record UserId(Guid Value) : IValueObject<Guid>;

// Id.Extensions.cs - your custom logic
public partial record UserId
{
    public bool IsEmpty => Value == Guid.Empty;
    
    public bool IsValid => Value != Guid.Empty;
}
```

### 5. Add Validation in Constructor or Factory Methods

```csharp
public partial record Age(int Value) : IValueObject<int>
{
    public Age(int value) : this(ValidateAge(value))
    {
    }

    private static int ValidateAge(int age)
    {
        if (age < 0 || age > 150)
            throw new ArgumentException("Age must be between 0 and 150");
        return age;
    }
}

// Or use factory method
public partial record Email(string Value) : IValueObject<string>;

public static partial class Email
{
    public static Email CreateOrThrow(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty");
        if (!value.Contains("@"))
            throw new ArgumentException("Email must contain @");
        return new Email(value);
    }
}
```

### 6. Combine with Records for Strongly-Typed Collections

```csharp
public partial record UserId(Guid Value) : IValueObject<Guid>;
public partial record ProductId(Guid Value) : IValueObject<Guid>;

// Return specific types, not raw collections
public IEnumerable<UserId> GetActiveUsers() => /* ... */;
public IEnumerable<ProductId> GetPopularProducts() => /* ... */;

// Much better than
public IEnumerable<Guid> GetIds() => /* ... */; // What do these IDs represent?
```

## Next Steps

- **[TryParse Emitter](./TRYPARSE_EMITTER.md)** - Deep dive into parsing
- **[Operators Emitter](./OPERATORS_EMITTER.md)** - Full operator reference
- **[Entity Framework Core](./ENTITY_FRAMEWORK.md)** - Database integration
- **[MongoDB Integration](./MONGODB.md)** - Document database support
- **[Configuration](./CONFIGURATION.md)** - Customizing generator behavior

