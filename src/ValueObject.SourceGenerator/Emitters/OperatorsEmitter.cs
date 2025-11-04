using ValueObject.SourceGenerator.Models;

namespace ValueObject.SourceGenerator.Emitters;

internal static class OperatorsEmitter
{
    public static void Emit(SourceProductionContext ctx,
                            IEnumerable<VoCandidate> vos)
    {
        foreach (var vo in vos)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto‑generated />");
            if (vo.Namespace is not null)
            {
                sb.AppendLine($"namespace {vo.Namespace};");
                sb.AppendLine();
            }

            // match the original declaration shape
            var ro = vo.IsReadOnly ? "readonly " : string.Empty;
            var rs = vo.IsRecordStruct ? "record struct" : "record";
            sb.AppendLine($"public {ro} partial {rs} {vo.TypeName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static implicit operator {vo.TvDisplay}({vo.TypeName} v) => v.Value;");
            sb.AppendLine($"    public static explicit operator {vo.TypeName}({vo.TvDisplay} v) => new(v);");
            // == operator
            sb.AppendLine($"    public static bool operator ==({vo.TypeName} left, {vo.TvDisplay} right) => left.Value == right;");
            sb.AppendLine($"    public static bool operator !=({vo.TypeName} left, {vo.TvDisplay} right) => left.Value != right;");
            // Only emit the reversed operators when the containing type is on the left to satisfy C# rules.
            // For the reversed comparison we still place {vo.TypeName} on the left.
            sb.AppendLine($"    public static bool operator ==({vo.TvDisplay} left, {vo.TypeName} right) => left == right.Value;");
            sb.AppendLine($"    public static bool operator !=({vo.TvDisplay} left, {vo.TypeName} right) => left != right.Value;");
            // <, >, <=, >= operators for certain types
            if (TypesWithGreaterLessOperators.Contains(vo.TvDisplay))
            {
                sb.AppendLine($"    public static bool operator <({vo.TypeName} left, {vo.TypeName} right) => left.Value < right.Value;");
                sb.AppendLine($"    public static bool operator <=({vo.TypeName} left, {vo.TypeName} right) => left.Value <= right.Value;");
                sb.AppendLine($"    public static bool operator >({vo.TypeName} left, {vo.TypeName} right) => left.Value > right.Value;");
                sb.AppendLine($"    public static bool operator >=({vo.TypeName} left, {vo.TypeName} right) => left.Value >= right.Value;");

                sb.AppendLine($"    public static bool operator <({vo.TypeName} left, {vo.TvDisplay} right) => left.Value < right;");
                sb.AppendLine($"    public static bool operator <=({vo.TypeName} left, {vo.TvDisplay} right) => left.Value <= right;");
                sb.AppendLine($"    public static bool operator >({vo.TypeName} left, {vo.TvDisplay} right) => left.Value > right;");
                sb.AppendLine($"    public static bool operator >=({vo.TypeName} left, {vo.TvDisplay} right) => left.Value >= right;");

                sb.AppendLine($"    public static bool operator <({vo.TvDisplay} left, {vo.TypeName} right) => left < right.Value;");
                sb.AppendLine($"    public static bool operator <=({vo.TvDisplay} left, {vo.TypeName} right) => left <= right.Value;");
                sb.AppendLine($"    public static bool operator >({vo.TvDisplay} left, {vo.TypeName} right) => left > right.Value;");
                sb.AppendLine($"    public static bool operator >=({vo.TvDisplay} left, {vo.TypeName} right) => left >= right.Value;");
            }
            // +, - operators for certain types
            if (TypesWithPlusOperators.Contains(vo.TvDisplay))
            {
                sb.AppendLine($"    public static {vo.TypeName} operator +({vo.TypeName} left, {vo.TypeName} right) => new(left.Value + right.Value);");
                sb.AppendLine($"    public static {vo.TypeName} operator +({vo.TypeName} left, {vo.TvDisplay} right) => new(left.Value + right);");
                sb.AppendLine($"    public static {vo.TypeName} operator +({vo.TvDisplay} left, {vo.TypeName} right) => new(left + right.Value);");
            }
            if (TypesWithMinusOperators.Contains(vo.TvDisplay))
            {
                sb.AppendLine($"    public static {vo.TypeName} operator -({vo.TypeName} left, {vo.TypeName} right) => new(left.Value - right.Value);");
                sb.AppendLine($"    public static {vo.TypeName} operator -({vo.TypeName} left, {vo.TvDisplay} right) => new(left.Value - right);");
                sb.AppendLine($"    public static {vo.TypeName} operator -({vo.TvDisplay} left, {vo.TypeName} right) => new(left - right.Value);");
            }


            sb.AppendLine("    public override string ToString() => Value.ToString();");
            sb.AppendLine("}");

            ctx.AddSource($"{vo.TypeName}_Operators.g.cs",
                          SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }

    // TODO: use Roslyn to determine if the type supports these operators instead of hardcoding type names
    private static readonly string[] TypesWithGreaterLessOperators =
    [
        "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal", "char",
        "byte?", "sbyte?", "short?", "ushort?", "int?", "uint?", "long?", "ulong?", "float?", "double?", "decimal?", "char?",
    ];

    private static readonly string[] TypesWithPlusOperators =
    [
        "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal", "string",
    ];

    private static readonly string[] TypesWithMinusOperators =
    [
        "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal",
    ];

}