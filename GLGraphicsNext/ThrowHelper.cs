using System.Diagnostics.CodeAnalysis;

namespace GLGraphicsNext;
internal static class ThrowHelper
{

    [DoesNotReturn]
    public static void ThrowInvalidOperationException(string? message)
    {
        throw new InvalidOperationException(message);
    }
}
