using ValueObject.SourceGenerator.Models;

namespace ValueObject.SourceGenerator.Emitters;

internal static class TryParseEmitter
{
    public static void Emit(SourceProductionContext ctx, IEnumerable<VoCandidate> vos)
    {
        foreach (var vo in vos)
        {
            var sb = new StringBuilder();
            sb.AppendLine("#nullable enable");
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
            
            // Determine how to call TryParse based on the underlying type
            var tvDisplay = vo.TvDisplay;
            var isString = tvDisplay is "string" or "System.String";
            var isObjectId = tvDisplay.Contains("ObjectId");
            var isNumericType = IsNumericType(tvDisplay);
            var isGuid = tvDisplay is "System.Guid" or "Guid";
            
            // Generate TryParse method with 3 parameters
            sb.AppendLine($"    public static bool TryParse(string? value, IFormatProvider? provider, out {vo.TypeName} result)");
            sb.AppendLine("    {");
            sb.AppendLine("        result = default!; // assign default to out parameter");
            sb.AppendLine("        if (string.IsNullOrEmpty(value))");
            sb.AppendLine("            return false;");
            sb.AppendLine("        provider ??= System.Globalization.CultureInfo.InvariantCulture;");
            if (IsFloatingType(tvDisplay))
            {
                sb.AppendLine("        value = value.Replace(',', '.');");
            }
            sb.AppendLine();
            
            if (isString)
            {
                // For string, just wrap the value directly
                sb.AppendLine($"        result = new(value!);");
                sb.AppendLine($"        return true;");
            }
            else if (isObjectId)
            {
                // For ObjectId, try to parse it
                sb.AppendLine($"        try");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            var parsedValue = {tvDisplay}.Parse(value);");
                sb.AppendLine($"            result = new(parsedValue);");
                sb.AppendLine($"            return true;");
                sb.AppendLine($"        }}");
                sb.AppendLine($"        catch");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            return false;");
                sb.AppendLine($"        }}");
            }
            else if (isNumericType && !isGuid)
            {
                // For numeric types that support NumberStyles overload
                sb.AppendLine($"        // Try TryParse(string, NumberStyles, IFormatProvider, out T)");
                sb.AppendLine($"        if ({tvDisplay}.TryParse(value, System.Globalization.NumberStyles.Any, provider, out var parsedValue))");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            result = new(parsedValue);");
                sb.AppendLine($"            return true;");
                sb.AppendLine($"        }}");
                sb.AppendLine();
                sb.AppendLine("        return false;");
            }
            else
            {
                // For other types (like Guid), just use the 3-parameter overload
                sb.AppendLine($"        // Try TryParse(string, IFormatProvider, out T)");
                sb.AppendLine($"        if ({tvDisplay}.TryParse(value, provider, out var parsedValue))");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            result = new(parsedValue);");
                sb.AppendLine($"            return true;");
                sb.AppendLine($"        }}");
                sb.AppendLine();
                sb.AppendLine("        return false;");
            }
            sb.AppendLine("    }");
            sb.AppendLine();
            
            // Also generate an overload without provider for convenience
            sb.AppendLine($"    public static bool TryParse(string? value, out {vo.TypeName} result)");
            sb.AppendLine("    {");
            sb.AppendLine($"        return TryParse(value, null, out result);");
            sb.AppendLine("    }");
            
            sb.AppendLine("}");

            ctx.AddSource($"{vo.TypeName}_TryParse.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }

    private static bool IsNumericType(string typeDisplay)
    {
        var cleanType = typeDisplay.Replace("global::", "").Replace("System.", "");
        return cleanType switch
        {
            "byte" or "sbyte" or "short" or "ushort" or "int" or "uint" or "long" or "ulong" 
            or "float" or "double" or "decimal" => true,
            _ => false
        };
    }

    private static bool IsFloatingType(string typeDisplay)
    {
        var cleanType = typeDisplay.Replace("global::", "").Replace("System.", "");
        return cleanType is "float" or "double" or "decimal";
    }
}
