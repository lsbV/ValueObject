using ValueObject.SourceGenerator.Models;

namespace ValueObject.SourceGenerator.Providers;

internal static class EntityValueObjectProvider
{
    internal static List<EntityValueObjectProperty> Collect(Compilation compilation, System.Collections.Immutable.ImmutableArray<VoCandidate> voArray)
    {
        var voTypeNames = new HashSet<string>(voArray.Select(v => v.TypeName));
        var entityProps = new List<EntityValueObjectProperty>();
        foreach (var tree in compilation.SyntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();
            var classDecls = root.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>();
            foreach (var classDecl in classDecls)
            {
                if (semanticModel.GetDeclaredSymbol(classDecl) is not INamedTypeSymbol classSymbol) continue;
                
                var entityName = classSymbol.Name;
                var entityNamespace = classSymbol.ContainingNamespace.IsGlobalNamespace ? null : classSymbol.ContainingNamespace.ToString();
                var properties = classSymbol.GetMembers().OfType<IPropertySymbol>();
                foreach (var prop in properties)
                {
                    var type = prop.Type;
                    var isNullable = type.NullableAnnotation == NullableAnnotation.Annotated;
                    var typeName = isNullable && type is INamedTypeSymbol nts && nts.TypeArguments.Length == 1
                        ? nts.TypeArguments[0].Name
                        : type.Name;
                    if (voTypeNames.Contains(typeName))
                    {
                        entityProps.Add(new EntityValueObjectProperty(
                            entityName,
                            entityNamespace,
                            prop.Name,
                            typeName,
                            isNullable
                        ));
                    }
                }
            }
        }

        return entityProps;
    }
}