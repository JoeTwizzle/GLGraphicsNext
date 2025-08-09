namespace GLGraphicsNext;

/// <summary>
/// An OpenGL Query object. This object may represent any one of the specific Query kinds.
/// </summary>
public readonly struct GLQueryBase : IDisposable, IEquatable<GLQueryBase>
{
    public readonly GLObjectHandle Handle;

    [Obsolete($"The paramaterless constructor or default({nameof(GLQueryBase)}) creates an invalid {nameof(GLQueryBase)}", true)]
    public GLQueryBase()
    {
        ThrowHelper.ThrowInvalidOperationException($"Creates an invalid {nameof(GLQueryBase)}");
    }

    public GLQueryBase(GLObjectHandle handle)
    {
        Handle = handle;
    }

    public GLQueryBase(QueryTarget queryTarget)
    {
        Handle = new GLObjectHandle(GL.CreateQuery(queryTarget), GLObjectType.Query);
    }

    public void Dispose()
    {
        Handle.Dispose();
    }

    public override bool Equals(object? obj)
    {
        return obj is GLQueryBase gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public static bool operator ==(GLQueryBase left, GLQueryBase right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLQueryBase left, GLQueryBase right)
    {
        return !(left == right);
    }

    public bool Equals(GLQueryBase other)
    {
        return Handle.Equals(other.Handle);
    }
}
