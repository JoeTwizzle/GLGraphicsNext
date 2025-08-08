using System.Runtime.CompilerServices;

namespace GLGraphicsNext;
/// <summary>
/// An OpenGL VertexArray object. Stores the memory layout of vertices and attached vertex- and index-buffers.
/// </summary>
public readonly unsafe struct GLVertexArray : IDisposable, IEquatable<GLVertexArray>
{
    public readonly GLObjectHandle Handle;

    public GLVertexArray()
    {
        Handle = new(GL.CreateVertexArray(), ObjectType.VertexArray);
    }

    public GLVertexArray(GLObjectHandle handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Sets the buffer as the index/element buffer for this GLVertexArray. Valid use implies <paramref name="buffer"/> is of type ElementArrayBuffer
    /// </summary>
    /// <param name="buffer">The buffer to use as the index/element buffer</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexArrayElementBuffer.xhtml"/></remarks>
    public void SetIndexBuffer(GLBuffer buffer)
    {
        GL.VertexArrayElementBuffer(Handle.Value, buffer.Handle.Value);
    }

    /// <summary>
    /// Sets a buffer as a vertex buffer for this GLVertexArray
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="stride">Number of bytes between each vertex</param>
    /// <param name="bindingIndex">The index of the vertex buffer binding point, to which to bind this buffer (default = 0)</param>
    /// <param name="firstElementOffsetInBytes">The offset of the first element of this buffer (default = 0)</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindVertexBuffer.xhtml"/></remarks>
    public void SetVertexBuffer(GLBuffer buffer, int stride, uint bindingIndex = 0, uint firstElementOffsetInBytes = 0)
    {
        GL.VertexArrayVertexBuffer(Handle.Value, bindingIndex, buffer.Handle.Value, (nint)firstElementOffsetInBytes, stride);
    }

    /// <inheritdoc cref="SetVertexBuffer(GLBuffer, int, uint, uint)"/>
    public void SetVertexBuffer<T>(GLBuffer<T> buffer, uint bindingIndex = 0, int firstElementOffsetInBytes = 0) where T : unmanaged
    {
        GL.VertexArrayVertexBuffer(Handle.Value, bindingIndex, buffer.RawBuffer.Handle.Value, (nint)firstElementOffsetInBytes, Unsafe.SizeOf<T>());
    }

    /// <inheritdoc cref="ConfigureLayoutFloat(uint, VertexAttribType, int, uint, uint, bool, uint)"/>
    public void ConfigureLayoutInt(int location, VertexAttribType vertexAttribType, int elements, int offsetInBytes, int divisor = 0, bool normalized = false, int vertexBufferBindingIndex = 0)
    {
        ConfigureLayoutFloat((uint)location, vertexAttribType, (uint)elements, (uint)offsetInBytes, (uint)divisor, normalized, (uint)vertexBufferBindingIndex);
    }

    /// <summary>
    /// Configures the layout for a vertex parameter that should be interpreted as a float
    /// </summary>
    /// <param name="location">Which location in shader programs is the current element bound to</param>
    /// <param name="vertexAttribType">What data type is the current element</param>
    /// <param name="elements">Number of elements in the current element (E.g: 3 for Vector3)</param>
    /// <param name="offsetInBytes">Offset in bytes from the beginning of the current vertex</param>
    /// <param name="vertexBufferBindingIndex">BindingIndex for which vertex buffer is this layout</param>
    /// <param name="divisor">Used for instanced rendering. <see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexBindingDivisor.xhtml"/> </param>
    /// <param name="normalized">If integer types should be normalized between 0 and 1</param>
    /// <remarks>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glEnableVertexAttribArray.xhtml"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexAttribFormat.xhtml"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexAttribBinding.xhtml"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexBindingDivisor.xhtml"/></para>
    /// </remarks>
    public void ConfigureLayoutFloat(uint location, VertexAttribType vertexAttribType, uint elements, uint offsetInBytes, uint divisor = 0, bool normalized = false, uint vertexBufferBindingIndex = 0)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(elements, 4u);
        GL.EnableVertexArrayAttrib(Handle.Value, location);
        GL.VertexArrayAttribFormat(Handle.Value, location, (int)elements, vertexAttribType, normalized, offsetInBytes);
        GL.VertexArrayAttribBinding(Handle.Value, location, vertexBufferBindingIndex);
        GL.VertexArrayBindingDivisor(Handle.Value, vertexBufferBindingIndex, divisor);
    }

    /// <inheritdoc cref="ConfigureLayoutInt(uint, VertexAttribLType, uint, uint, uint, uint)"/>
    public void ConfigureLayoutInt(int location, VertexAttribIType vertexAttribType, int elements, int offsetInBytes, int divisor = 0, int vertexBufferBindingIndex = 0)
    {
        ConfigureLayoutInt((uint)location, vertexAttribType, (uint)elements, (uint)offsetInBytes, (uint)divisor, (uint)vertexBufferBindingIndex);
    }

    /// <summary>
    /// Configures the layout for a vertex parameter that should be interpreted as an integer
    /// </summary>
    /// <param name="location">Which location in shader programs is the current element bound to</param>
    /// <param name="vertexAttribType">What data type is the current element</param>
    /// <param name="elements">Number of elements in the current element (E.g: 3 for Vector3)</param>
    /// <param name="offsetInBytes">Offset in bytes from the beginning of the current vertex</param>
    /// <param name="vertexBufferBindingIndex">BindingIndex for which vertex buffer is this layout</param>
    /// <param name="divisor">Used for instanced rendering. <see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexBindingDivisor.xhtml"/> </param>
    /// <remarks>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glEnableVertexAttribArray.xhtml"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexAttribFormat.xhtml"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexAttribBinding.xhtml"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexBindingDivisor.xhtml"/></para>
    /// </remarks>
    public void ConfigureLayoutInt(uint location, VertexAttribIType vertexAttribType, uint elements, uint offsetInBytes, uint divisor = 0, uint vertexBufferBindingIndex = 0)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(elements, 4u);
        GL.EnableVertexArrayAttrib(Handle.Value, location);
        GL.VertexArrayAttribIFormat(Handle.Value, location, (int)elements, vertexAttribType, offsetInBytes);
        GL.VertexArrayAttribBinding(Handle.Value, location, vertexBufferBindingIndex);
        GL.VertexArrayBindingDivisor(Handle.Value, vertexBufferBindingIndex, divisor);
    }

    /// <inheritdoc cref="ConfigureLayoutLong(uint, VertexAttribLType, uint, uint, uint, uint)"/>
    public void ConfigureLayoutLong(int location, VertexAttribLType vertexAttribType, int elements, int offsetInBytes, int divisor = 0, int vertexBufferBindingIndex = 0)
    {
        ConfigureLayoutLong((uint)location, vertexAttribType, (uint)elements, (uint)offsetInBytes, (uint)divisor, (uint)vertexBufferBindingIndex);
    }

    /// <summary>
    /// Configures the layout for a vertex parameter that should be interpreted as a long integer
    /// </summary>
    /// <param name="location">Which location in shader programs is the current element bound to</param>
    /// <param name="vertexAttribType">What data type is the current element</param>
    /// <param name="elements">Number of elements in the current element (E.g: 3 for Vector3)</param>
    /// <param name="offsetInBytes">Offset in bytes from the beginning of the current vertex</param>
    /// <param name="vertexBufferBindingIndex">BindingIndex for which vertex buffer is this layout</param>
    /// <param name="divisor">Used for instanced rendering. <see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexBindingDivisor.xhtml"/> </param>
    /// <remarks>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glEnableVertexAttribArray.xhtml"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexAttribFormat.xhtml"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexAttribBinding.xhtml"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexBindingDivisor.xhtml"/></para>
    /// </remarks>
    public void ConfigureLayoutLong(uint location, VertexAttribLType vertexAttribType, uint elements, uint offsetInBytes, uint divisor = 0, uint vertexBufferBindingIndex = 0)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(elements, 4u);
        GL.EnableVertexArrayAttrib(Handle.Value, location);
        GL.VertexArrayAttribLFormat(Handle.Value, location, (int)elements, vertexAttribType, offsetInBytes);
        GL.VertexArrayAttribBinding(Handle.Value, location, vertexBufferBindingIndex);
        GL.VertexArrayBindingDivisor(Handle.Value, vertexBufferBindingIndex, divisor);
    }

    /// <summary>
    /// Binds this GLVertexArray to the state as the active GLVertexArray
    /// </summary>
    public void Bind()
    {
        GL.BindVertexArray(Handle.Value);
    }

    /// <summary>
    /// Unbinds any GLVertexArray currently bound to the state
    /// </summary>
    public static void Unbind()
    {
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        Handle.Dispose();
    }

    public override bool Equals(object? obj)
    {
        return obj is GLVertexArray gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public static bool operator ==(GLVertexArray left, GLVertexArray right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLVertexArray left, GLVertexArray right)
    {
        return !(left == right);
    }

    public bool Equals(GLVertexArray other)
    {
        return Handle.Equals(other.Handle);
    }
}
