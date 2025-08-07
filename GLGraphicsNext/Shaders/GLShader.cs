using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GLGraphicsNext;

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
    /// <exception cref="InvalidOperationException"></exception>
    public GLShader(ShaderType shaderType, string sourceText)
    {
        int handle = GL.CreateShader(shaderType);
        Handle = new(handle, GLObjectType.Shader);
        GL.ShaderSource(Handle.Value, sourceText);
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
