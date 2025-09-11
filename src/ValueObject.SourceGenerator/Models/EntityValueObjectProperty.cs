namespace ValueObject.SourceGenerator.Models;

internal sealed record EntityValueObjectProperty(
    string EntityName,
    string? EntityNamespace,
    string PropertyName,
    string ValueObjectTypeName,
    bool IsNullable
);
