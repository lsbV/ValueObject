# Architecture Overview

## High-Level Design

The ValueObject source generator is an incremental source generator that automatically discovers value object declarations and generates utility methods.

### Generation Pipeline

```
Input: C# source code
    ↓
[1] Syntax Analysis
    - Discover all `record` declarations with base list
    ↓
[2] Semantic Analysis
    - Check if record implements IValueObject<T>
    - Extract type name, namespace, and wrapped type T
    ↓
[3] Incremental Collection
    - Collect all VoCandidate objects
    - Group by underlying type
    ↓
[4] Code Generation (Parallel Emitters)
    ├→ OperatorsEmitter
    ├→ TryParseEmitter
    ├→ TvAsEmitter (Type Extensions)
    ├→ ExtensionBlockEmitter
    ├→ ConversionEmitter (EF Core, conditional)
    ├→ MongoDbSerializerEmitter (conditional)
    ├→ MongoDbClassMapEmitter (conditional)
    └→ EfCoreModelBuilderEmitter (EF Core, conditional)
    ↓
Output: Generated .g.cs files
```

## Key Components

### 1. VoDeclarationProvider (Discovery)

**Location:** `src/ValueObject.SourceGenerator/Providers/VoDeclarationProvider.cs`

**Responsibility:** Discovers all value object candidates in the codebase

```csharp
public static IncrementalValuesProvider<VoCandidate> Setup(IncrementalGeneratorInitializationContext ctx)
{
    return ctx.SyntaxProvider
        // Step 1: Filter syntax - only look at records with base lists
        .CreateSyntaxProvider(
            predicate: (node, _) => node is RecordDeclarationSyntax r && r.BaseList is not null,
            // Step 2: Transform - extract semantic information
            transform: (ctx, _) => {
                var sym = ctx.SemanticModel.GetDeclaredSymbol(decl) as INamedTypeSymbol;
                // Step 3: Check for IValueObject<T> interface
                var impl = sym.AllInterfaces.FirstOrDefault(i => 
                    SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, voIf));
                // Return VoCandidate with type info
                return new VoCandidate(sym.Name, ns, tvString, isRecordStruct, isReadOnly);
            }
        );
}
```

**Output:** `IEnumerable<VoCandidate>` - all discovered value objects

**VoCandidate Model:**
```csharp
internal sealed record VoCandidate(
    string TypeName,              // e.g., "UserId"
    string? Namespace,            // e.g., "MyApp.Domain"
    string TvDisplay,             // e.g., "global::System.Guid"
    bool IsRecordStruct,          // true if 'record struct'
    bool IsReadOnly               // true if 'readonly'
);
```

### 2. Emitters (Code Generation)

Each emitter is responsible for generating one aspect of the value object:

#### OperatorsEmitter
**File:** `src/ValueObject.SourceGenerator/Emitters/OperatorsEmitter.cs`

Generates:
- Implicit operator to underlying type
- Explicit operator from underlying type
- Equality operators (`==`, `!=`)
- Comparison operators (`<`, `>`, `<=`, `>=`) for numeric types

#### TryParseEmitter
**File:** `src/ValueObject.SourceGenerator/Emitters/TryParseEmitter.cs`

Generates:
- `TryParse(string?, IFormatProvider?, out T)` method
- `TryParse(string?, out T)` convenience overload
- Type-specific parsing logic for each underlying type

#### TvAsEmitter (Type Extensions Core)
**File:** `src/ValueObject.SourceGenerator/Emitters/TvAsEmitter.cs`

Generates:
- `StringAs`, `GuidAs`, `IntAs`, etc. helper classes
- One property per value object for each underlying type
- Implements `IAs<T>` interface

#### ExtensionBlockEmitter
**File:** `src/ValueObject.SourceGenerator/Emitters/ExtensionBlockEmitter.cs`

Generates:
- `AsExtensions` class with extension methods
- One extension method per underlying type
- Enables `.As()` syntax on values

#### ConversionEmitter (EF Core)
**File:** `src/ValueObject.SourceGenerator/Emitters/ConversionEmitter.cs`

Generates (when enabled):
- `XyzValueConverter : ValueConverter<T, U>` classes
- `XyzNullableValueConverter` for nullable support
- Enables EF Core value conversion

#### MongoDbSerializerEmitter
**File:** `src/ValueObject.SourceGenerator/Emitters/MongoDbSerializerEmitter.cs`

Generates (when enabled):
- `XyzSerializer : SerializerBase<T>` classes
- Type-specific serialization logic
- BSON serialization/deserialization

#### MongoDbClassMapEmitter
**File:** `src/ValueObject.SourceGenerator/Emitters/MongoDbClassMapEmitter.cs`

Generates (when enabled):
- `MongoClassMaps` static class
- `RegisterAll()` method registering all serializers
- Convenience method for initialization

#### EfCoreModelBuilderEmitter
**File:** `src/ValueObject.SourceGenerator/Emitters/EfCoreModelBuilderEmitter.cs`

Generates (when enabled):
- `ValueObjectEfCoreExtensions` class
- `ConfigureValueObjectProperties()` extension method
- Entity-specific converter configuration

### 3. EntityValueObjectProvider (EF Core Analysis)

**Location:** `src/ValueObject.SourceGenerator/Providers/EntityValueObjectProvider.cs`

**Responsibility:** Scans entities for value object properties

Analyzes all types to find:
- Properties with value object types
- Their parent entity types
- Whether they're nullable

**Output:** `IEnumerable<EntityValueObjectProperty>`

```csharp
internal sealed record EntityValueObjectProperty(
    string EntityName,
    string? EntityNamespace,
    string PropertyName,
    string ValueObjectTypeName,
    bool IsNullable
);
```

### 4. Main Generator Orchestration

**Location:** `src/ValueObject.SourceGenerator/ValueObjectIncrementalGenerator.cs`

**Responsibility:** Coordinates all emitters

```csharp
public void Initialize(IncrementalGeneratorInitializationContext ctx)
{
    // 1) Discover value objects
    var vosProvider = VoDeclarationProvider.Setup(ctx).Collect();

    // 2) Register output with all providers
    ctx.RegisterSourceOutput(
        ctx.CompilationProvider.Combine(vosProvider),
        (spc, source) =>
        {
            var (compilation, voArray) = source;
            
            // Read per-assembly settings
            var (genMongo, genEfCore) = GetSettings(compilation);

            // Always emit core generators
            OperatorsEmitter.Emit(spc, voArray);
            TvAsEmitter.Emit(spc, voArray);
            ExtensionBlockEmitter.Emit(spc, voArray);
            TryParseEmitter.Emit(spc, voArray);

            // Conditionally emit based on settings
            if (genEfCore)
            {
                ConversionEmitter.Emit(spc, voArray);
                var entityProps = EntityValueObjectProvider.Collect(compilation, voArray);
                EfCoreModelBuilderEmitter.Emit(spc, entityProps);
            }

            if (genMongo)
            {
                MongoDbSerializerEmitter.Emit(spc, voArray);
                MongoDbClassMapEmitter.Emit(spc, voArray);
            }
        });
}
```

## Incremental Generation Benefits

The generator uses **incremental source generation** for performance:

- **Syntax filtering**: Only processes records with base lists (early filter)
- **Semantic filtering**: Only analyzes records implementing `IValueObject<T>`
- **Incremental updates**: Only regenerates changed files
- **Caching**: Results cached between builds
- **Parallelization**: Emitters run in parallel where possible

## Type Analysis

The generator understands these type categories:

### String Types
```csharp
if (tvDisplay is "string" or "System.String")
{
    // Direct wrapping, no parsing logic
}
```

### Numeric Types
```csharp
IsNumericType(tvDisplay) // byte, short, int, long, float, double, decimal
{
    // Use TryParse with NumberStyles.Any
}
```

### GUID Types
```csharp
if (tvDisplay is "System.Guid" or "Guid")
{
    // Use Guid.TryParse(string, IFormatProvider, out Guid)
}
```

### MongoDB ObjectId
```csharp
if (tvDisplay.Contains("ObjectId"))
{
    // Use ObjectId.Parse() with exception handling
}
```

### Custom Types
```csharp
// If type has static TryParse(string, IFormatProvider, out T)
{
    // Generate code using that method
}
```

## Configuration Resolution

The generator reads assembly-level `ValueObjectSettingsAttribute`:

```csharp
private static (bool generateMongo, bool generateEfCore) GetSettings(Compilation compilation)
{
    var attrType = compilation.GetTypeByMetadataName(
        "ValueObject.Core.ValueObjectSettingsAttribute");
    
    // If not found, use defaults: both true
    // If found, read constructor arguments
}
```

## Generated File Naming Convention

Each emitter follows a naming pattern:

- **Operators**: `{TypeName}_Operators.g.cs`
- **TryParse**: `{TypeName}_TryParse.g.cs`
- **Converters**: `{TypeName}_ValueConverters.g.cs`
- **Serializers**: `{TypeName}_MongoDbSerializer.g.cs`
- **Type extensions**: `{TypeSimpleName}As.g.cs` (one per type like `StringAs`)
- **Helpers**: `MongoClassMaps.g.cs`, `AsExtensions.g.cs`, `ValueObjectEfCoreExtensions.g.cs`

## Key Design Decisions

### 1. Partial Classes
Generated code extends partial declarations:
- Doesn't modify original source
- Developer can add custom methods alongside
- Avoids fragmentation

### 2. Type-Specific Code Generation
Different logic for different types:
- String types: direct wrapping
- Numeric types: with NumberStyles
- ObjectId: with exception handling
- Others: standard TryParse pattern

### 3. Grouped Emitters
Some emitters group by underlying type:
- `TvAsEmitter`: One class per underlying type (e.g., `StringAs`, `GuidAs`)
- `ExtensionBlockEmitter`: One extension class for all types
- `MongoDbClassMapEmitter`: One registry for all serializers

### 4. Conditional Generation
Some code generated only when enabled:
- EF Core converters: only if `generateEfCoreValueConverter`
- MongoDB serializers: only if `generateMongoDbSerializer`
- TryParse/Operators: always (can't be disabled)

### 5. No Reflection at Runtime
All generation is compile-time:
- Zero runtime overhead
- Works with AOT compilation
- Efficient execution

## File Structure Reference

```
src/ValueObject.SourceGenerator/
├── ValueObjectIncrementalGenerator.cs      (Orchestrator)
├── GlobalUsing.cs                           (Global imports)
├── RuntimeHelpers.cs                        (Helper methods)
├── Providers/
│   ├── VoDeclarationProvider.cs            (Discovery)
│   └── EntityValueObjectProvider.cs        (EF Core analysis)
├── Emitters/
│   ├── OperatorsEmitter.cs
│   ├── TryParseEmitter.cs
│   ├── TvAsEmitter.cs
│   ├── ExtensionBlockEmitter.cs
│   ├── ConversionEmitter.cs
│   ├── MongoDbSerializerEmitter.cs
│   ├── MongoDbClassMapEmitter.cs
│   └── EfCoreModelBuilderEmitter.cs
└── Models/
    ├── VoCandidate.cs                       (VO info)
    └── EntityValueObjectProperty.cs        (EF Core property info)
```

## Next Steps

For contributors:
- **[Emitter Development](./EMITTER_DEVELOPMENT.md)** - How to create new emitters
- **[Testing Guide](./TESTING.md)** - Writing tests for the generator


