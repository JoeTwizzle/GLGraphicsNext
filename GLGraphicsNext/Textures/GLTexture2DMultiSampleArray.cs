using OpenTK.Mathematics;

namespace GLGraphicsNext;

/// <summary>
/// An OpenGL Texture object with two dimensions and one index to its data. 
/// Every source pixel gets its color information from one or more samples when used as a Framebuffer attachment.
/// </summary>
public readonly unsafe struct GLTexture2DMultiSampleArray : IDisposable, IEquatable<GLTexture2DMultiSampleArray>
{
    public readonly GLTextureBase RawTexture;
    public readonly uint Width;
    public readonly uint Height;
    public readonly uint Layers;

    public GLTexture2DMultiSampleArray(GLTexture2DMultiSample srcTexture, SizedInternalFormat viewFormat)
    {
        RawTexture = new GLTextureBase(new GLObjectHandle(GL.GenTexture(), ObjectType.Texture));
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.Texture2dMultisampleArray, srcTexture.RawTexture.Handle.Value, viewFormat, 0, 1, 0, 1);
        Width = srcTexture.Width;
        Height = srcTexture.Height;
        Layers = 1;
    }

    public GLTexture2DMultiSampleArray(GLTexture2DMultiSampleArray srcTexture, SizedInternalFormat viewFormat, uint firstLayer, uint layerCount)
    {
        RawTexture = new GLTextureBase(new GLObjectHandle(GL.GenTexture(), ObjectType.Texture));
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.Texture2dMultisampleArray, srcTexture.RawTexture.Handle.Value, viewFormat, 0, 1, firstLayer, layerCount);
        Width = srcTexture.Width;
        Height = srcTexture.Height;
        Layers = layerCount;
    }

    [Obsolete($"The paramaterless constructor or default({nameof(GLTexture2DMultiSampleArray)}) creates an invalid {nameof(GLTexture2DMultiSampleArray)}", true)]
    public GLTexture2DMultiSampleArray()
    {
        ThrowHelper.ThrowInvalidOperationException($"Creates an invalid {nameof(GLTexture2DMultiSampleArray)}");
    }

    public GLTexture2DMultiSampleArray(GLTextureBase rawTexture)
    {
        RawTexture = rawTexture;
        Width = RawTexture.GetWidth();
        Height = RawTexture.GetHeight();
        Layers = RawTexture.GetDepth();
    }

    public GLTexture2DMultiSampleArray(uint width, uint height, uint layers, uint sampleCount, SizedInternalFormat sizedInternalFormat, bool useFixedSampleLocations = false)
    {
        Width = width;
        Height = height;
        Layers = layers;
        RawTexture = new GLTextureBase(TextureTarget.Texture2dMultisampleArray);
        GL.TextureStorage3DMultisample(RawTexture.Handle.Value, (int)sampleCount, sizedInternalFormat, (int)width, (int)height, (int)layers, useFixedSampleLocations);
    }

    public SizedInternalFormat GetSizedInternalFormat()
    {
        return RawTexture.GetSizedInternalFormat();
    }

    /// <summary>
    /// Fills the texture with a specific color value
    /// </summary>
    /// <param name="clearColor">The value to fill the texture with</param>
    /// <param name="level">The mip level to fill</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glClearTexImage.xhtml"/></remarks>
    public void Clear(Vector4 clearColor, int level = 0)
    {
        GL.ClearTexImage(RawTexture.Handle.Value, level, PixelFormat.Rgba, PixelType.Float, ref clearColor);
    }

    /// <summary>
    /// Binds a texture mip level to a specific 'image unit' (NOT 'texture image unit')
    /// </summary>
    /// <param name="access">How the texture is accessed in the shader program</param>
    /// <param name="unit">Which unit to bind the texture to</param>
    /// <param name="level">Which mip level to bind</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindImageTexture.xhtml"/></remarks>
    public void BindImage(BufferAccess access, uint unit = 0, int level = 0)
    {
        int internalFormat = 0;
        GL.GetTextureLevelParameteri(RawTexture.Handle.Value, 0, GetTextureParameter.TextureInternalFormat, ref internalFormat);
        GL.BindImageTexture(unit, RawTexture.Handle.Value, level, true, 0, access, (InternalFormat)internalFormat);
    }

    /// <summary>
    /// Binds texture mip level 0 to a specific 'image unit' with readwrite access (NOT 'texture image unit')
    /// </summary>
    /// <param name="unit">Which 'image unit' unit to bind the texture to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindImageTextures.xhtml"/></remarks>
    public void BindImage(uint unit = 0)
    {
        GL.BindImageTextures(unit, 1, [RawTexture.Handle.Value]);
    }

    /// <summary>
    /// Binds this texture to a specific 'texture image unit' (NOT 'image unit)
    /// </summary>
    /// <param name="unit">Which unit to bind the texture to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindTextureUnit.xhtml"/></remarks>
    public void Bind(uint unit = 0)
    {
        GL.BindTextureUnit(unit, RawTexture.Handle.Value);
    }

    public void Dispose()
    {
        RawTexture.Dispose();
    }

    public override bool Equals(object? obj)
    {
        return obj is GLTexture2DMultiSampleArray gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return RawTexture.GetHashCode();
    }

    public static bool operator ==(GLTexture2DMultiSampleArray left, GLTexture2DMultiSampleArray right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLTexture2DMultiSampleArray left, GLTexture2DMultiSampleArray right)
    {
        return !(left == right);
    }

    public bool Equals(GLTexture2DMultiSampleArray other)
    {
        return RawTexture.Equals(other.RawTexture);
    }
}
