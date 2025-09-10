using ValueObject.SourceGenerator.Emitters;

namespace ValueObject.SourceGenerator;

[Generator]
public class ValueObjectIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext ctx)
    {
        // 1) find all VO records
        var vos = VoDeclarationProvider.Setup(ctx)
                 .Collect();

        // 2) generate once, passing in the full list
        ctx.RegisterSourceOutput(
            ctx.CompilationProvider.Combine(vos),
            (spc, source) =>
            {
                var (compilation, voArray) = source;
                if (voArray.Length == 0) return;

                // 3) emit each piece
                OperatorsEmitter.Emit(spc, voArray);
                TvAsEmitter.Emit(spc, voArray);
                ExtensionBlockEmitter.Emit(spc, voArray);
                ConversionEmitter.Emit(spc, voArray);
            });
    }
}
