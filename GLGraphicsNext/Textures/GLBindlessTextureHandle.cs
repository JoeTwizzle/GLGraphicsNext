namespace GLGraphicsNext;

/// <summary>
/// A handle that uniquely identifies a specific OpenGL texture. Requires extension ARB_bindless_texture.
/// </summary>
/// <remarks>
/// <para><see href="https://ktstephano.github.io/rendering/opengl/bindless"/></para>
/// <para><see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_bindless_texture.txt"/></para>
/// </remarks>
public readonly struct GLBindlessTextureHandle : IEquatable<GLBindlessTextureHandle>
{
    public readonly ulong Handle;

    /// <summary>
    /// Constructs the bindless handle that identifies this texture. 
    /// </summary>
    /// <remarks>
    /// <para><see href="https://ktstephano.github.io/rendering/opengl/bindless"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_bindless_texture.txt"/></para>
    /// </remarks>
    public GLBindlessTextureHandle(GLTextureBase texture)
    {
        Handle = GL.ARB.GetTextureHandleARB(texture.Handle.Value);
    }

    public GLBindlessTextureHandle(ulong handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Makes a texture accessible via its handle by the GPU
    /// </summary>
    /// <remarks>
    /// <para><see href="https://ktstephano.github.io/rendering/opengl/bindless"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_bindless_texture.txt"/></para>
    /// </remarks>
    public void MakeResident()
    {
        GL.ARB.MakeTextureHandleResidentARB(Handle);
    }
    /// <summary>
    /// Makes a texture inaccessible via its handle by the GPU
    /// </summary>
    /// <remarks>
    /// <para><see href="https://ktstephano.github.io/rendering/opengl/bindless"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_bindless_texture.txt"/></para>
    /// </remarks>
    public void MakeNonResident()
    {
        GL.ARB.MakeTextureHandleNonResidentARB(Handle);
    }

    public override bool Equals(object? obj)
    {
        return obj is GLBindlessTextureHandle bindlessTextureHandle && Equals(bindlessTextureHandle);
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public static bool operator ==(GLBindlessTextureHandle left, GLBindlessTextureHandle right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLBindlessTextureHandle left, GLBindlessTextureHandle right)
    {
        return !(left == right);
    }

    public bool Equals(GLBindlessTextureHandle other)
    {
        return Handle == other.Handle;
    }
}
