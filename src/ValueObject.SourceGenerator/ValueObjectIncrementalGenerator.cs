using ValueObject.SourceGenerator.Emitters;
using ValueObject.SourceGenerator.Providers;

namespace ValueObject.SourceGenerator;

[Generator]
public class ValueObjectIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext ctx)
    {
        // 1) find all VO records
        var vosProvider = VoDeclarationProvider.Setup(ctx).Collect();

        // 2) combine with compilation for entity properties
        ctx.RegisterSourceOutput(
            ctx.CompilationProvider.Combine(vosProvider),
            (spc, source) =>
            {
                var (compilation, voArray) = source;
                if (voArray.Length == 0) return;

                // Emit standard VO pieces first
                OperatorsEmitter.Emit(spc, voArray);
                TvAsEmitter.Emit(spc, voArray);
                ExtensionBlockEmitter.Emit(spc, voArray);
                ConversionEmitter.Emit(spc, voArray);

                var entityProps = EntityValueObjectProvider.Collect(compilation, voArray);
                EfCoreModelBuilderEmitter.Emit(spc, entityProps);
            });
    }   
}