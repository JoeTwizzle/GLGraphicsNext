using OpenTK.Mathematics;
using System.Diagnostics;

namespace GLGraphicsNext;

/// <summary>
/// An OpenGL Program object. It represents an executeble GPU program build from one or more linked GLShaders.
/// </summary>
public readonly unsafe struct GLProgram : IDisposable, IEquatable<GLProgram>
{
    public readonly GLObjectHandle Handle;
    public GLProgram()
    {
        Handle = new GLObjectHandle(GL.CreateProgram(), GLObjectType.Program);
    }

    public GLProgram(GLObjectHandle handle)
    {
        Handle = handle;
    }

    public void Dispose()
    {
        Handle.Dispose();
    }

    /// <summary>
    /// Adds a shader to the GLProgram, which can then be linked into an executable program using <see cref="LinkProgram"/>
    /// </summary>
    /// <remarks>Any added shader may be disposed after linking with <see cref="LinkProgram"/></remarks>
    /// <param name="shader">The shader to add</param>
    public void AddShader(GLShader shader)
    {
        GL.AttachShader(Handle.Value, shader.Handle.Value);
    }

    /// <summary>
    /// Removes a previously added shader from the GLProgram before linking.
    /// </summary>
    /// <remarks>Why did you add it in the first place, if you're just going to remove it?</remarks>
    /// <param name="shader">The shader to Remove</param>
    public void RemoveShader(GLShader shader)
    {
        GL.DetachShader(Handle.Value, shader.Handle.Value);
    }

    /// <summary>
    /// Links the attached shaders into an executable program
    /// </summary>
    /// <remarks>Any added shader may be disposed after successful linking, without affecting functionality</remarks>
    /// <exception cref="InvalidOperationException"></exception>
    public void LinkProgram()
    {
        GL.LinkProgram(Handle.Value);
        GL.GetProgrami(Handle.Value, ProgramProperty.LinkStatus, out int code);
        if (code != (int)All.True)
        {
            GL.GetProgramInfoLog(Handle.Value, out var info);
            ThrowHelper.ThrowInvalidOperationException($"Error occurred whilst linking Program({Handle.Value}) {Environment.NewLine}{info}");
        }
    }

    /// <summary>
    /// Binds this GLProgram to the state as the active GLProgram
    /// </summary>
    public void Bind()
    {
        GL.UseProgram(Handle.Value);
    }

    /// <summary>
    /// Unbinds any GLProgram currently bound to the state
    /// </summary>
    public static void Unbind()
    {
        GL.UseProgram(0);
    }

    public void SetUniform<T>(int location, T value, bool transposeIfMatrix = true)
    {
        if (value is bool b)
        {
            SetUniform1(location, b ? 1 : 0);
        }
        else if (value is int i)
        {
            SetUniform1(location, i);
        }
        else if (value is uint ui)
        {
            SetUniform1(location, ui);
        }
        else if (value is float f1)
        {
            SetUniform1(location, f1);
        }
        else if (value is Vector2 v2)
        {
            SetUniform2(location, v2);
        }
        else if (value is Vector3 v3)
        {
            SetUniform3(location, v3);
        }
        else if (value is Vector4 v4)
        {
            SetUniform4(location, v4);
        }
        else if (value is Color4<Rgba> col)
        {
            SetUniform4(location, col);
        }
        else if (value is Matrix2 mat2)
        {
            SetUniformMatrix2(location, mat2, transposeIfMatrix);
        }
        else if (value is Matrix3 mat3)
        {
            SetUniformMatrix3(location, mat3, transposeIfMatrix);
        }
        else if (value is Matrix4 mat4)
        {
            SetUniformMatrix4(location, mat4, transposeIfMatrix);
        }
        else
        {
            Debug.WriteLine("The type " + value?.GetType() + " is not a valid type for uniforms.");
        }
    }

    public void SetUniformMatrix4(int location, Matrix4 mat4, bool rowMajor = true)
    {
        GL.ProgramUniformMatrix4f(Handle.Value, location, 1, rowMajor, ref mat4);
    }

    public void SetUniformMatrix3(int location, Matrix3 mat3, bool rowMajor = true)
    {
        GL.ProgramUniformMatrix3f(Handle.Value, location, 1, rowMajor, ref mat3);
    }

    public void SetUniformMatrix2(int location, Matrix2 mat2, bool rowMajor = true)
    {
        GL.ProgramUniformMatrix2f(Handle.Value, location, 1, rowMajor, ref mat2);
    }

    public void SetUniform4(int location, Color4<Rgba> val)
    {
        GL.ProgramUniform4fv(Handle.Value, location, 1, &val.X);
    }

    public void SetUniform4(int location, Vector4 val)
    {
        GL.ProgramUniform4fv(Handle.Value, location, 1, &val.X);
    }

    public void SetUniform3(int location, Vector3 val)
    {
        GL.ProgramUniform3fv(Handle.Value, location, 1, &val.X);
    }

    public void SetUniform2(int location, Vector2 val)
    {
        GL.ProgramUniform2fv(Handle.Value, location, 1, &val.X);
    }

    public void SetUniform1(int location, double val)
    {
        GL.ProgramUniform1d(Handle.Value, location, val);
    }

    public void SetUniform1(int location, float val)
    {
        GL.ProgramUniform1f(Handle.Value, location, val);
    }

    public void SetUniform1(int location, int val)
    {
        GL.ProgramUniform1i(Handle.Value, location, val);
    }

    public void SetUniform1(int location, uint val)
    {
        GL.ProgramUniform1ui(Handle.Value, location, val);
    }

    //Static

    public static void SetUniformMatrix4(int program, int location, Matrix4 mat4, bool rowMajor = true)
    {
        GL.ProgramUniformMatrix4f(program, location, 1, rowMajor, ref mat4);
    }

    public static void SetUniformMatrix4(int program, int location, Mat4 mat4, bool rowMajor = true)
    {
        GL.ProgramUniformMatrix4f(program, location, 1, rowMajor, ref mat4);
    }

    public static void SetUniformMatrix3(int program, int location, Matrix3 mat3, bool rowMajor = true)
    {
        GL.ProgramUniformMatrix3f(program, location, 1, rowMajor, ref mat3);
    }

    public static void SetUniformMatrix2(int program, int location, Matrix2 mat2, bool rowMajor = true)
    {
        GL.ProgramUniformMatrix2f(program, location, 1, rowMajor, ref mat2);
    }

    public static void SetUniform4<T>(int program, int location, Color4<T> val) where T : IColorSpace4
    {
        GL.ProgramUniform4fv(program, location, 1, &(val.X));
    }

    public static void SetUniform4(int program, int location, Vector4 val)
    {
        GL.ProgramUniform4fv(program, location, 1, &(val.X));
    }

    public static void SetUniform3(int program, int location, Vector3 val)
    {
        GL.ProgramUniform3fv(program, location, 1, &(val.X));
    }

    public static void SetUniform2(int program, int location, Vector2 val)
    {
        GL.ProgramUniform2fv(program, location, 1, &(val.X));
    }

    public static void SetUniform1(int program, int location, double val)
    {
        GL.ProgramUniform1d(program, location, val);
    }

    public static void SetUniform1(int program, int location, float val)
    {
        GL.ProgramUniform1f(program, location, val);
    }

    public static void SetUniform1(int program, int location, int val)
    {
        GL.ProgramUniform1i(program, location, val);
    }

    public static void SetUniform1(int program, int location, uint val)
    {
        GL.ProgramUniform1ui(program, location, val);
    }

    public override bool Equals(object? obj)
    {
        return obj is GLProgram gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public static bool operator ==(GLProgram left, GLProgram right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLProgram left, GLProgram right)
    {
        return !(left == right);
    }

    public bool Equals(GLProgram other)
    {
        return Handle.Equals(other.Handle);
    }
}
