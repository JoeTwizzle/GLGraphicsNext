namespace GLGraphicsNext;

/// <summary>
/// An OpenGL Shader object. It represents a compiled program constructed from (usually GLSL) shader code.
/// </summary>
public readonly struct GLShader : IDisposable, IEquatable<GLShader>
{
    public readonly GLObjectHandle Handle;

    [Obsolete($"The paramaterless constructor or default({nameof(GLShader)}) creates an invalid {nameof(GLShader)}", true)]
    public GLShader()
    {
        ThrowHelper.ThrowInvalidOperationException($"Creates an invalid {nameof(GLShader)}");
    }

    public GLShader(GLObjectHandle handle)
    {
        Handle = handle;
    }
    /// <summary>
    /// Initializes a new shader object from source code. The shader must still be linked into a program before use.
    /// </summary>
    /// <param name="sourceText">The shaders source code text</param>
    /// <param name="shaderType">The shaders type / shader stage</param>
    /// <exception cref="InvalidOperationException"></exception>
    public GLShader(ShaderType shaderType, string sourceText)
    {
        int handle = GL.CreateShader(shaderType);
        Handle = new(handle, ObjectType.Shader);
        GL.ShaderSource(Handle.Value, sourceText);
        GL.CompileShader(Handle.Value);
        GL.GetShaderi(Handle.Value, ShaderParameterName.CompileStatus, out int code);
        if (code != (int)All.True)
        {
            GL.GetShaderInfoLog(Handle.Value, out var info);
            ThrowHelper.ThrowInvalidOperationException($"Error occurred whilst compiling Shader with handle: {Handle} {Environment.NewLine} {info}");
        }
    }
    /// <summary>
    /// Initializes a new shader object from source code. The shader must still be linked into a program before use.
    /// </summary>
    /// <param name="sourceTextUtf8">The shaders source code text encoded in utf8</param>
    /// <param name="shaderType">The shaders type / shader stage</param>
    /// <exception cref="InvalidOperationException"></exception>
    public unsafe GLShader(ShaderType shaderType, ReadOnlySpan<byte> sourceTextUtf8)
    {
        int handle = GL.CreateShader(shaderType);
        Handle = new(handle, ObjectType.Shader);
        int* lengths = stackalloc int[1] { sourceTextUtf8.Length };
        fixed (byte* sTextPtr = sourceTextUtf8)
        {
            byte** texts = stackalloc byte*[1] { sTextPtr };
            GL.ShaderSource(Handle.Value, 1, texts, lengths);
        }
        GL.CompileShader(Handle.Value);
        GL.GetShaderi(Handle.Value, ShaderParameterName.CompileStatus, out int code);
        if (code != (int)All.True)
        {
            GL.GetShaderInfoLog(Handle.Value, out var info);
            ThrowHelper.ThrowInvalidOperationException($"Error occurred whilst compiling Shader with handle: {Handle} {Environment.NewLine} {info}");
        }
    }

    public void Dispose()
    {
        Handle.Dispose();
    }

    public override bool Equals(object? obj)
    {
        return obj is GLShader gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public static bool operator ==(GLShader left, GLShader right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLShader left, GLShader right)
    {
        return !(left == right);
    }

    public bool Equals(GLShader other)
    {
        return Handle.Equals(other.Handle);
    }
}
