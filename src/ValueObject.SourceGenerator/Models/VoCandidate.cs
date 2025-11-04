namespace ValueObject.SourceGenerator.Models;

internal sealed record VoCandidate(
    string TypeName,
    string? Namespace,
    string TvDisplay,
    bool IsRecordStruct,
    bool IsReadOnly
);