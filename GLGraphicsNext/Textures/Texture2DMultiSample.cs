using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GLGraphicsNext;

public readonly unsafe struct Texture2DMultiSample : IDisposable, IEquatable<Texture2DMultiSample>
{
    public readonly TextureBase RawTexture;
    public readonly uint Width;
    public readonly uint Height;

    public Texture2DMultiSample(Texture2DMultiSample srcTexture, SizedInternalFormat viewFormat)
    {
        RawTexture = new TextureBase(new GLObjectHandle(GL.GenTexture(), GLObjectType.Texture));
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.Texture2dMultisample, srcTexture.RawTexture.Handle.Value, viewFormat, 0, 1, 0, 1);
        Width = srcTexture.Width;
        Height = srcTexture.Height;
    }

    public Texture2DMultiSample(Texture2DMultiSampleArray srcTexture, SizedInternalFormat viewFormat, uint layer)
    {
        RawTexture = new TextureBase(new GLObjectHandle(GL.GenTexture(), GLObjectType.Texture));
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.Texture2dMultisample, srcTexture.RawTexture.Handle.Value, viewFormat, 0, 1, layer, 1);
        Width = srcTexture.Width;
        Height = srcTexture.Height;
    }

    [Obsolete($"The paramaterless constructor or default({nameof(Texture2DMultiSample)}) creates an invalid {nameof(Texture2DMultiSample)}", true)]
    public Texture2DMultiSample()
    {
        ThrowHelper.ThrowInvalidOperationException($"Creates an invalid {nameof(Texture2DMultiSample)}");
    }

    public Texture2DMultiSample(TextureBase rawTexture)
    {
        RawTexture = rawTexture;
        Width = RawTexture.GetWidth();
        Height = RawTexture.GetHeight();
    }

    public Texture2DMultiSample(uint width, uint height, uint sampleCount, SizedInternalFormat sizedInternalFormat, bool useFixedSampleLocations = false)
    {
        Width = width;
        Height = height;
        RawTexture = new TextureBase(TextureTarget.Texture2dMultisample);
        GL.TextureStorage2DMultisample(RawTexture.Handle.Value, (int)sampleCount, sizedInternalFormat, (int)width, (int)height, useFixedSampleLocations);
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
        return obj is Texture2DMultiSample gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return RawTexture.GetHashCode();
    }

    public static bool operator ==(Texture2DMultiSample left, Texture2DMultiSample right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Texture2DMultiSample left, Texture2DMultiSample right)
    {
        return !(left == right);
    }

    public bool Equals(Texture2DMultiSample other)
    {
        return RawTexture.Equals(other.RawTexture);
    }
}
