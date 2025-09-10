namespace ValueObject.SourceGenerator.Emitters;

internal class ConversionEmitter
{
    public static void Emit(SourceProductionContext ctx,
                             IEnumerable<VoCandidate> vos)
    {
        foreach (var vo in vos)
        {
            var sb = new StringBuilder();
            sb.AppendLine("#nullable enable");
            sb.AppendLine();
            sb.AppendLine("// <auto‑generated />");
            sb.AppendLine();
            sb.AppendLine("using Microsoft.EntityFrameworkCore.Storage.ValueConversion;");
            sb.AppendLine();
            if (vo.Namespace is not null)
            {
                sb.AppendLine($"namespace {vo.Namespace}.Generated.ValueConverters;");
                sb.AppendLine();
            }
            sb.AppendLine($"public class {vo.TypeName}ValueConverter : ValueConverter<{vo.TypeName}, {vo.TvDisplay}>");
            sb.AppendLine("{");
            sb.AppendLine($"    public {vo.TypeName}ValueConverter() : base(");
            sb.AppendLine($"        v => ({vo.TvDisplay})v,");
            sb.AppendLine($"        v => ({vo.TypeName})v)");
            sb.AppendLine("    {}");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine($"public class {vo.TypeName}NullableValueConverter : ValueConverter<{vo.TypeName}?, {vo.TvDisplay}?>");
            sb.AppendLine("{");
            sb.AppendLine($"    public {vo.TypeName}NullableValueConverter() : base(");
            sb.AppendLine($"        v => v == null ? null : ({vo.TvDisplay})v,");
            sb.AppendLine($"        v => v == null ? null : ({vo.TypeName})v)");
            sb.AppendLine("    {}");
            sb.AppendLine("}");
            ctx.AddSource($"{vo.TypeName}_ValueConverters.g.cs",
                          SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}
