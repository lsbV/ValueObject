# Testing Guide

This guide explains how to test the ValueObject source generator and its emitters.

## Testing Strategy

The project uses XUnit for testing with snapshot-based and assertion-based approaches:

- **Unit Tests**: Test individual emitters
- **Integration Tests**: Test full generation pipeline
- **Snapshot Tests**: Verify generated code content

## Test Project Structure

```
tests/ValueObjectTests/
├── AssemblyInfo.cs              // Test value object declarations
├── TryParseTests.cs             // TryParse emitter tests
├── ConverterTests.cs            // EF Core converter tests
├── MongoDbSerializerTests.cs    // MongoDB serializer tests
├── OperatorsTests.cs            // Operators emitter tests
├── EntityFrameworkTests.cs      // EF Core integration tests
└── xunit.runner.json            // XUnit configuration
```

## Test Value Objects

Test value objects are defined in the test project itself:

```csharp
// tests/ValueObjectTests/AssemblyInfo.cs
using ValueObject.Core;

[assembly: ValueObjectSettings(generateMongoDbSerializer: true, generateEfCoreValueConverter: true)]

// String-based
public partial record Name(string Value) : IValueObject<string>;
public partial record Email(string Value) : IValueObject<string>;
public partial record ImageUrl(string Value) : IValueObject<string>;

// Guid-based
public partial record UserId(Guid Value) : IValueObject<Guid>;

// Numeric
public partial record Age(int Value) : IValueObject<int>;

// MongoDB
public partial record EntityId(MongoDB.Bson.ObjectId Value) : IValueObject<MongoDB.Bson.ObjectId>;
```

These are declared in the test project so the source generator processes them during test compilation.

## Writing TryParse Tests

### Test Structure

```csharp
[Fact]
public void Name_TryParse_Should_Parse_Valid_String()
{
    const string testValue = "John Doe";
    var result = Name.TryParse(testValue, out var name);
    
    Assert.True(result);
    Assert.Equal(testValue, name.Value);
}

[Fact]
public void Name_TryParse_Should_Return_False_For_Null()
{
    var result = Name.TryParse(null, out var name);
    Assert.False(result);
}
```

### Sections to Test

```csharp
#region String Type Tests
[Fact]
public void String_TryParse_Valid() { }

[Fact]
public void String_TryParse_Empty() { }

[Fact]
public void String_TryParse_Null() { }
#endregion

#region Numeric Type Tests
[Fact]
public void Numeric_TryParse_Valid() { }

[Fact]
public void Numeric_TryParse_Invalid() { }

[Fact]
public void Numeric_TryParse_Negative() { }

[Fact]
public void Numeric_TryParse_With_Provider() { }
#endregion

#region Guid Type Tests
[Fact]
public void Guid_TryParse_Valid() { }

[Fact]
public void Guid_TryParse_Multiple_Formats() { }
#endregion

#region ObjectId Type Tests
[Fact]
public void ObjectId_TryParse_Valid() { }

[Fact]
public void ObjectId_TryParse_Invalid() { }
#endregion
```

## Writing Operators Tests

```csharp
public class OperatorsTests
{
    [Fact]
    public void UserId_Implicit_Conversion_To_Guid()
    {
        var userId = new UserId(Guid.NewGuid());
        Guid guidValue = userId;
        
        Assert.Equal(userId.Value, guidValue);
    }

    [Fact]
    public void UserId_Explicit_Conversion_From_Guid()
    {
        var guid = Guid.NewGuid();
        UserId userId = (UserId)guid;
        
        Assert.Equal(guid, userId.Value);
    }

    [Fact]
    public void Age_Comparison_Operators()
    {
        var age1 = new Age(25);
        var age2 = new Age(30);
        
        Assert.True(age1 < age2);
        Assert.True(age1 <= age2);
        Assert.False(age1 > age2);
        Assert.True(age1 != age2);
    }

    [Fact]
    public void Age_Comparison_With_Primitive()
    {
        var age = new Age(25);
        
        Assert.True(age < 30);
        Assert.False(age > 30);
        Assert.True(age == 25);
    }
}
```

## Writing Entity Framework Tests

```csharp
public class EntityFrameworkTests
{
    [Fact]
    public void ValueConverter_Should_Convert_ValueObject_To_Primitive()
    {
        var age = new Age(30);
        var converter = new AgeValueConverter();
        
        var compiled = converter.ConvertToProviderExpression.Compile();
        int result = compiled(age);
        
        Assert.Equal(30, result);
    }

    [Fact]
    public void ValueConverter_Should_Convert_Primitive_To_ValueObject()
    {
        var converter = new AgeValueConverter();
        var compiled = converter.ConvertFromProviderExpression.Compile();
        
        var result = compiled(30);
        
        Assert.Equal(30, result.Value);
    }

    [Fact]
    public void NullableValueConverter_Should_Handle_Null()
    {
        Age? age = null;
        var converter = new AgeNullableValueConverter();
        
        var compiled = converter.ConvertToProviderExpression.Compile();
        int? result = compiled(age);
        
        Assert.Null(result);
    }
}
```

## Writing MongoDB Tests

```csharp
public class MongoDbSerializerTests
{
    [Fact]
    public void Serializer_Should_Be_Registered()
    {
        var serializers = BsonSerializer.SerializerRegistry;
        var serializer = serializers.GetSerializer(typeof(UserId));
        
        Assert.NotNull(serializer);
        Assert.IsType<UserIdSerializer>(serializer);
    }

    [Fact]
    public void Serializer_Should_Roundtrip_Value()
    {
        MongoClassMaps.RegisterAll();
        
        var original = new UserId(Guid.NewGuid());
        var collection = GetTestCollection<UserId>();
        
        collection.InsertOne(original);
        var retrieved = collection.Find(u => u.Id == original.Id).First();
        
        Assert.Equal(original.Value, retrieved.Value);
    }
}
```

## Test Organization Best Practices

### 1. Use Descriptive Test Names

```csharp
// ✅ Good - describes scenario and expected result
[Fact]
public void Name_TryParse_Should_Return_False_For_Empty_String()
{
}

// ❌ Bad - vague
[Fact]
public void Test1()
{
}
```

### 2. Use Arrange-Act-Assert Pattern

```csharp
[Fact]
public void Age_TryParse_Should_Parse_Valid_Integer()
{
    // Arrange
    const string testValue = "42";
    
    // Act
    var result = Age.TryParse(testValue, out var age);
    
    // Assert
    Assert.True(result);
    Assert.Equal(42, age.Value);
}
```

### 3. Test Both Success and Failure Cases

```csharp
// ✅ Test success
[Fact]
public void TryParse_Valid_Input() => Assert.True(Age.TryParse("25", out _));

// ✅ Test failure
[Fact]
public void TryParse_Invalid_Input() => Assert.False(Age.TryParse("invalid", out _));

// ✅ Test edge cases
[Fact]
public void TryParse_Empty_String() => Assert.False(Age.TryParse("", out _));

[Fact]
public void TryParse_Null() => Assert.False(Age.TryParse(null, out _));
```

### 4. Group Related Tests

```csharp
#region String Type Tests
[Fact] public void String_Valid() { }
[Fact] public void String_Null() { }
[Fact] public void String_Empty() { }
#endregion

#region Numeric Type Tests
[Fact] public void Numeric_Valid() { }
[Fact] public void Numeric_Invalid() { }
#endregion
```

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "ClassName=TryParseTests"
```

### Run Specific Test Method
```bash
dotnet test --filter "FullyQualifiedName~TryParseTests.Name_TryParse_Should_Parse_Valid_String"
```

### Run with Detailed Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Test Coverage Goals

Target coverage by feature:

| Feature | Min Coverage |
|---------|------------|
| TryParse | 90% |
| Operators | 85% |
| Type Conversions | 85% |
| EF Core Integration | 80% |
| MongoDB Integration | 80% |

## Adding New Tests

### When Adding a New Emitter Feature

1. **Create test value objects** in `AssemblyInfo.cs`
2. **Write tests** in a new test class
3. **Test success cases** (valid inputs)
4. **Test failure cases** (invalid inputs, edge cases)
5. **Test integration** with other features

### Example: Testing a New Emitter

```csharp
// 1. Add test value object
// In AssemblyInfo.cs:
public partial record MyNewType(int Value) : IValueObject<int>;

// 2. Create test file
// File: tests/ValueObjectTests/MyNewEmitterTests.cs
public class MyNewEmitterTests
{
    [Fact]
    public void MyNewEmitter_Generates_Code()
    {
        // The test value object above is processed by the generator
        // Test the generated functionality
    }
}
```

## Debugging Tests

### Enable Source Generator Debugging

```csharp
// In ValueObjectIncrementalGenerator.cs, uncomment:
// #if DEBUG
// if (!System.Diagnostics.Debugger.IsAttached)
// {
//     System.Diagnostics.Debugger.Launch();
// }
// #endif
```

This will pause at generator initialization during test run.

### Check Generated Files

Generated files are in:
```
tests/ValueObjectTests/obj/Debug/net10.0/generated/ValueObject.SourceGenerator/
```

View these files to verify correct code generation.

### Print Debug Output

```csharp
[Fact]
public void MyTest()
{
    var result = Age.TryParse("25", out var age);
    Console.WriteLine($"Result: {result}, Value: {age?.Value}");
    Assert.True(result);
}
```

Run with: `dotnet test --logger "console;verbosity=detailed"`

## Test Data Patterns

### Numeric Boundaries

```csharp
[Fact]
public void Int_TryParse_Zero() => Assert.True(Age.TryParse("0", out var age) && age.Value == 0);

[Fact]
public void Int_TryParse_Negative() => Assert.True(Age.TryParse("-1", out var age) && age.Value == -1);

[Fact]
public void Int_TryParse_MaxValue()
{
    Assert.True(Age.TryParse(int.MaxValue.ToString(), out var age));
    Assert.Equal(int.MaxValue, age.Value);
}
```

### Format Providers

```csharp
[Fact]
public void Decimal_TryParse_With_German_Culture()
{
    var culture = CultureInfo.GetCultureInfo("de-DE");
    Assert.True(Price.TryParse("19,99", culture, out var price));
    Assert.Equal(19.99m, price.Value);
}
```

### String Variations

```csharp
[Fact]
public void Email_TryParse_Accepts_Whitespace()
{
    // Email wraps string, which accepts whitespace
    Assert.True(Email.TryParse("   ", out var email));
    Assert.Equal("   ", email.Value);
}

[Fact]
public void Email_TryParse_Rejects_Null()
{
    Assert.False(Email.TryParse(null, out var email));
}
```

## CI/CD Integration

Tests should pass on:
- Local development machines
- CI servers (GitHub Actions, Azure Pipelines, etc.)
- Different .NET versions (if targeting multiple frameworks)

Example GitHub Actions workflow:

```yaml
name: Tests
on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '10.0'
      - run: dotnet restore
      - run: dotnet test --no-restore
```

## Next Steps

- **[Emitter Development](./EMITTER_DEVELOPMENT.md)** - How to create new emitters
- **[Architecture Overview](./ARCHITECTURE.md)** - How the generator works
- **[Getting Started](../GETTING_STARTED.md)** - User guide

