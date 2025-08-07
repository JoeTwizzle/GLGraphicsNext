using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace GLGraphicsNext;
internal static class ThrowHelper
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentOutOfRangeException(string? paramName)
    {
        throw new ArgumentOutOfRangeException(paramName);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidOperationException(string? message)
    {
        throw new InvalidOperationException(message);
    }
}
