# TryParse Emitter

## Overview

The TryParseEmitter is a source generator that automatically generates `TryParse` methods for value objects, enabling them to be used as route parameters in ASP.NET Core minimal APIs.

## Generated Code Example

For a value object like:

```csharp
public partial record Age(int Value) : IValueObject<int>;
```

The following methods are automatically generated:

```csharp
#nullable enable
// <auto‑generated />
namespace ValueObjectTests;

public partial record Age
{
    public static bool TryParse(string? value, IFormatProvider? provider, out Age result)
    {
        result = default!;
        if (string.IsNullOrEmpty(value))
            return false;

        // Try TryParse(string, NumberStyles, IFormatProvider, out T)
        if (int.TryParse(value, System.Globalization.NumberStyles.Any, provider, out var parsedValue))
        {
            result = new(parsedValue);
            return true;
        }

        return false;
    }

    public static bool TryParse(string? value, out Age result)
    {
        return TryParse(value, null, out result);
    }
}
```

## Usage Examples

### Basic Parsing

```csharp
// Parse from string
if (Age.TryParse("25", out var age))
{
    Console.WriteLine($"Age: {age.Value}"); // Output: Age: 25
}

// Parse with format provider
if (Price.TryParse("19.99", CultureInfo.InvariantCulture, out var price))
{
    Console.WriteLine($"Price: {price.Value}"); // Output: Price: 19.99
}
```

### Minimal API Usage

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Value objects are automatically parsed from route parameters
app.MapGet("/products/{productId}", (ProductId productId) => 
{
    // productId is automatically parsed from the route
    return Results.Ok($"Product: {productId.Value}");
});

app.MapPost("/users/{userId}/age/{age}", (UserId userId, Age age) =>
{
    // Both userId and age are automatically parsed
    return Results.Ok($"User {userId.Value} is {age.Value} years old");
});

app.Run();
```

### Query Parameter Parsing

```csharp
// Parse from query strings
if (Length.TryParse(HttpContext.Request.Query["length"], out var length))
{
    // Process the parsed length value
}
```

## Supported Types

The emitter automatically handles the following underlying types:

- `string`
- `int`
- `double`
- `decimal`
- `Guid`
- `ObjectId` (MongoDB.Bson)

## Method Signatures

```csharp
// Full signature with format provider
public static bool TryParse(string? value, IFormatProvider? provider, out ValueObjectType result)

// Convenience overload without provider
public static bool TryParse(string? value, out ValueObjectType result)
```

Return Values:
- `true` - Successfully parsed the value
- `false` - Failed to parse (null/empty input, invalid format)


