using ValueObject.SourceGenerator.Models;

namespace ValueObject.SourceGenerator.Emitters;

internal static class TvAsEmitter
{
    public static void Emit(SourceProductionContext ctx,
                            IEnumerable<VoCandidate> vos)
    {
        var byTv = vos
            .GroupBy(v => v.TvDisplay)
            .OrderBy(g => g.Key, StringComparer.Ordinal);
        var namespaces = vos
            .Select(v => v.Namespace)
            .Where(ns => ns is not null)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(ns => ns, StringComparer.Ordinal)
            .ToList();

        foreach (var grp in byTv)
        {
            var tv = grp.Key;
            var tvSimple = tv.Replace("global::", "").Split('.').Last();

            var className = $"{tvSimple}As";         // e.g. "StringAs"
            var ctorParam = tv;                      // the parameter type
            var genericArg = tv;                      // the As<T> type argument

            var sb = new StringBuilder();
            sb.AppendLine("// <auto‑generated />");
            // add using directives, if any
            sb.AppendLine();
            foreach (var ns in namespaces)
            {
                sb.AppendLine($"using {ns};");
            }

            sb.AppendLine("namespace ValueObject.Core;");
            sb.AppendLine();
            // use primary ctor and call the base‑class primary ctor:
            sb.AppendLine($"public readonly record struct {className} : IAs<{genericArg}>");
            sb.AppendLine("{");
            sb.AppendLine($"    public {ctorParam} Value {{ get; }}");
            sb.AppendLine($"    public {className}({ctorParam} value) : this() => Value = value;");
            // one property-per‐VO
            foreach (var vo in grp)
            {
                sb.AppendLine($"    public {vo.TypeName} {vo.TypeName} => ({vo.TypeName})Value;");
            }
            sb.AppendLine("}");

            ctx.AddSource($"{className}.g.cs",
                          SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}
