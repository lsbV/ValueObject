namespace ValueObject.SourceGenerator
{
    internal class RuntimeHelpers
    {
    }
}

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Runtime.CompilerServices
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Shim so that C# 9 record/init-only support compiles
    /// on frameworks that don’t ship this type.
    /// </summary>
    internal static class IsExternalInit { }
}
