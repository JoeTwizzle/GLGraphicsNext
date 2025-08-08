using System.Runtime.CompilerServices;

namespace GLGraphicsNext;
public readonly unsafe struct GLBuffer : IDisposable, IEquatable<GLBuffer>
{
    public readonly GLObjectHandle Handle;
    public readonly nuint SizeInBytes;

    [Obsolete($"The paramaterless constructor or default({nameof(GLBuffer)}) creates an invalid {nameof(GLBuffer)}", true)]
    public GLBuffer()
    {
        ThrowHelper.ThrowInvalidOperationException($"Creates an invalid {nameof(GLBuffer)}");
    }

    public GLBuffer(GLObjectHandle handle)
    {
        Handle = handle;
        int sizeInBytes = 0;
        GL.GetNamedBufferParameteri(handle.Value, BufferPName.BufferSize, ref sizeInBytes);
        SizeInBytes = (uint)sizeInBytes;
    }

    public GLBuffer(int sizeInBytes, BufferStorageMask bufferStorageMask = BufferStorageMask.DynamicStorageBit)
        : this((nuint)sizeInBytes, bufferStorageMask)
    { }

    public GLBuffer(nuint sizeInBytes, BufferStorageMask bufferStorageMask = BufferStorageMask.DynamicStorageBit)
    {
        Handle = new GLObjectHandle(GL.CreateBuffer(), GLObjectType.Buffer);
        SizeInBytes = sizeInBytes;
        GL.NamedBufferStorage(Handle.Value, (nint)sizeInBytes, (void*)0, bufferStorageMask);
    }

    public GLBuffer(int sizeInBytes, void* data, BufferStorageMask bufferStorageMask = BufferStorageMask.DynamicStorageBit)
        : this((nuint)sizeInBytes, data, bufferStorageMask)
    { }

    public GLBuffer(nuint sizeInBytes, void* data, BufferStorageMask bufferStorageMask = BufferStorageMask.DynamicStorageBit)
    {
        Handle = new GLObjectHandle(GL.CreateBuffer(), GLObjectType.Buffer);
        SizeInBytes = sizeInBytes;
        GL.NamedBufferStorage(Handle.Value, (nint)sizeInBytes, data, bufferStorageMask);
    }

    public GLBuffer(ReadOnlySpan<byte> data, BufferStorageMask bufferStorageMask = BufferStorageMask.DynamicStorageBit)
    {
        Handle = new GLObjectHandle(GL.CreateBuffer(), GLObjectType.Buffer);
        SizeInBytes = (nuint)data.Length;
        GL.NamedBufferStorage(Handle.Value, data.Length, data, bufferStorageMask);
    }
    /// <inheritdoc cref="Fill{T}(SizedInternalFormat, PixelFormat, PixelType, T)"/>
    public void Fill(SizedInternalFormat destFormat, PixelFormat srcPixelFormat, PixelType srcPixelType, void* data)
    {
        GL.ClearNamedBufferData(Handle.Value, destFormat, srcPixelFormat, srcPixelType, data);
    }

    /// <summary>
    /// Fills the buffer with a fixed value. Can transform data layout during fill
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="destFormat">Format of how the data will be stored in the buffer</param>
    /// <param name="srcPixelFormat">Pixelformat of the source data</param>
    /// <param name="srcPixelType">PixelType of the source data</param>
    /// <param name="data">Data value to fill the buffer with</param>
    public void Fill<T>(SizedInternalFormat destFormat, PixelFormat srcPixelFormat, PixelType srcPixelType, T data) where T : unmanaged
    {
        GL.ClearNamedBufferData(Handle.Value, destFormat, srcPixelFormat, srcPixelType, ref data);
    }

    /// <summary>
    /// Maps a host buffer into client visible memory
    /// </summary>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glMapBuffer.xhtml"/></remarks>
    /// <param name="bufferAccess">Flags specifying what operations are allowed on the returned pointer</param>
    /// <returns>A pointer to read/write from depending on <paramref name="bufferAccess"/> flags set</returns>
    public void* MapBuffer(BufferAccess bufferAccess)
    {
        return GL.MapNamedBuffer(Handle.Value, bufferAccess);
    }

    /// <inheritdoc cref="MapBufferRange(nuint, nuint, MapBufferAccessMask)"/>
    public void* MapBuffer(MapBufferAccessMask bufferAccess)
    {
        return MapBufferRange((nuint)0, (nuint)SizeInBytes, bufferAccess);
    }

    /// <inheritdoc cref="MapBufferRange(nuint, nuint, MapBufferAccessMask)"/>
    public void* MapBufferRange(int offsetInBytes, int lengthInBytes, MapBufferAccessMask bufferAccess)
    {
        return MapBufferRange((nuint)offsetInBytes, (nuint)lengthInBytes, bufferAccess);
    }

    /// <summary>
    /// Maps a range from a host buffer into client visible memory
    /// </summary>
    /// <remarks><see href="https://www.khronos.org/registry/OpenGL-Refpages/gl4/html/glMapBufferRange.xhtml"/></remarks>
    /// <param name="bufferAccess">Flags specifying what operations are allowed on the returned pointer</param>
    /// <param name="offsetInBytes">Offset in bytes from the beginning of the GLBuffer</param>
    /// <param name="lengthInBytes">Length in bytes of accessible memory</param>
    /// <returns>A pointer to read/write from depending on <paramref name="bufferAccess"/> flags set</returns>
    public void* MapBufferRange(nuint offsetInBytes, nuint lengthInBytes, MapBufferAccessMask bufferAccess)
    {
        return GL.MapNamedBufferRange(Handle.Value, (nint)offsetInBytes, (nint)lengthInBytes, bufferAccess);
    }

    /// <summary>
    /// Maps a range from a host buffer into client visible memory
    /// </summary>
    /// <returns>Boolean indicating success when true/error when false</returns>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glUnmapBuffer.xhtml"/></remarks>
    public bool UnmapBuffer()
    {
        return GL.UnmapNamedBuffer(Handle.Value);
    }

    /// <inheritdoc cref="UpdateData(nuint, nuint, void*)"/>
    public void UpdateData(int offsetInBytes, int dataSizeInBytes, void* data)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offsetInBytes);
        ArgumentOutOfRangeException.ThrowIfNegative(dataSizeInBytes);
        UpdateData((nuint)offsetInBytes, (nuint)dataSizeInBytes, data);
    }

    /// <summary>
    /// Updates the data in the buffer, by uploading data from client side to host side
    /// </summary>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferSubData.xhtml"/></remarks>
    /// <param name="offsetInBytes">Offset into the buffer at which to begin writing</param>
    /// <param name="dataSizeInBytes">Size of bytes to write</param>
    /// <param name="data">Pointer to data to write</param>
    public void UpdateData(nuint offsetInBytes, nuint dataSizeInBytes, void* data)
    {
        GL.NamedBufferSubData(Handle.Value, (nint)offsetInBytes, (nint)dataSizeInBytes, data);
    }

    /// <summary>
    /// Updates the data in the buffer, by uploading data from client side to host side
    /// </summary>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferSubData.xhtml"/></remarks>
    /// <param name="offsetInBytes">Offset into the buffer at which to begin writing</param>
    /// <param name="data">ReadOnlySpan of data to write</param>
    public void UpdateData(nuint offsetInBytes, ReadOnlySpan<byte> data)
    {
        GL.NamedBufferSubData(Handle.Value, (nint)offsetInBytes, data.Length, data);
    }

    /// <summary>
    /// Binds this GLBuffer to the state as the active GLBuffer for <paramref name="bufferTarget"/>
    /// </summary>
    /// <param name="bufferTarget">The buffer target to bind this buffer to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindBuffer.xhtml"/></remarks>
    public void Bind(BufferTarget bufferTarget)
    {
        GL.BindBuffer(bufferTarget, Handle.Value);
    }

    /// <summary>
    /// Unbinds any GLBuffer from the state for <paramref name="bufferTarget"/>
    /// </summary>
    /// <param name="bufferTarget">The buffer target to unbind</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindBuffer.xhtml"/></remarks>
    public static void Unbind(BufferTarget bufferTarget)
    {
        GL.BindBuffer(bufferTarget, 0);
    }

    /// <inheritdoc cref="BindBase(BufferTarget, uint)"/>
    public void BindBase(BufferTarget bufferTarget, int index = 0) => BindBase(bufferTarget, (uint)index);

    /// <summary>
    /// Binds a GLBuffer object to an indexed buffer target
    /// </summary>
    /// <param name="bufferTarget">The buffer target to bind to</param>
    /// <param name="index">The index on the buffer target to bind to (default = 0)</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindBufferBase.xhtml"/></remarks>
    public void BindBase(BufferTarget bufferTarget, uint index = 0)
    {
        GL.BindBufferBase(bufferTarget, index, Handle.Value);
    }

    public void Dispose()
    {
        Handle.Dispose();
    }

    public override bool Equals(object? obj)
    {
        return obj is GLBuffer gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public static bool operator ==(GLBuffer left, GLBuffer right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLBuffer left, GLBuffer right)
    {
        return !(left == right);
    }

    public bool Equals(GLBuffer other)
    {
        return Handle.Equals(other.Handle);
    }
}

public readonly unsafe struct GLBuffer<T> : IDisposable, IEquatable<GLBuffer<T>> where T : unmanaged
{
    public readonly GLBuffer RawBuffer;
    public nuint Length => RawBuffer.SizeInBytes / (nuint)Unsafe.SizeOf<T>();

    [Obsolete($"The paramaterless constructor or default({nameof(GLBuffer<T>)}) creates an invalid {nameof(GLBuffer<T>)}", true)]
    public GLBuffer()
    {
        ThrowHelper.ThrowInvalidOperationException($"Creates an invalid {nameof(GLBuffer<T>)}");
    }

    public GLBuffer(GLBuffer rawBuffer)
    {
        RawBuffer = rawBuffer;
    }

    public GLBuffer(int elementCount, BufferStorageMask bufferStorageMask = BufferStorageMask.DynamicStorageBit) :
        this((nuint)elementCount, bufferStorageMask)
    { }

    public GLBuffer(nuint elementCount, BufferStorageMask bufferStorageMask = BufferStorageMask.DynamicStorageBit)
    {
        RawBuffer = new GLBuffer(elementCount * (nuint)Unsafe.SizeOf<T>(), bufferStorageMask);
    }

    public GLBuffer(ReadOnlySpan<T> data, BufferStorageMask bufferStorageMask = BufferStorageMask.DynamicStorageBit)
    {
        fixed (void* ptr = data)
        {
            RawBuffer = new GLBuffer((nuint)data.Length * (nuint)Unsafe.SizeOf<T>(), ptr, bufferStorageMask);
        }
    }

    public void Dispose()
    {
        RawBuffer.Dispose();
    }

    /// <inheritdoc cref="GLBuffer.MapBuffer(BufferAccess)"/>
    public void* MapBuffer(BufferAccess bufferAccess)
    {
        return RawBuffer.MapBuffer(bufferAccess);
    }

    /// <inheritdoc cref="MapBufferRange(uint, uint, MapBufferAccessMask)"/>
    public T* MapBuffer(MapBufferAccessMask bufferAccess)
    {
        return MapBufferRange((uint)0, (uint)RawBuffer.SizeInBytes, bufferAccess);
    }

    /// <inheritdoc cref="MapBufferRange(uint, uint, MapBufferAccessMask)"/>
    public void* MapBufferRange(int offset, int length, MapBufferAccessMask bufferAccess)
    {
        return MapBufferRange((uint)offset, (uint)length, bufferAccess);
    }

    /// <summary>
    /// Maps a range from a host buffer into client visible memory
    /// </summary>
    /// <remarks><see href="https://www.khronos.org/registry/OpenGL-Refpages/gl4/html/glMapBufferRange.xhtml"/></remarks>
    /// <param name="bufferAccess">Flags specifying what operations are allowed on the returned pointer</param>
    /// <param name="offset">Offset in elements from the beginning of the GLBuffer</param>
    /// <param name="length">Length in elements of accessible memory</param>
    /// <returns>A pointer to read/write from depending on <paramref name="bufferAccess"/> flags set</returns>
    public T* MapBufferRange(uint offset, uint length, MapBufferAccessMask bufferAccess)
    {
        nuint offsetInBytes = (nuint)offset * (nuint)Unsafe.SizeOf<T>();
        nuint lengthInBytes = (nuint)Unsafe.SizeOf<T>() * (nuint)length;
        return (T*)RawBuffer.MapBufferRange(offsetInBytes, lengthInBytes, bufferAccess);
    }

    /// <inheritdoc cref="GLBuffer.UnmapBuffer"/>
    public bool UnmapBuffer()
    {
        return RawBuffer.UnmapBuffer();
    }

    /// <summary>
    /// Updates the data in the buffer, by uploading data from client side to host side
    /// </summary>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferSubData.xhtml"/></remarks>
    /// <param name="offset">Offset in elements into the buffer at which to begin writing</param>
    /// <param name="data">ReadOnlySpan of data to write</param>
    public void UpdateData(int offset, ReadOnlySpan<T> data)
    {
        fixed (void* ptr = data)
        {
            nuint offsetInBytes = (nuint)offset * (nuint)Unsafe.SizeOf<T>();
            nuint dataSizeInBytes = (nuint)Unsafe.SizeOf<T>() * (nuint)data.Length;
            RawBuffer.UpdateData(offsetInBytes, dataSizeInBytes, ptr);
        }
    }

    /// <summary>
    /// <inheritdoc cref="GLBuffer.Bind(BufferTarget)"/>
    /// </summary>
    public void Bind(BufferTarget bufferTarget)
    {
        RawBuffer.Bind(bufferTarget);
    }

    /// <summary>
    /// <inheritdoc cref="GLBuffer.BindBase(BufferTarget, int)"/>
    /// </summary>
    public void BindBase(BufferTarget bufferTarget, int index = 0) => BindBase(bufferTarget, (uint)index);

    /// <summary>
    /// <inheritdoc cref="GLBuffer.BindBase(BufferTarget, uint)"/>
    /// </summary>
    public void BindBase(BufferTarget bufferTarget, uint index = 0)
    {
        RawBuffer.BindBase(bufferTarget, index);
    }

    public static implicit operator GLBuffer(GLBuffer<T> buffer) => buffer.RawBuffer;

    public GLBuffer ToGLBuffer()
    {
        return RawBuffer;
    }

    public override bool Equals(object? obj)
    {
        return obj is GLBuffer<T> gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return RawBuffer.GetHashCode();
    }

    public static bool operator ==(GLBuffer<T> left, GLBuffer<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLBuffer<T> left, GLBuffer<T> right)
    {
        return !(left == right);
    }

    public bool Equals(GLBuffer<T> other)
    {
        return RawBuffer.Equals(other.RawBuffer);
    }
}