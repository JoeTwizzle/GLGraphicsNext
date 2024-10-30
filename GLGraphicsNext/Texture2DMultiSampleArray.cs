using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GLGraphicsNext;

public readonly unsafe struct Texture2DMultiSampleArray : IDisposable, IEquatable<Texture2DMultiSampleArray>
{
    public readonly TextureBase RawTexture;
    public readonly uint Width;
    public readonly uint Height;
    public readonly uint Layers;

    public Texture2DMultiSampleArray(Texture2DMultiSample srcTexture, SizedInternalFormat viewFormat)
    {
        RawTexture = new TextureBase(new GLObjectHandle(GL.GenTexture(), GLObjectType.Texture));
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.Texture2dMultisampleArray, srcTexture.RawTexture.Handle.Value, viewFormat, 0, 1, 0, 1);
        Width = srcTexture.Width;
        Height = srcTexture.Height;
        Layers = 1;
    }

    public Texture2DMultiSampleArray(Texture2DMultiSampleArray srcTexture, SizedInternalFormat viewFormat, uint firstLayer, uint layerCount)
    {
        RawTexture = new TextureBase(new GLObjectHandle(GL.GenTexture(), GLObjectType.Texture));
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.Texture2dMultisampleArray, srcTexture.RawTexture.Handle.Value, viewFormat, 0, 1, firstLayer, layerCount);
        Width = srcTexture.Width;
        Height = srcTexture.Height;
        Layers = layerCount;
    }

    [Obsolete($"The paramaterless constructor creates an invalid {nameof(Texture2DMultiSampleArray)}")]
    public Texture2DMultiSampleArray()
    { }

    public Texture2DMultiSampleArray(TextureBase rawTexture)
    {
        RawTexture = rawTexture;
        Width = RawTexture.GetWidth();
        Height = RawTexture.GetHeight();
        Layers = RawTexture.GetDepth();
    }

    public Texture2DMultiSampleArray(uint width, uint height, uint layers, uint sampleCount, SizedInternalFormat sizedInternalFormat, bool useFixedSampleLocations = false)
    {
        Width = width;
        Height = height;
        Layers = layers;
        RawTexture = new TextureBase(TextureTarget.Texture2dMultisampleArray);
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
        return obj is Texture2DMultiSampleArray gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return RawTexture.GetHashCode();
    }

    public static bool operator ==(Texture2DMultiSampleArray left, Texture2DMultiSampleArray right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Texture2DMultiSampleArray left, Texture2DMultiSampleArray right)
    {
        return !(left == right);
    }

    public bool Equals(Texture2DMultiSampleArray other)
    {
        return RawTexture.Equals(other.RawTexture);
    }
}
