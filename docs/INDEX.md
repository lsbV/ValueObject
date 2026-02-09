# ValueObject Documentation Index

Welcome to the ValueObject documentation! This project is a C# source generator that automatically creates utility methods for value objects.

## 📖 For Users

Start here if you want to use ValueObject in your projects.

### Getting Started
- **[Getting Started Guide](./GETTING_STARTED.md)** ⭐ **START HERE**
  - Installation and setup
  - Core concepts
  - Your first value object
  - Common patterns and best practices

### Feature Documentation
- **[TryParse Emitter](./TRYPARSE_EMITTER.md)**
  - Safe parsing from strings
  - ASP.NET Core minimal APIs integration
  - Format provider support
  - Supported types reference

- **[Operators Emitter](./OPERATORS_EMITTER.md)**
  - Implicit/explicit conversions
  - Equality and comparison operators
  - Type-safe operations
  - Real-world examples

- **[Type Extensions (As Pattern)](./TYPE_EXTENSIONS.md)**
  - Convenient type conversions
  - The `As` pattern syntax
  - Accessing multiple value objects
  - Grouped type conversions

- **[Entity Framework Core Integration](./ENTITY_FRAMEWORK.md)**
  - Value converters generation
  - Model builder extensions
  - Database mapping
  - E-commerce example

- **[MongoDB Integration](./MONGODB.md)**
  - BSON serializer generation
  - Document serialization
  - Repository patterns
  - Order management example

### Configuration
- **[Configuration Guide](./CONFIGURATION.md)**
  - Customizing generator behavior
  - Enabling/disabling features
  - Project-specific settings
  - Performance considerations

## 🛠️ For Contributors

Documentation for developers extending or improving the generator.

### Getting Started as a Contributor
- **[Contributing Guidelines](./contribute/CONTRIBUTING.md)** ⭐ **START HERE**
  - Project setup
  - How to contribute
  - Code style and standards
  - Pull request process

### Technical Documentation
- **[Architecture Overview](./contribute/ARCHITECTURE.md)**
  - System design and pipeline
  - Key components (Providers, Emitters, Models)
  - Incremental generation benefits
  - Type analysis system
  - Design decisions

- **[Emitter Development Guide](./contribute/EMITTER_DEVELOPMENT.md)**
  - Creating new emitters
  - Step-by-step examples
  - Type analysis patterns
  - Grouping strategies
  - Best practices and common issues

- **[Testing Guide](./contribute/TESTING.md)**
  - Test structure and organization
  - Writing tests for features
  - Running tests
  - Debugging generators
  - Coverage goals

## 🎯 Quick Navigation

### By Use Case

**I want to...**

- **Use ValueObject in my project**
  → Start with [Getting Started Guide](./GETTING_STARTED.md)

- **Integrate with ASP.NET Core**
  → See [TryParse Emitter](./TRYPARSE_EMITTER.md) and [Getting Started Guide](./GETTING_STARTED.md)

- **Use with Entity Framework Core**
  → Read [Entity Framework Core Integration](./ENTITY_FRAMEWORK.md)

- **Use with MongoDB**
  → Read [MongoDB Integration](./MONGODB.md)

- **Configure the generator**
  → See [Configuration Guide](./CONFIGURATION.md)

- **Contribute to the project**
  → Start with [Contributing Guidelines](./contribute/CONTRIBUTING.md)

- **Create a new emitter**
  → Follow [Emitter Development Guide](./contribute/EMITTER_DEVELOPMENT.md)

- **Understand how it works**
  → Read [Architecture Overview](./contribute/ARCHITECTURE.md)

- **Write tests**
  → Follow [Testing Guide](./contribute/TESTING.md)

### By Feature

| Feature | User Docs | Contributor Docs |
|---------|-----------|------------------|
| TryParse | [TRYPARSE_EMITTER.md](./TRYPARSE_EMITTER.md) | [ARCHITECTURE.md](./contribute/ARCHITECTURE.md) |
| Operators | [OPERATORS_EMITTER.md](./OPERATORS_EMITTER.md) | [ARCHITECTURE.md](./contribute/ARCHITECTURE.md) |
| Type Extensions | [TYPE_EXTENSIONS.md](./TYPE_EXTENSIONS.md) | [EMITTER_DEVELOPMENT.md](./contribute/EMITTER_DEVELOPMENT.md) |
| EF Core | [ENTITY_FRAMEWORK.md](./ENTITY_FRAMEWORK.md) | [ARCHITECTURE.md](./contribute/ARCHITECTURE.md) |
| MongoDB | [MONGODB.md](./MONGODB.md) | [ARCHITECTURE.md](./contribute/ARCHITECTURE.md) |

## 📚 Documentation Highlights

### Getting Started Guide
Complete introduction covering:
- What is a value object?
- Why use value objects?
- Installation and basic setup
- Your first value object
- Generated methods overview
- Common patterns with real examples
- Best practices and tips

### TryParse Emitter
In-depth guide to safe parsing:
- 3 generated code examples (string, numeric, GUID)
- 5 practical usage examples
- Complete supported types list
- Type-specific behavior details
- Method signatures and error handling
- Performance notes

### Architecture Overview
Technical deep-dive covering:
- Full generation pipeline
- Key components (Providers, Emitters)
- Incremental generation benefits
- Type analysis system
- Design decisions explained
- File structure reference

## 🔍 Finding Information

### By Topic

**Core Concepts**
- Value objects and why: [GETTING_STARTED.md](./GETTING_STARTED.md)
- Architecture: [ARCHITECTURE.md](./contribute/ARCHITECTURE.md)

**Type Conversions**
- Operators: [OPERATORS_EMITTER.md](./OPERATORS_EMITTER.md)
- Type extensions: [TYPE_EXTENSIONS.md](./TYPE_EXTENSIONS.md)
- TryParse: [TRYPARSE_EMITTER.md](./TRYPARSE_EMITTER.md)

**Database Integration**
- Entity Framework Core: [ENTITY_FRAMEWORK.md](./ENTITY_FRAMEWORK.md)
- MongoDB: [MONGODB.md](./MONGODB.md)

**Development**
- Contributing: [CONTRIBUTING.md](./contribute/CONTRIBUTING.md)
- Emitter development: [EMITTER_DEVELOPMENT.md](./contribute/EMITTER_DEVELOPMENT.md)
- Testing: [TESTING.md](./contribute/TESTING.md)

### By Audience

**Beginners**
1. [Getting Started Guide](./GETTING_STARTED.md) - Start here!
2. [Configuration Guide](./CONFIGURATION.md) - Customize for your project
3. Feature guides as needed ([TryParse](./TRYPARSE_EMITTER.md), [EF Core](./ENTITY_FRAMEWORK.md), etc.)

**Advanced Users**
1. [Architecture Overview](./contribute/ARCHITECTURE.md) - Deep dive
2. [Emitter Development Guide](./contribute/EMITTER_DEVELOPMENT.md) - Extend functionality
3. [Testing Guide](./contribute/TESTING.md) - Write tests

**Contributors**
1. [Contributing Guidelines](./contribute/CONTRIBUTING.md) - Getting started
2. [Architecture Overview](./contribute/ARCHITECTURE.md) - System design
3. [Emitter Development Guide](./contribute/EMITTER_DEVELOPMENT.md) - Create features
4. [Testing Guide](./contribute/TESTING.md) - Write tests

## 📋 Document Structure

All documentation follows a consistent structure:

1. **Overview** - What is this feature?
2. **Why** - Benefits and motivations
3. **Generated Code Example** - What code is created?
4. **Usage Examples** - Practical uses with code
5. **Reference** - Complete method/type documentation
6. **Real-World Example** - Practical domain modeling
7. **Next Steps** - Links to related topics

## 🔗 Cross-References

Documents link to related content:

- User docs link to relevant feature docs
- Feature docs link to configuration and examples
- Contributor docs link to architecture and testing
- All docs link to "Next Steps"

## 📝 Notes

- **Code Examples**: All examples are realistic and tested
- **API Reference**: Complete method signatures and parameters
- **Type Support**: Specific types listed with behavior
- **Performance**: Notes on efficiency and overhead
- **Best Practices**: Proven patterns and recommendations

## 🆘 Troubleshooting

### I can't find information about...

Check these resources in order:
1. [Getting Started Guide](./GETTING_STARTED.md) - Common topics
2. Feature-specific docs ([OPERATORS_EMITTER.md](./OPERATORS_EMITTER.md), etc.)
3. [Configuration Guide](./CONFIGURATION.md) - Setup issues
4. [Architecture Overview](./contribute/ARCHITECTURE.md) - How it works

### I want to contribute but don't know where to start

1. Read [Contributing Guidelines](./contribute/CONTRIBUTING.md)
2. Pick an area:
   - Bug fix? See [TESTING.md](./contribute/TESTING.md)
   - New feature? See [EMITTER_DEVELOPMENT.md](./contribute/EMITTER_DEVELOPMENT.md)
   - Documentation? See [CONTRIBUTING.md](./contribute/CONTRIBUTING.md)
3. Open an issue or PR

### Documentation is unclear or incomplete

Please help! You can:
- Open an issue describing what's unclear
- Suggest improvements
- Contribute clarifications
- Submit pull requests with better examples

## 📈 Roadmap

Key areas for documentation expansion:
- [ ] Video tutorials
- [ ] Interactive examples
- [ ] More real-world domain examples
- [ ] Performance benchmarks
- [ ] Migration guides from other patterns

## 🎓 Learning Path

**Beginner**
1. Read [Getting Started Guide](./GETTING_STARTED.md)
2. Create your first value objects
3. Explore [TryParse](./TRYPARSE_EMITTER.md) and [Operators](./OPERATORS_EMITTER.md)

**Intermediate**
1. Integrate with [Entity Framework Core](./ENTITY_FRAMEWORK.md) or [MongoDB](./MONGODB.md)
2. Review [Configuration Guide](./CONFIGURATION.md)
3. Check out best practices in individual guides

**Advanced**
1. Study [Architecture Overview](./contribute/ARCHITECTURE.md)
2. Learn [Emitter Development](./contribute/EMITTER_DEVELOPMENT.md)
3. Review [Testing Guide](./contribute/TESTING.md)
4. Contribute to the project!

---

**Questions?** Open an issue or check the [README.md](../README.md) for additional resources.

**Happy coding!** 🚀

