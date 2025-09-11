

using ValueObject.SourceGenerator.Models;

namespace ValueObject.SourceGenerator.Emitters;

internal static class ExtensionBlockEmitter
{
    public static void Emit(SourceProductionContext ctx,
                            IEnumerable<VoCandidate> vos)
    {
        // group by underlying TValue
        var byTv = vos
            .Select(v => v.TvDisplay)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(tv => tv, StringComparer.Ordinal);

        var sb = new StringBuilder();
        sb.AppendLine("// <auto‑generated />");
        sb.AppendLine("namespace ValueObject.Core;");
        sb.AppendLine();
        sb.AppendLine("public static class AsExtensions");
        sb.AppendLine("{");

        foreach (var tv in byTv)
        {
            // derive a simple name for the class from the type
            var simple = tv
                .Replace("global::", "")
                .Split('.')
                .Last();      // e.g. "String", "Guid", "DateOnly"

            sb.AppendLine($"    extension({tv} value)");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {simple}As As => new {simple}As(value);");
            sb.AppendLine("    }");
        }

        sb.AppendLine("}");
        ctx.AddSource("AsExtensions.g.cs",
                      SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}