using OpenTK.Mathematics;

namespace GLGraphicsNext;

public readonly unsafe struct Texture1DArray : IDisposable, IEquatable<Texture1DArray>
{
    public readonly TextureBase RawTexture;
    public readonly uint Width;
    public readonly uint Layers;
    public readonly uint MipLevels;

    public Texture1DArray(Texture1DArray srcTexture, SizedInternalFormat viewFormat, uint firstLayer, uint layerCount, uint firstMipLevel = 0, uint mipLevels = 0)
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
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.Texture1dArray, srcTexture.RawTexture.Handle.Value, viewFormat, firstMipLevel, mipLevels, 0, 1);
        Width = srcTexture.Width >> (int)firstMipLevel;
        Layers = layerCount;
        MipLevels = mipLevels;
    }

    public Texture1DArray(Texture1D srcTexture, SizedInternalFormat viewFormat, CubemapFace face, uint firstMipLevel = 0, uint mipLevels = 0)
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
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.Texture2d, srcTexture.RawTexture.Handle.Value, viewFormat, firstMipLevel, mipLevels, (uint)face, 1);
        Width = srcTexture.Width >> (int)firstMipLevel;
        Layers = 1;
        MipLevels = mipLevels;
    }

    [Obsolete($"The paramaterless constructor or default({nameof(Texture1DArray)}) creates an invalid {nameof(Texture1DArray)}", true)]
    public Texture1DArray()
    {
        ThrowHelper.ThrowInvalidOperationException($"Creates an invalid {nameof(Texture1DArray)}");
    }

    public Texture1DArray(TextureBase rawTexture)
    {
        RawTexture = rawTexture;
        Width = RawTexture.GetWidth();
        Layers = RawTexture.GetHeight();
        MipLevels = RawTexture.GetMipmapLevels();
    }

    public Texture1DArray(uint width, uint layers, SizedInternalFormat sizedInternalFormat, uint mipLevels = 1)
    {
        Width = width;
        Layers = layers;
        MipLevels = mipLevels;
        RawTexture = new TextureBase(TextureTarget.Texture1dArray);
        GL.TextureStorage2D(RawTexture.Handle.Value, (int)mipLevels, sizedInternalFormat, (int)width, (int)layers);
    }

    public SizedInternalFormat GetSizedInternalFormat()
    {
        return RawTexture.GetSizedInternalFormat();
    }
    /// <summary>
    /// Uploads compressed image data into a region of this texture
    /// </summary>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="xOffset">X offset into the destination texture</param>
    /// <param name="layer">Layer in the texture array to write to</param>
    /// <param name="regionWidth">Width of the region to write to</param>
    /// <param name="internalFormat">Internal format of the source data (must be compressed)</param>
    /// <param name="compressedDataSize">Size in bytes of the souce data to copy</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glCompressedTexSubImage2D.xhtml"/></remarks>
    public void UploadCompressedImageData(void* data, uint xOffset, uint layer, uint regionWidth, InternalFormat internalFormat, uint compressedDataSize, uint mipLevel = 0)
    {
        GL.CompressedTextureSubImage2D(RawTexture.Handle.Value, (int)mipLevel, (int)xOffset, (int)layer, (int)regionWidth, 1, internalFormat, (int)compressedDataSize, data);
    }

    /// <summary>
    /// Uploads compressed image data into a region of this texture
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="xOffset">X offset into the destination texture</param>
    /// <param name="layer">Layer in the texture array to write to</param>
    /// <param name="regionWidth">Width of the region to write to</param>
    /// <param name="internalFormat">Internal format of the source data (must be compressed)</param>
    /// <param name="compressedDataSize">Size in bytes of the souce data to copy</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glCompressedTexSubImage2D.xhtml"/></remarks>
    public void UploadCompressedImageData<T>(ReadOnlySpan<T> data, uint xOffset, uint layer, uint regionWidth, InternalFormat internalFormat, uint compressedDataSize, uint mipLevel = 0) where T : unmanaged
    {
        GL.CompressedTextureSubImage2D(RawTexture.Handle.Value, (int)mipLevel, (int)xOffset, (int)layer, (int)regionWidth, 1, internalFormat, (int)compressedDataSize, data);
    }


    /// <summary>
    /// Uploads image data into a region of this texture
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="range">Region of texture to write to</param>
    /// <param name="layer">Layer in the texture array to write to</param>
    /// <param name="pixelFormat">PixelFormat of the source data</param>
    /// <param name="pixelType">PixelType of the souce data</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexSubImage2D.xhtml"/></remarks>
    public void UploadImageData(void* data, Range range, int layer, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0)
    {
        var (offset, length) = range.GetOffsetAndLength((int)Width);
        UploadImageData(data, (uint)offset, (uint)layer, (uint)length, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <inheritdoc cref="UploadImageData{T}(ReadOnlySpan{T}, uint, uint, uint, PixelFormat, PixelType, uint)"/>
    public void UploadImageData(void* data, int xOffset, int layer, int regionWidth, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0)
    {
        UploadImageData(data, (uint)xOffset, (uint)layer, (uint)regionWidth, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <inheritdoc cref="UploadImageData{T}(ReadOnlySpan{T}, uint, uint, uint, PixelFormat, PixelType, uint)"/>
    public void UploadImageData(void* data, uint xOffset, uint layer, uint regionWidth, PixelFormat pixelFormat, PixelType pixelType, uint mipLevel = 0)
    {
        GL.TextureSubImage2D(RawTexture.Handle.Value, (int)mipLevel, (int)xOffset, (int)layer, (int)regionWidth, 1, pixelFormat, pixelType, data);
    }

    /// <summary>
    /// Uploads image data into a region of this texture
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="range">Region of texture to write to</param>
    /// <param name="layer">Layer in the texture array to write to</param>
    /// <param name="pixelFormat">PixelFormat of the source data</param>
    /// <param name="pixelType">PixelType of the souce data</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexSubImage2D.xhtml"/></remarks>
    public void UploadImageData<T>(ReadOnlySpan<T> data, Range range, int layer, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0) where T : unmanaged
    {
        var (offset, length) = range.GetOffsetAndLength((int)Width);
        UploadImageData(data, (uint)offset, (uint)layer, (uint)length, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <inheritdoc cref="UploadImageData{T}(ReadOnlySpan{T}, uint, uint, uint, PixelFormat, PixelType, uint)"/>
    public void UploadImageData<T>(ReadOnlySpan<T> data, int xOffset, int layer, int regionWidth, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0) where T : unmanaged
    {
        UploadImageData(data, (uint)xOffset, (uint)layer, (uint)regionWidth, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <summary>
    /// Uploads image data into a region of this texture
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="xOffset">X offset into the destination texture</param>
    /// <param name="layer">Layer in the texture array to write to</param>
    /// <param name="regionWidth">Width of the region to write to</param>
    /// <param name="pixelFormat">PixelFormat of the source data</param>
    /// <param name="pixelType">PixelType of the souce data</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexSubImage2D.xhtml"/></remarks>
    public void UploadImageData<T>(ReadOnlySpan<T> data, uint xOffset, uint layer, uint regionWidth, PixelFormat pixelFormat, PixelType pixelType, uint mipLevel = 0) where T : unmanaged
    {
        ArgumentOutOfRangeException.ThrowIfLessThan((uint)data.Length, regionWidth);
        GL.TextureSubImage2D(RawTexture.Handle.Value, (int)mipLevel, (int)xOffset, (int)layer, (int)regionWidth, 1, pixelFormat, pixelType, data);
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
    /// <param name="clearColor">The RGBA color to fill the texture with</param>
    /// <param name="level">The mip level to fill</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glClearTexImage.xhtml"/></remarks>
    public void Clear(Vector4 clearColor, int level = 0)
    {
        GL.ClearTexImage(RawTexture.Handle.Value, level, PixelFormat.Rgba, PixelType.Float, ref clearColor);
    }

    /// <inheritdoc cref="Clear(Vector4, int)"/>
    public void Clear(Vec4 clearColor, int level = 0)
    {
        GL.ClearTexImage(RawTexture.Handle.Value, level, PixelFormat.Rgba, PixelType.Float, ref clearColor);
    }

    /// <inheritdoc cref="Clear(Vector4, int)"/>
    public void Clear(Color4<Rgba> clearColor, int level = 0)
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
        return obj is Texture1DArray gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return RawTexture.GetHashCode();
    }

    public static bool operator ==(Texture1DArray left, Texture1DArray right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Texture1DArray left, Texture1DArray right)
    {
        return !(left == right);
    }

    public bool Equals(Texture1DArray other)
    {
        return RawTexture.Equals(other.RawTexture);
    }
}
