# Contributing to ValueObject

Thank you for your interest in contributing to the ValueObject project! This guide will help you get started.

## Getting Started

### Prerequisites

- .NET 10.0 or later
- C# 12 or later
- Visual Studio 2022, Rider, or VS Code
- Git

### Clone and Build

```bash
# Clone the repository
git clone https://github.com/yourusername/ValueObject.git
cd ValueObject

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test
```

## Project Structure

```
ValueObject/
├── src/
│   ├── ValueObject.Core/              # Core interfaces and attributes
│   │   ├── IValueObject.cs
│   │   ├── IAs.cs
│   │   └── ValueObjectSettingsAttribute.cs
│   └── ValueObject.SourceGenerator/   # Source generator implementation
│       ├── ValueObjectIncrementalGenerator.cs
│       ├── Emitters/                  # Code generation logic
│       ├── Providers/                 # Discovery logic
│       └── Models/                    # Data models
├── tests/
│   └── ValueObjectTests/              # Integration tests
├── docs/
│   ├── *.md                           # User documentation
│   └── contribute/                    # Contributor documentation
└── README.md
```

## How to Contribute

### Bug Reports

Found a bug? Please create an issue with:

1. **Description**: What's broken?
2. **Reproduction**: Steps to reproduce
3. **Expected behavior**: What should happen
4. **Actual behavior**: What happens instead
5. **Environment**: .NET version, OS, etc.

### Feature Requests

Have an idea? Open an issue with:

1. **Description**: What feature do you want?
2. **Motivation**: Why is it needed?
3. **Proposed solution**: How would it work?
4. **Alternatives**: Other approaches you considered

### Code Contributions

Ready to code? Follow these steps:

#### 1. Fork and Branch

```bash
git clone https://github.com/yourusername/ValueObject.git
cd ValueObject
git checkout -b feature/your-feature-name
```

Use descriptive branch names:
- `fix/issue-description`
- `feature/new-emitter-name`
- `docs/section-name`

#### 2. Make Your Changes

Follow these guidelines:

**Code Style**
- Use consistent indentation (4 spaces)
- Follow C# naming conventions
- Write self-documenting code
- Add comments for complex logic

**Emitter Development**
- See [Emitter Development Guide](./ARCHITECTURE.md)
- Follow the established emitter pattern
- Test your emitter thoroughly
- Add XML documentation comments

**Testing**
- See [Testing Guide](./TESTING.md)
- Write tests for new features
- Ensure all tests pass locally
- Aim for >80% code coverage

**Documentation**
- Update relevant docs in `docs/`
- Add examples in docstrings
- Update README if applicable

#### 3. Commit Your Changes

Write clear commit messages:

```bash
git add .
git commit -m "feat: add new emitter for XYZ feature

- Describe what was added
- Explain the implementation
- Reference any related issues (#123)"
```

Commit message format:
- `feat:` for new features
- `fix:` for bug fixes
- `docs:` for documentation
- `test:` for test changes
- `refactor:` for code cleanup

#### 4. Push and Create Pull Request

```bash
git push origin feature/your-feature-name
```

In your PR description:

1. **Title**: Clear, concise description
2. **Description**: What changed and why
3. **Motivation**: Link to related issue (e.g., "Fixes #123")
4. **Changes**: Key changes made
5. **Testing**: How was this tested?
6. **Checklist**:
   - [ ] Tests pass locally
   - [ ] Documentation updated
   - [ ] No breaking changes
   - [ ] Code follows style guide

## Development Workflow

### Building

```bash
# Full build
dotnet build

# Build specific project
dotnet build src/ValueObject.SourceGenerator/
```

### Testing

```bash
# Run all tests
dotnet test

# Run specific test file
dotnet test tests/ValueObjectTests/TryParseTests.cs

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Debugging

**Debugging the Generator**

The generator can be debugged during test execution:

1. Uncomment the debugger launch in `ValueObjectIncrementalGenerator.cs`
2. Run tests with `dotnet test`
3. Visual Studio will prompt to attach debugger

**Viewing Generated Code**

Generated files are in:
```
tests/ValueObjectTests/obj/Debug/net10.0/generated/
```

## Architecture Overview

For detailed information on how the generator works:

- **[Architecture Overview](./ARCHITECTURE.md)** - System design
- **[Emitter Development](./EMITTER_DEVELOPMENT.md)** - Creating new emitters
- **[Testing Guide](./TESTING.md)** - Writing tests

## Documentation

The project has two documentation sets:

**User Documentation** (`docs/*.md`)
- For developers using the library
- Getting started, features, examples
- Configuration options

**Contributor Documentation** (`docs/contribute/*.md`)
- For developers contributing to the generator
- Architecture, emitter development, testing

When contributing:
- Update user docs if changing public API
- Update contributor docs if changing internals
- Add examples for new features
- Keep docs in sync with code

## Code Review Process

All pull requests require review:

1. **Automated checks**
   - Build must pass
   - Tests must pass
   - Code style checks

2. **Code review**
   - At least one maintainer review
   - Feedback on approach and implementation
   - Discussion of design decisions

3. **Approval and merge**
   - Reviewers approve changes
   - Changes merged to main
   - Deploy to NuGet when ready

## Reporting Security Issues

Found a security vulnerability? Please email security@example.com instead of using the issue tracker. Include:

- Description of vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if any)

## Community Guidelines

- Be respectful and constructive
- Welcome diverse perspectives
- Focus on the code, not the person
- Help others in discussions
- Celebrate contributions

## Getting Help

- **Documentation**: See `docs/` folder
- **Issues**: Open a GitHub issue
- **Discussions**: Start a discussion thread
- **Email**: Contact maintainers

## Roadmap

See [ROADMAP.md](./ROADMAP.md) for planned features and improvements.

## License

By contributing, you agree to license your contributions under the [LICENSE.txt](../LICENSE.txt).

## Recognition

Contributors will be recognized in:
- Commit history
- Release notes
- Contributors file

Thank you for making ValueObject better! 🎉

## Quick Reference

| Task | Command |
|------|---------|
| Build | `dotnet build` |
| Test | `dotnet test` |
| Clean | `dotnet clean` |
| Format | `dotnet format` (if configured) |
| Pack | `dotnet pack` |

## Related Resources

- **[Getting Started](../GETTING_STARTED.md)** - User guide
- **[Architecture](./ARCHITECTURE.md)** - System design
- **[Emitters](./EMITTER_DEVELOPMENT.md)** - Creating features
- **[Testing](./TESTING.md)** - Writing tests

---

Questions? Open an issue or reach out to the maintainers!

