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

            sb.AppendLine($"public partial record {vo.TypeName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static implicit operator {vo.TvDisplay}({vo.TypeName} v) => v.Value;");
            sb.AppendLine($"    public static explicit operator {vo.TypeName}({vo.TvDisplay} v) => new(v);");
            sb.AppendLine("}");

            ctx.AddSource($"{vo.TypeName}_Operators.g.cs",
                          SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}
