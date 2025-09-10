using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValueObject.SourceGenerator;

internal static class VoDeclarationProvider
{
    public static IncrementalValuesProvider<VoCandidate> Setup(IncrementalGeneratorInitializationContext ctx)
    {
        return ctx.SyntaxProvider
            // 1) Syntax filter: any record with a base‑list
            .CreateSyntaxProvider(
                predicate: static (node, _) =>
                    node is RecordDeclarationSyntax r && r.BaseList is not null,
                transform: static (ctx, _) =>
                {
                    var decl = (RecordDeclarationSyntax)ctx.Node;
                    var sym = ctx.SemanticModel.GetDeclaredSymbol(decl) as INamedTypeSymbol;
                    if (sym is null) return null;

                    // 2) Look for IValueObject<T>
                    var voIf = ctx.SemanticModel.Compilation
                        .GetTypeByMetadataName("ValueObject.Core.IValueObject`1");
                    var impl = sym.AllInterfaces
                        .FirstOrDefault(i =>
                            SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, voIf));
                    if (impl is null) return null;

                    var tValue = impl.TypeArguments[0];
                    var tvString = tValue.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var ns = sym.ContainingNamespace.IsGlobalNamespace
                                   ? null
                                   : sym.ContainingNamespace.ToString();

                    return new VoCandidate(sym.Name, ns, tvString);
                }
            )
            .Where(x => x is not null)!;  // filter out non‑VOs
    }
}
