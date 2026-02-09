# Configuration

## Overview

The ValueObject generator works out-of-the-box with sensible defaults, but you can customize behavior using the `ValueObjectSettings` assembly attribute.

## Default Behavior

By default, the generator:
- ✅ Generates `TryParse` methods (always)
- ✅ Generates operators and conversions (always)
- ✅ Generates type extension classes (always)
- ✅ Generates Entity Framework Core value converters
- ✅ Generates MongoDB BSON serializers

## Customization

Use the `ValueObjectSettingsAttribute` to control generation:

```csharp
[assembly: ValueObjectSettings(
    generateMongoDbSerializer: true,           // Enable/disable MongoDB
    generateEfCoreValueConverter: true         // Enable/disable EF Core
)]
```

Place this in:
- `Program.cs`
- `AssemblyInfo.cs`
- Any `.cs` file in your project

### Disable MongoDB Serializers

```csharp
[assembly: ValueObjectSettings(
    generateMongoDbSerializer: false,
    generateEfCoreValueConverter: true
)]
```

**Use when:**
- You don't use MongoDB in your application
- You want to reduce generated code size
- You only use EF Core or SQL databases

**What's disabled:**
- Individual value object serializers (e.g., `UserIdSerializer`)
- `MongoClassMaps.RegisterAll()` registration helper

### Disable Entity Framework Core Converters

```csharp
[assembly: ValueObjectSettings(
    generateMongoDbSerializer: true,
    generateEfCoreValueConverter: false
)]
```

**Use when:**
- You don't use Entity Framework Core
- You don't use .NET ORM (using raw SQL or other data access)
- You only use MongoDB or other non-relational database

**What's disabled:**
- Value converter classes (e.g., `UserIdValueConverter`)
- Nullable value converter classes (e.g., `UserIdNullableValueConverter`)
- `ConfigureValueObjectProperties()` extension methods

### Disable Both

```csharp
[assembly: ValueObjectSettings(
    generateMongoDbSerializer: false,
    generateEfCoreValueConverter: false
)]
```

**Use when:**
- You only need basic operators, conversions, and parsing
- You manage database mapping manually
- Minimal generated code is a priority

**Still generated:**
- `TryParse` methods
- Operator overloads (`==`, `!=`, `<`, `>`, etc.)
- Implicit/explicit conversions
- Type extension classes (`.As()` pattern)

## Example Configurations

### Minimal Web API

```csharp
// Program.cs
[assembly: ValueObjectSettings(
    generateMongoDbSerializer: false,
    generateEfCoreValueConverter: true
)]

var builder = WebApplication.CreateBuilder(args);

// Add EF Core
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();

// Value objects work automatically in minimal APIs
app.MapGet("/users/{userId}", (UserId userId) => Results.Ok(userId.Value));

app.Run();
```

### MongoDB-Only Application

```csharp
// Program.cs
[assembly: ValueObjectSettings(
    generateMongoDbSerializer: true,
    generateEfCoreValueConverter: false
)]

using ValueObject.Core;

var builder = WebApplication.CreateBuilder(args);

// Register MongoDB serializers
MongoClassMaps.RegisterAll();

var app = builder.Build();
app.Run();
```

### CLI Application

```csharp
// Program.cs
[assembly: ValueObjectSettings(
    generateMongoDbSerializer: false,
    generateEfCoreValueConverter: false
)]

using ValueObject.Core;

// Parse command-line arguments
if (Age.TryParse(args[0], out var age))
{
    Console.WriteLine($"Age: {age.Value}");
}
else
{
    Console.WriteLine("Invalid age");
}
```

### Library/Shared Package

```csharp
// AssemblyInfo.cs
[assembly: ValueObjectSettings(
    generateMongoDbSerializer: true,
    generateEfCoreValueConverter: true
)]

namespace MySharedLibrary
{
    // Define domain value objects
    public partial record UserId(Guid Value) : IValueObject<Guid>;
    public partial record Email(string Value) : IValueObject<string>;
}
```

Since libraries are used in different contexts (some with EF Core, some with MongoDB, some with both), enable both by default in shared packages.

## Changing Configuration

If you need to change the configuration:

1. **Remove the old attribute** (if it exists)
2. **Add the new attribute** in any `.cs` file
3. **Rebuild** the project

The generator will regenerate all code files with the new settings.

## When Settings Don't Apply

These settings only affect database integration generators:
- `TryParse` methods are **always** generated (can't be disabled)
- Operators and conversions are **always** generated (can't be disabled)
- Type extensions are **always** generated (can't be disabled)

## Verifying Configuration

Check the generated files to verify your settings are applied:

Look for `obj/Debug/net.../generated/ValueObject.SourceGenerator/`:
- `*_ValueConverters.g.cs` files → EF Core is enabled
- `*_MongoDbSerializer.g.cs` files → MongoDB is enabled
- `*_TryParse.g.cs` files → Always present
- `*_Operators.g.cs` files → Always present

## Project-Specific Configuration

You can have different settings in different projects:

```
Solution/
├── MyAPI.csproj              (EF Core only)
│   └── Program.cs
│       └── [assembly: ValueObjectSettings(false, true)]
│
├── MyMobileApp.csproj        (MongoDB only)
│   └── Program.cs
│       └── [assembly: ValueObjectSettings(true, false)]
│
└── MyShared.csproj           (Both)
    └── AssemblyInfo.cs
        └── [assembly: ValueObjectSettings(true, true)]
```

## Performance Impact of Settings

### With Full Configuration (Default)

- Generated code: ~50 lines per value object (includes all generators)
- Compile time: Slightly longer
- Runtime: No difference (generators are compile-time only)

### Disabled MongoDB

- Saves ~15 lines per value object
- Removes MongoDB dependencies from generated code
- No impact on API or EF Core functionality

### Disabled EF Core

- Saves ~20 lines per value object
- Removes Microsoft.EntityFrameworkCore dependencies from generated code
- No impact on API or MongoDB functionality

### Minimal (Both Disabled)

- Smallest generated code footprint
- ~30 lines per value object
- Fastest compilation
- Still full feature support for parsing, operators, conversions

## Troubleshooting

### Configuration Attribute Not Found

**Error:** "ValueObjectSettingsAttribute is not defined"

**Solution:** Make sure you have `using ValueObject.Core;` at the top of the file where you declare the attribute.

```csharp
using ValueObject.Core;

[assembly: ValueObjectSettings(true, true)]
```

### Changes Not Taking Effect

**Problem:** You changed the configuration but generated files didn't update

**Solution:**
1. Clean the project: `dotnet clean`
2. Delete the `obj` folder
3. Rebuild: `dotnet build`
4. Check if the generated files in `obj/Debug/net.../generated/` reflect the new settings

### Too Much Generated Code

If you have many value objects and are concerned about generated code size:

```csharp
[assembly: ValueObjectSettings(
    generateMongoDbSerializer: false,
    generateEfCoreValueConverter: false
)]
```

Only the core generators (TryParse, operators, conversions) will run.

## Next Steps

- **[Getting Started](./GETTING_STARTED.md)** - Basic usage guide
- **[Entity Framework Core](./ENTITY_FRAMEWORK.md)** - EF Core-specific setup
- **[MongoDB Integration](./MONGODB.md)** - MongoDB-specific setup
- **[TryParse Emitter](./TRYPARSE_EMITTER.md)** - Parsing methods

