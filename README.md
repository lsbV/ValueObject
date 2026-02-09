# ValueObject

A powerful C# source generator that automates the creation of utility methods for value objects. Transform simple value object declarations into fully-featured types with operators, type conversions, parsing capabilities, and database integration support.

## ✨ Features

- **TryParse Methods**: Automatically generate safe parsing methods for minimal APIs and CLI applications
- **Operator Overloading**: Implicit/explicit conversions and comparison operators
- **Type Casting Extensions**: Convenient type conversion extensions with the `As` pattern
- **Entity Framework Core Integration**: Automatic value converters and model builder extensions
- **MongoDB Support**: Built-in serializers and class maps for MongoDB BSON serialization
- **Zero Configuration**: Smart defaults with optional per-assembly customization

## 🚀 Quick Start

### 1. Install the Package

```bash
dotnet add package ValueObject
```

### 2. Define Your Value Object

```csharp
using ValueObject.Core;

public partial record UserId(Guid Value) : IValueObject<Guid>;

public partial record Email(string Value) : IValueObject<string>;

public partial record Age(int Value) : IValueObject<int>;
```

### 3. Use the Generated Methods

```csharp
// TryParse - safe parsing
if (Age.TryParse("25", out var age))
{
    Console.WriteLine($"Age: {age.Value}"); // Output: Age: 25
}

// Operators - natural comparisons
var userId1 = new UserId(Guid.Parse("..."));
var userId2 = userId1;
if (userId1 == userId2) { /* ... */ }

// Implicit conversion to underlying type
Guid id = userId1; // implicitly converts UserId to Guid

// Type extensions - convenient casting
var users = userIds.Select(id => id.As().Guid); // using StringAs extensions
```

### 4. Use in ASP.NET Core Minimal APIs

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Value objects are automatically parsed from route parameters
app.MapGet("/users/{userId}/details", (UserId userId) => 
{
    return Results.Ok($"Getting user: {userId.Value}");
});

app.Run();
```

## 📚 Documentation

- **[Getting Started Guide](./docs/GETTING_STARTED.md)** - Comprehensive introduction and common patterns
- **[TryParse Emitter](./docs/TRYPARSE_EMITTER.md)** - Parser generation for safe type conversion
- **[Operators Emitter](./docs/OPERATORS_EMITTER.md)** - Operator overloading and conversions
- **[Type Extensions](./docs/TYPE_EXTENSIONS.md)** - The `As` pattern for type conversions
- **[Entity Framework Core](./docs/ENTITY_FRAMEWORK.md)** - EF Core integration and configuration
- **[MongoDB Integration](./docs/MONGODB.md)** - MongoDB serialization support
- **[Configuration](./docs/CONFIGURATION.md)** - Customizing generator behavior

### For Contributors

- **[Architecture Overview](./docs/contribute/ARCHITECTURE.md)** - How the source generator works
- **[Emitter Development](./docs/contribute/EMITTER_DEVELOPMENT.md)** - Creating new emitters
- **[Testing Guide](./docs/contribute/TESTING.md)** - Contributing tests

## 🎯 Why Value Objects?

Value objects provide:
- **Type Safety**: Compile-time guarantees that values are correct types
- **Domain Clarity**: Self-documenting code that expresses intent
- **Reusability**: Share validation logic across your application
- **Maintainability**: Changes centralized in value object definitions

## 🔄 How It Works

ValueObject is a .NET incremental source generator that:

1. **Discovers** all `record` declarations implementing `IValueObject<T>`
2. **Analyzes** the underlying value type `T`
3. **Generates** optimized methods tailored to each type
4. **Emits** zero-cost abstractions at compile time

No runtime overhead, no reflection—just generated code.

## 💡 Example: E-Commerce Domain

```csharp
// Define your domain value objects
public partial record ProductId(Guid Value) : IValueObject<Guid>;
public partial record Price(decimal Value) : IValueObject<decimal>;
public partial record Quantity(int Value) : IValueObject<int>;
public partial record Email(string Value) : IValueObject<string>;

// Automatically get:
// ✓ TryParse methods for parsing from strings
// ✓ Operators for natural syntax (price > 100)
// ✓ EF Core value converters for database mapping
// ✓ MongoDB serializers for document storage
// ✓ Type extensions for safe casting

// In your API
app.MapPost("/orders/{productId}/{quantity}", 
    (ProductId productId, Quantity quantity, Price pricePerUnit) =>
{
    if (pricePerUnit > 1000) // operator comparison
        return Results.BadRequest();
    
    var total = quantity.Value * pricePerUnit.Value; // easy arithmetic
    return Results.Ok(new { productId, total });
});
```

## 🛠️ Configuration

Control generation with assembly-level settings:

```csharp
// Program.cs or AssemblyInfo.cs
[assembly: ValueObjectSettings(
    generateMongoDbSerializer: true,
    generateEfCoreValueConverter: true
)]
```

## 📖 Supported Types

ValueObject supports all common .NET types:
- **Primitives**: `int`, `long`, `float`, `double`, `decimal`, `bool`, `byte`
- **Strings**: `string`
- **Types**: `Guid`, `DateTime`, `DateOnly`, `TimeOnly`
- **MongoDB**: `MongoDB.Bson.ObjectId`
- **Custom**: Any type with `TryParse(string, IFormatProvider, out T)`

## 🤝 Contributing

Contributions are welcome! See [CONTRIBUTING.md](./docs/contribute/CONTRIBUTING.md) for guidelines.

## 📄 License

See [LICENSE.txt](./LICENSE.txt) for details.

---

**Made with ❤️ for developers who care about type safety and clean code**
