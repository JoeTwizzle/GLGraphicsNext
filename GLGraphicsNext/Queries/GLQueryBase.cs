using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLGraphicsNext;
public readonly struct GLQueryBase : IDisposable, IEquatable<GLQueryBase>
{
    public readonly GLObjectHandle Handle;

    [Obsolete($"The paramaterless constructor creates an invalid {nameof(GLQueryBase)}")]
    public GLQueryBase()
    { }

    public GLQueryBase(GLObjectHandle handle)
    {
        Handle = handle;
    }

    public GLQueryBase(QueryTarget queryTarget)
    {
        Handle = new(GL.CreateQuery(queryTarget), GLObjectType.Query);
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
