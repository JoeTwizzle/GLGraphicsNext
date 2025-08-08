using OpenTK.Mathematics;

namespace GLGraphicsNext;

/// <summary>
/// An OpenGL Texture object with six faces, each with two dimensions of data and one index for each group of the six faces. 
/// </summary>
public readonly unsafe struct GLTextureCubemapArray : IDisposable, IEquatable<GLTextureCubemapArray>
{
    public readonly GLTextureBase RawTexture;
    public readonly uint Width;
    public readonly uint Height;
    public readonly uint MipLevels;
    public readonly uint Layers;

    public GLTextureCubemapArray(GLTextureCubemapArray srcTexture, SizedInternalFormat viewFormat, uint firstLayer, uint layerCount, uint firstMipLevel = 0, uint mipLevels = 0)
    {
        if (mipLevels == 0)
        {
            int lvls = 0;
            GL.GetTextureLevelParameteri(srcTexture.RawTexture.Handle.Value, 0, (GetTextureParameter)All.TextureImmutableLevels, ref lvls);
            lvls -= (int)firstMipLevel;
            ArgumentOutOfRangeException.ThrowIfLessThan(lvls, 1);
            mipLevels = (uint)lvls;
        }

        RawTexture = new GLTextureBase(new GLObjectHandle(GL.GenTexture(), ObjectType.Texture));
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.TextureCubeMapArray, srcTexture.RawTexture.Handle.Value, viewFormat, firstMipLevel, mipLevels, firstLayer * 6, layerCount * 6);
        Width = srcTexture.Width >> (int)firstMipLevel;
        Height = srcTexture.Height >> (int)firstMipLevel;
        MipLevels = mipLevels;
        Layers = layerCount;
    }

    public GLTextureCubemapArray(GLTextureCubemap srcTexture, SizedInternalFormat viewFormat, uint firstMipLevel = 0, uint mipLevels = 0)
    {
        if (mipLevels == 0)
        {
            int lvls = 0;
            GL.GetTextureLevelParameteri(srcTexture.RawTexture.Handle.Value, 0, (GetTextureParameter)All.TextureImmutableLevels, ref lvls);
            lvls -= (int)firstMipLevel;
            ArgumentOutOfRangeException.ThrowIfLessThan(lvls, 1);
            mipLevels = (uint)lvls;
        }

        RawTexture = new GLTextureBase(new GLObjectHandle(GL.GenTexture(), ObjectType.Texture));
        GL.TextureView(RawTexture.Handle.Value, TextureTarget.TextureCubeMapArray, srcTexture.RawTexture.Handle.Value, viewFormat, firstMipLevel, mipLevels, 0, 1);
        Width = srcTexture.Width >> (int)firstMipLevel;
        Height = srcTexture.Height >> (int)firstMipLevel;
        MipLevels = mipLevels;
        Layers = 1;
    }

    [Obsolete($"The paramaterless constructor or default({nameof(GLTextureCubemapArray)}) creates an invalid {nameof(GLTextureCubemapArray)}", true)]
    public GLTextureCubemapArray()
    {
        ThrowHelper.ThrowInvalidOperationException($"Creates an invalid {nameof(GLTextureCubemapArray)}");
    }

    public GLTextureCubemapArray(GLTextureBase rawTexture)
    {
        RawTexture = rawTexture;
        Width = RawTexture.GetWidth();
        Height = RawTexture.GetHeight();
        Layers = rawTexture.GetDepth() / 6;
        MipLevels = RawTexture.GetMipmapLevels();
    }

    public GLTextureCubemapArray(uint width, uint height, uint layers, SizedInternalFormat sizedInternalFormat, uint mipLevels = 1)
    {
        Width = width;
        Height = height;
        Layers = layers;
        MipLevels = mipLevels;
        RawTexture = new GLTextureBase(TextureTarget.TextureCubeMapArray);
        GL.TextureStorage3D(RawTexture.Handle.Value, (int)mipLevels, sizedInternalFormat, (int)width, (int)height, (int)layers * 6);
    }

    /// <summary>
    /// Uploads image data for the entire cubemap
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="layer">Which layer in the array to upload into</param>
    /// <param name="pixelFormat">PixelFormat of the source data</param>
    /// <param name="pixelType">PixelType of the souce data</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexSubImage3D.xhtml"/></remarks>
    public void UploadImageData(void* data, int layer, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0)
    {
        UploadImageData(data, (uint)layer, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <inheritdoc cref="UploadImageData{T}(ReadOnlySpan{T}, uint, PixelFormat, PixelType, uint)"/>
    public void UploadImageData(void* data, uint layer, PixelFormat pixelFormat, PixelType pixelType, uint mipLevel = 0)
    {
        GL.TextureSubImage3D(RawTexture.Handle.Value, (int)mipLevel, 0, 0, (int)layer * 6, (int)Width, (int)Height, 6, pixelFormat, pixelType, data);
    }

    /// <inheritdoc cref="UploadImageData{T}(ReadOnlySpan{T}, uint, PixelFormat, PixelType, uint)"/>
    public void UploadImageData<T>(ReadOnlySpan<T> data, int layer, PixelFormat pixelFormat, PixelType pixelType, uint mipLevel = 0) where T : unmanaged
    {
        UploadImageData(data, (uint)layer, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <summary>
    /// Uploads image data for the entire cubemap
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="layer">Which layer in the array to upload into</param>
    /// <param name="pixelFormat">PixelFormat of the source data</param>
    /// <param name="pixelType">PixelType of the souce data</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexSubImage3D.xhtml"/></remarks>
    public void UploadImageData<T>(ReadOnlySpan<T> data, uint layer, PixelFormat pixelFormat, PixelType pixelType, uint mipLevel = 0) where T : unmanaged
    {
        ArgumentOutOfRangeException.ThrowIfLessThan((uint)data.Length, Width * Height * 6);
        GL.TextureSubImage3D(RawTexture.Handle.Value, (int)mipLevel, 0, 0, (int)layer * 6, (int)Width, (int)Height, 6, pixelFormat, pixelType, data);
    }

    /// <summary>
    /// Uploads image data into a region of this texture
    /// </summary>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="region">Region of texture to write to</param>
    /// <param name="layer">Which layer in the array to upload into</param>
    /// <param name="face">Which cubemap face to write to</param>
    /// <param name="pixelFormat">PixelFormat of the source data</param>
    /// <param name="pixelType">PixelType of the souce data</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexSubImage3D.xhtml"/></remarks>
    public void UploadImageData(void* data, Box2i region, int layer, CubemapFace face, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0)
    {
        UploadImageData(data, (uint)region.X, (uint)region.Y, (uint)layer, face, (uint)region.SizeX, (uint)region.SizeY, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <inheritdoc cref="UploadImageData{T}(ReadOnlySpan{T}, Box2i, CubemapFace, PixelFormat, PixelType, int)"/>
    public void UploadImageData(void* data, int xOffset, int yOffset, int layer, CubemapFace face, int regionWidth, int regionHeight, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0)
    {
        UploadImageData(data, (uint)xOffset, (uint)yOffset, (uint)layer, face, (uint)regionWidth, (uint)regionHeight, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <inheritdoc cref="UploadImageData{T}(ReadOnlySpan{T}, uint, uint, uint, CubemapFace, uint, uint, PixelFormat, PixelType, uint)"/>
    public void UploadImageData(void* data, uint xOffset, uint yOffset, uint layer, CubemapFace face, uint regionWidth, uint regionHeight, PixelFormat pixelFormat, PixelType pixelType, uint mipLevel = 0)
    {
        GL.TextureSubImage3D(RawTexture.Handle.Value, (int)mipLevel, (int)xOffset, (int)yOffset, (int)layer * 6 + (int)face, (int)regionWidth, (int)regionHeight, 1, pixelFormat, pixelType, data);
    }

    /// <summary>
    /// Uploads image data into a region of this texture
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="region">Region of texture to write to</param>    
    /// <param name="layer">Which layer in the array to upload into</param>
    /// <param name="face">Which cubemap face to write to</param>
    /// <param name="pixelFormat">PixelFormat of the source data</param>
    /// <param name="pixelType">PixelType of the souce data</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexSubImage3D.xhtml"/></remarks>
    public void UploadImageData<T>(ReadOnlySpan<T> data, Box2i region, int layer, CubemapFace face, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0) where T : unmanaged
    {
        UploadImageData(data, (uint)region.X, (uint)region.Y, (uint)layer, face, (uint)region.SizeX, (uint)region.SizeY, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <inheritdoc cref="UploadImageData{T}(ReadOnlySpan{T}, uint, uint, uint, CubemapFace, uint, uint, PixelFormat, PixelType, uint)"/>
    public void UploadImageData<T>(ReadOnlySpan<T> data, int xOffset, int yOffset, int layer, CubemapFace face, int regionWidth, int regionHeight, PixelFormat pixelFormat, PixelType pixelType, int mipLevel = 0) where T : unmanaged
    {
        UploadImageData(data, (uint)xOffset, (uint)yOffset, (uint)layer, face, (uint)regionWidth, (uint)regionHeight, pixelFormat, pixelType, (uint)mipLevel);
    }

    /// <summary>
    /// Uploads image data into a region of this texture
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">Source data being uploaded</param>
    /// <param name="xOffset">X offset into the destination texture</param>
    /// <param name="yOffset">Y offset into the destination texture</param>
    /// <param name="face">Which cubemap face to write to</param>    
    /// <param name="layer">Which layer in the array to upload into</param>
    /// <param name="regionWidth">Width of the region to write to</param>
    /// <param name="regionHeight">Height of the region to write to</param>
    /// <param name="pixelFormat">PixelFormat of the source data</param>
    /// <param name="pixelType">PixelType of the souce data</param>
    /// <param name="mipLevel">Which mip level to write to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexSubImage3D.xhtml"/></remarks>
    public void UploadImageData<T>(ReadOnlySpan<T> data, uint xOffset, uint yOffset, uint layer, CubemapFace face, uint regionWidth, uint regionHeight, PixelFormat pixelFormat, PixelType pixelType, uint mipLevel = 0) where T : unmanaged
    {
        ArgumentOutOfRangeException.ThrowIfLessThan((uint)data.Length, regionWidth * regionHeight);
        GL.TextureSubImage3D(RawTexture.Handle.Value, (int)mipLevel, (int)xOffset, (int)yOffset, (int)layer * 6 + (int)face, (int)regionWidth, (int)regionHeight, 1, pixelFormat, pixelType, data);
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
        return obj is GLTextureCubemapArray gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return RawTexture.GetHashCode();
    }

    public static bool operator ==(GLTextureCubemapArray left, GLTextureCubemapArray right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLTextureCubemapArray left, GLTextureCubemapArray right)
    {
        return !(left == right);
    }

    public bool Equals(GLTextureCubemapArray other)
    {
        return RawTexture.Equals(other.RawTexture);
    }
}
