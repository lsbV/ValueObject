using ValueObject.SourceGenerator.Emitters;
using ValueObject.SourceGenerator.Providers;

namespace ValueObject.SourceGenerator;

[Generator]
public class ValueObjectIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext ctx)
    {
//#if DEBUG
//        if (!System.Diagnostics.Debugger.IsAttached)
//        {
//            System.Diagnostics.Debugger.Launch();
//        }
//#endif
        //1) find all VO records
        var vosProvider = VoDeclarationProvider.Setup(ctx).Collect();

        //2) combine with compilation for entity properties
        ctx.RegisterSourceOutput(
            ctx.CompilationProvider.Combine(vosProvider),
            (spc, source) =>
            {
                var (compilation, voArray) = source;
                if (voArray.Length ==0) return;

                // Read per-assembly settings (defaults to true if attribute absent)
                var (genMongo, genEfCore) = GetSettings(compilation);

                // Always emit core helpers/operators
                OperatorsEmitter.Emit(spc, voArray);
                TvAsEmitter.Emit(spc, voArray);
                ExtensionBlockEmitter.Emit(spc, voArray);
                TryParseEmitter.Emit(spc, voArray);

                // Conditionally emit EF Core value converters and model builder helpers
                if (genEfCore)
                {
                    ConversionEmitter.Emit(spc, voArray);

                    var entityProps = EntityValueObjectProvider.Collect(compilation, voArray);
                    EfCoreModelBuilderEmitter.Emit(spc, entityProps);
                }

                // Conditionally emit MongoDB serializers and registrar
                if (genMongo)
                {
                    MongoDbSerializerEmitter.Emit(spc, voArray);
                    MongoDbClassMapEmitter.Emit(spc, voArray);
                }
            });
    }

    private static (bool generateMongo, bool generateEfCore) GetSettings(Compilation compilation)
    {
        // Defaults
        var generateMongo = true;
        var generateEfCore = true;

        var attrType = compilation.GetTypeByMetadataName("ValueObject.Core.ValueObjectSettingsAttribute");
        if (attrType is null)
            return (generateMongo, generateEfCore);

        var asmAttrs = compilation.Assembly.GetAttributes();
        foreach (var ad in asmAttrs)
        {
            if (!SymbolEqualityComparer.Default.Equals(ad.AttributeClass, attrType))
                continue;

            // Constructor has (bool generateMongoDbSerializer, bool generateEfCoreValueConverter)
            if (ad.ConstructorArguments.Length ==2)
            {
                var a0 = ad.ConstructorArguments[0];
                var a1 = ad.ConstructorArguments[1];
                if (a0.Value is bool b0) generateMongo = b0;
                if (a1.Value is bool b1) generateEfCore = b1;
            }

            // Named arguments could also target properties (in case ctor changes later)
            foreach (var kv in ad.NamedArguments)
            {
                var name = kv.Key;
                var typedVal = kv.Value;
                if (string.Equals(name, "GenerateMongoDbSerializer", StringComparison.Ordinal))
                {
                    if (typedVal.Value is bool b) generateMongo = b;
                }
                else if (string.Equals(name, "GenerateEfCoreValueConverter", StringComparison.Ordinal))
                {
                    if (typedVal.Value is bool b) generateEfCore = b;
                }
            }

            // Use the first matching attribute
            break;
        }

        return (generateMongo, generateEfCore);
    }
}