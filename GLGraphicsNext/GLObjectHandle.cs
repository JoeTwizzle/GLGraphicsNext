using System.Diagnostics;

namespace GLGraphicsNext;

/// <summary>
/// A handle representing one of the specifc OpenGL objects, as denoted by <see cref="ObjectType"/>.
/// </summary>
public readonly struct GLObjectHandle : IDisposable, IEquatable<GLObjectHandle>
{
    public readonly int Value;
    public readonly ObjectType ObjectType;
    public bool IsValid => Value != 0 && ObjectType != ObjectType.None;

    [Obsolete($"The paramaterless constructor or default({nameof(GLObjectHandle)}) creates an invalid {nameof(GLObjectHandle)}", true)]
    public GLObjectHandle()
    {
        ThrowHelper.ThrowInvalidOperationException($"Creates an invalid {nameof(GLObjectHandle)}");
    }

    public GLObjectHandle(int value, ObjectType objectType)
    {
        Value = value;
        ObjectType = objectType;
    }
    public static GLObjectHandle Create(int value, ObjectType objectType)
    {
        return new GLObjectHandle(value, objectType);
    }

    public void Dispose()
    {
        switch (ObjectType)
        {
            case ObjectType.VertexArray:
                GL.DeleteVertexArray(Value);
                break;
            case ObjectType.Program:
                GL.DeleteProgram(Value);
                break;
            case ObjectType.FrameBuffer:
                GL.DeleteFramebuffer(Value);
                break;
            case ObjectType.Sampler:
                GL.DeleteSampler(Value);
                break;
            case ObjectType.ProgramPipeline:
                GL.DeleteProgramPipeline(Value);
                break;
            case ObjectType.Texture:
                GL.DeleteTexture(Value);
                break;
            case ObjectType.Buffer:
                GL.DeleteBuffer(Value);
                break;
            case ObjectType.RenderBuffer:
                GL.DeleteRenderbuffer(Value);
                break;
            case ObjectType.Query:
                GL.DeleteQuery(Value);
                break;
            case ObjectType.Shader:
                GL.DeleteShader(Value);
                break;
            case ObjectType.None:
            default:
                Debug.WriteLine("Tried to dispose of uninitialized GLObject");
                break;
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is GLObjectHandle glObjectHandle && Equals(glObjectHandle);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(GLObjectHandle left, GLObjectHandle right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLObjectHandle left, GLObjectHandle right)
    {
        return !(left == right);
    }

    public bool Equals(GLObjectHandle other)
    {
        return Value == other.Value && ObjectType == other.ObjectType;
    }
}
