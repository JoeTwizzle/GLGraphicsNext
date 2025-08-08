using System.Diagnostics.CodeAnalysis;

namespace GLGraphicsNext;
internal static class ThrowHelper
{
    [DoesNotReturn]
    internal static void ThrowInvalidOperationException(string? message)
    {
        throw new InvalidOperationException(message);
    }
}
