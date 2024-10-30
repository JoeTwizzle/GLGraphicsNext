﻿using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GLGraphicsNext;

public readonly unsafe struct Texture2DArray : IDisposable, IEquatable<Texture2DArray>
{
    public readonly TextureBase RawTexture;
    public readonly uint Width;
    public readonly uint Height;
    public readonly uint MipLevels;
    public readonly uint Layers;

    public Texture2DArray(Texture2D srcTexture, SizedInternalFormat viewFormat, uint firstMipLevel = 0, uint mipLevels = 0)
    {
        if (mipLevels == 0)
        {
            int lvls = 0;
            GL.GetTextureLevelParameteri(srcTexture.RawTexture.Handle.Value, 0, (GetTextureParameter)All.TextureImmutableLevels, ref lvls);
            lvls -= (int)firstMipLevel;
            ArgumentOutOfRangeException.ThrowIfLessThan(lvls, 1);
            mipLevels = (uint)lvls;
        }

        RawTexture = new TextureBase(new GLObjectHandle(GL.GenTexture(), GLObjectType.Texture));
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.Texture2dArray, srcTexture.RawTexture.Handle.Value, viewFormat, firstMipLevel, mipLevels, 0, 1);
        Width = srcTexture.Width >> (int)firstMipLevel;
        Height = srcTexture.Height >> (int)firstMipLevel;
        Layers = 1;
        MipLevels = mipLevels;
    }

    public Texture2DArray(TextureCubemap srcTexture, SizedInternalFormat viewFormat, CubemapFace firstFace, uint faceCount, uint firstMipLevel = 0, uint mipLevels = 0)
    {
        if (mipLevels == 0)
        {
            int lvls = 0;
            GL.GetTextureLevelParameteri(srcTexture.RawTexture.Handle.Value, 0, (GetTextureParameter)All.TextureImmutableLevels, ref lvls);
            lvls -= (int)firstMipLevel;
            ArgumentOutOfRangeException.ThrowIfLessThan(lvls, 1);
            mipLevels = (uint)lvls;
        }

        RawTexture = new TextureBase(new GLObjectHandle(GL.GenTexture(), GLObjectType.Texture));
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.Texture2dArray, srcTexture.RawTexture.Handle.Value, viewFormat, firstMipLevel, mipLevels, (uint)firstFace, faceCount);
        Width = srcTexture.Width >> (int)firstMipLevel;
        Height = srcTexture.Height >> (int)firstMipLevel;
        Layers = faceCount;
        MipLevels = mipLevels;
    }

    public Texture2DArray(Texture2DArray srcTexture, SizedInternalFormat viewFormat, uint firstLayer, uint layerCount, uint firstMipLevel = 0, uint mipLevels = 0)
    {
        if (mipLevels == 0)
        {
            int lvls = 0;
            GL.GetTextureLevelParameteri(srcTexture.RawTexture.Handle.Value, 0, (GetTextureParameter)All.TextureImmutableLevels, ref lvls);
            lvls -= (int)firstMipLevel;
            ArgumentOutOfRangeException.ThrowIfLessThan(lvls, 1);
            mipLevels = (uint)lvls;
        }

        RawTexture = new TextureBase(new GLObjectHandle(GL.GenTexture(), GLObjectType.Texture));
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.Texture2dArray, srcTexture.RawTexture.Handle.Value, viewFormat, firstMipLevel, mipLevels, firstLayer, layerCount);
        Width = srcTexture.Width >> (int)firstMipLevel;
        Height = srcTexture.Height >> (int)firstMipLevel;
        Layers = layerCount;
        MipLevels = mipLevels;
    }

    public Texture2DArray(TextureCubemapArray srcTexture, SizedInternalFormat viewFormat, CubemapFace firstFace, uint faceCount, uint firstLayer, uint layerCount, uint firstMipLevel = 0, uint mipLevels = 0)
    {
        if (mipLevels == 0)
        {
            int lvls = 0;
            GL.GetTextureLevelParameteri(srcTexture.RawTexture.Handle.Value, 0, (GetTextureParameter)All.TextureImmutableLevels, ref lvls);
            lvls -= (int)firstMipLevel;
            ArgumentOutOfRangeException.ThrowIfLessThan(lvls, 1);
            mipLevels = (uint)lvls;
        }

        RawTexture = new TextureBase(new GLObjectHandle(GL.GenTexture(), GLObjectType.Texture));
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.Texture2dArray, srcTexture.RawTexture.Handle.Value, viewFormat, firstMipLevel, mipLevels, firstLayer * 6 + (uint)firstFace, layerCount * 6 + faceCount);
        Width = srcTexture.Width >> (int)firstMipLevel;
        Height = srcTexture.Height >> (int)firstMipLevel;
        Layers = layerCount * 6 + faceCount;
        MipLevels = mipLevels;
    }

    [Obsolete($"The paramaterless constructor creates an invalid {nameof(Texture2DArray)}")]
    public Texture2DArray()
    { }

    public Texture2DArray(TextureBase rawTexture)
    {
        RawTexture = rawTexture;
        Width = RawTexture.GetWidth();
        Height = RawTexture.GetHeight();
        Layers = RawTexture.GetDepth();
        MipLevels = RawTexture.GetMipmapLevels();
    }

    public Texture2DArray(uint width, uint height, uint layers, SizedInternalFormat sizedInternalFormat, uint mipLevels = 1)
    {
        Width = width;
        Height = height;
        Layers = layers;
        MipLevels = mipLevels;
        RawTexture = new TextureBase(TextureTarget.Texture2dArray);
        GL.TextureStorage3D(RawTexture.Handle.Value, (int)mipLevels, sizedInternalFormat, (int)width, (int)height, (int)layers);
    }

    public SizedInternalFormat GetSizedInternalFormat()
    {
        return RawTexture.GetSizedInternalFormat();
    }

    /// <summary>
    /// Uploads image data into a region of this texture
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="region">Region of texture to write to</param>
    /// <param name="index">Index in the texture array to write to</param>
    /// <param name="pixelFormat">PixelFormat of the source data</param>
    /// <param name="pixelType">PixelType of the souce data</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexSubImage3D.xhtml"/></remarks>
    public void UploadImageData(void* data, Box2i region, int index, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0)
    {
        UploadImageData(data, (uint)region.X, (uint)region.Y, (uint)index, (uint)region.SizeX, (uint)region.SizeY, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <inheritdoc cref="UploadImageData{T}(ReadOnlySpan{T}, uint, uint, uint, uint, PixelFormat, PixelType, uint)"/>
    public void UploadImageData(void* data, int xOffset, int yOffset, int index, int regionWidth, int regionHeight, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0)
    {
        UploadImageData(data, (uint)xOffset, (uint)yOffset, (uint)index, (uint)regionWidth, (uint)regionHeight, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <inheritdoc cref="UploadImageData{T}(ReadOnlySpan{T}, uint, uint, uint, uint, PixelFormat, PixelType, uint)"/>
    public void UploadImageData(void* data, uint xOffset, uint yOffset, uint index, uint regionWidth, uint regionHeight, PixelFormat pixelFormat, PixelType pixelType, uint mipLevel = 0)
    {
        GL.TextureSubImage3D(RawTexture.Handle.Value, (int)mipLevel, (int)xOffset, (int)yOffset, (int)index, (int)regionWidth, (int)regionHeight, 1, pixelFormat, pixelType, data);
    }

    /// <summary>
    /// Uploads image data into a region of this texture
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="region">Region of texture to write to</param>
    /// <param name="index">Index in the texture array to write to</param>
    /// <param name="pixelFormat">PixelFormat of the source data</param>
    /// <param name="pixelType">PixelType of the souce data</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexSubImage3D.xhtml"/></remarks>
    public void UploadImageData<T>(ReadOnlySpan<T> data, Box2i region, int index, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0) where T : unmanaged
    {
        UploadImageData(data, (uint)region.X, (uint)region.Y, (uint)index, (uint)region.SizeX, (uint)region.SizeY, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <inheritdoc cref="UploadImageData{T}(ReadOnlySpan{T}, uint, uint, uint, uint, uint, PixelFormat, PixelType, uint)"/>
    public void UploadImageData<T>(ReadOnlySpan<T> data, int xOffset, int yOffset, int index, int regionWidth, int regionHeight, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0) where T : unmanaged
    {
        UploadImageData(data, (uint)xOffset, (uint)yOffset, (uint)index, (uint)regionWidth, (uint)regionHeight, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <summary>
    /// Uploads image data into a region of this texture
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="xOffset">X offset into the destination texture</param>
    /// <param name="yOffset">Y offset into the destination texture</param>
    /// <param name="index">Index in the texture array to write to</param>
    /// <param name="regionWidth">Width of the region to write to</param>
    /// <param name="regionHeight">Height of the region to write to</param>
    /// <param name="pixelFormat">PixelFormat of the source data</param>
    /// <param name="pixelType">PixelType of the souce data</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexSubImage3D.xhtml"/></remarks>
    public void UploadImageData<T>(ReadOnlySpan<T> data, uint xOffset, uint yOffset, uint index, uint regionWidth, uint regionHeight, PixelFormat pixelFormat, PixelType pixelType, uint mipLevel = 0) where T : unmanaged
    {
        ArgumentOutOfRangeException.ThrowIfLessThan((uint)data.Length, regionWidth * regionHeight);
        GL.TextureSubImage3D(RawTexture.Handle.Value, (int)mipLevel, (int)xOffset, (int)yOffset, (int)index, (int)regionWidth, (int)regionHeight, 1, pixelFormat, pixelType, data);
    }

    /// <summary>
    /// Generates mipmaps based on information present at mip level 0
    /// </summary>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenerateMipmap.xhtml"/></remarks>
    public void GenerateMipmaps()
    {
        GL.GenerateTextureMipmap(RawTexture.Handle.Value);
    }

    /// <summary>
    /// Fills a mip level of a texture with a specific color value
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
        return obj is Texture2DArray gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return RawTexture.GetHashCode();
    }

    public static bool operator ==(Texture2DArray left, Texture2DArray right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Texture2DArray left, Texture2DArray right)
    {
        return !(left == right);
    }

    public bool Equals(Texture2DArray other)
    {
        return RawTexture.Equals(other.RawTexture);
    }
}
