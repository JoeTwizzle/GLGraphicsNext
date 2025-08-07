namespace GLGraphicsNext;
public readonly struct TextureBase : IDisposable, IEquatable<TextureBase>
{
    public readonly GLObjectHandle Handle;

    [Obsolete($"The paramaterless constructor or default({nameof(TextureBase)}) creates an invalid {nameof(TextureBase)}", true)]
    public TextureBase()
    {
        ThrowHelper.ThrowInvalidOperationException($"Creates an invalid {nameof(TextureBase)}");
    }

    public TextureBase(GLObjectHandle handle)
    {
        Handle = handle;
    }

    public TextureBase(TextureTarget textureTarget)
    {
        Handle = new(GL.CreateTexture(textureTarget), GLObjectType.Texture);
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
        return obj is TextureBase gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public static bool operator ==(TextureBase left, TextureBase right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TextureBase left, TextureBase right)
    {
        return !(left == right);
    }

    public bool Equals(TextureBase other)
    {
        return Handle.Equals(other.Handle);
    }
}
