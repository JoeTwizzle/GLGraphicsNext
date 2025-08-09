namespace GLGraphicsNext;

/// <summary>
/// An OpenGL Texture object. This object may represent any one of the specific Texture kinds.
/// </summary>
public readonly struct GLTextureBase : IDisposable, IEquatable<GLTextureBase>
{
    public readonly GLObjectHandle Handle;

    [Obsolete($"The paramaterless constructor or default({nameof(GLTextureBase)}) creates an invalid {nameof(GLTextureBase)}", true)]
    public GLTextureBase()
    {
        ThrowHelper.ThrowInvalidOperationException($"Creates an invalid {nameof(GLTextureBase)}");
    }

    public GLTextureBase(GLObjectHandle handle)
    {
        Handle = handle;
    }

    public GLTextureBase(TextureTarget textureTarget)
    {
        Handle = new GLObjectHandle(GL.CreateTexture(textureTarget), GLObjectType.Texture);
    }

    /// <summary>
    /// Gets the bindless handle that identifies this texture. 
    /// </summary>
    /// <returns>The new bindless handle for this texture</returns>
    /// <remarks>
    /// <para><see href="https://ktstephano.github.io/rendering/opengl/bindless"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_bindless_texture.txt"/></para>
    /// </remarks>
    public GLBindlessTextureHandle CreateBindlessHandle()
    {
        return new GLBindlessTextureHandle(this);
    }

    public uint GetWidth()
    {
        int width = 0;
        GL.GetTextureLevelParameteri(Handle.Value, 0, GetTextureParameter.TextureWidth, ref width);
        return (uint)width;
    }

    public uint GetHeight()
    {
        int height = 0;
        GL.GetTextureLevelParameteri(Handle.Value, 0, GetTextureParameter.TextureHeight, ref height);
        return (uint)height;
    }

    public uint GetDepth()
    {
        int depth = 0;
        GL.GetTextureLevelParameteri(Handle.Value, 0, GetTextureParameter.TextureDepthExt, ref depth);
        return (uint)depth;
    }

    public uint GetMipmapLevels()
    {
        int levels = 0;
        GL.GetTextureLevelParameteri(Handle.Value, 0, (GetTextureParameter)All.TextureImmutableLevels, ref levels);
        return (uint)levels;
    }

    public SizedInternalFormat GetSizedInternalFormat()
    {
        int internalFormat = 0;
        GL.GetTextureLevelParameteri(Handle.Value, 0, GetTextureParameter.TextureInternalFormat, ref internalFormat);
        return (SizedInternalFormat)internalFormat;
    }

    public void Dispose()
    {
        Handle.Dispose();
    }

    public override bool Equals(object? obj)
    {
        return obj is GLTextureBase gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public static bool operator ==(GLTextureBase left, GLTextureBase right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLTextureBase left, GLTextureBase right)
    {
        return !(left == right);
    }

    public bool Equals(GLTextureBase other)
    {
        return Handle.Equals(other.Handle);
    }
}
