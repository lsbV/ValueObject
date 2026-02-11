# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.5] - 2026-02-11

### Fixed
- Added nullable comparison operators for value object structs to resolve ambiguous operator invocation errors when comparing nullable and non-nullable value objects.

### Added
- MongoDB integration tests with class fixture pattern using Testcontainers
  - Comprehensive CRUD operation tests with value objects
  - Query filtering tests by value object properties
  - Schema snapshot testing for MongoDB collections
  - Fixture-based container management for efficient test execution

## [1.0.4] - 2026-02-10

### Fixed
- EF Core model builder emitter now generates one file-scoped namespace and a single extensions class per namespace to avoid duplicate declarations.
- TryParse generation now defaults to invariant culture when provider is null and normalizes decimal separators for floating-point inputs.
- TryParse roundtrip test now formats doubles using invariant culture to match parsing behavior.

### Added
- Initial documentation suite
- Complete user and contributor guides

## [1.0.0] - 2026-02-09

### Added
- **TryParse Emitter**: Automatic generation of safe parsing methods for minimal APIs
  - Support for string, numeric, GUID, DateTime, and MongoDB ObjectId types
  - Format provider support for culture-specific parsing
  - Two overloads per value object (with and without provider)

- **Operators Emitter**: Automatic operator overloading
  - Implicit conversion to underlying type
  - Explicit conversion from underlying type
  - Equality operators (`==`, `!=`)
  - Comparison operators (`<`, `>`, `<=`, `>=`) for numeric types

- **Type Extensions Emitter**: The `As` pattern for type conversions
  - Generated `XyzAs` helper classes for each underlying type
  - Extension methods for ergonomic `.As()` syntax
  - Grouped access to all value objects of same type

- **Entity Framework Core Integration**
  - Automatic `ValueConverter<T, U>` generation
  - Nullable value converter support
  - Model builder extension methods
  - Seamless database mapping

- **MongoDB Integration**
  - BSON serializer generation for supported types
  - `MongoClassMaps.RegisterAll()` helper for easy initialization
  - Support for ObjectId, string, numeric, GUID, DateTime types

- **Configuration Support**
  - `ValueObjectSettingsAttribute` for assembly-level settings
  - Toggle EF Core converter generation
  - Toggle MongoDB serializer generation
  - Smart defaults (both enabled)

- **Core Features**
  - `IValueObject<T>` interface
  - `IAs<T>` interface for type extensions
  - Incremental source generator for performance
  - Support for both `record` and `record struct`
  - Support for `readonly record struct`
  - Zero runtime overhead
  - No reflection required

### Documentation
- Comprehensive user documentation (9 files)
  - Getting Started guide
  - Feature-specific documentation for each emitter
  - Configuration guide
  - Quick reference card
  - Documentation index/navigation hub

- Contributor documentation (4 files)
  - Architecture overview
  - Emitter development guide
  - Testing guide
  - Contributing guidelines

- 40+ practical code examples
- Real-world domain modeling examples (e-commerce, order management)
- Type support matrices
- Performance notes

### Technical
- .NET 10.0 support for core library
- netstandard2.0 for source generator (maximum compatibility)
- C# 12 language features
- Compatible with Visual Studio 2022, Rider, VS Code
- NuGet package ready for publishing

---

## Version History

### Version 1.0.0 - Initial Release
First public release with complete feature set:
- TryParse generation
- Operators and conversions
- Type extensions (As pattern)
- EF Core integration
- MongoDB integration
- Comprehensive documentation
