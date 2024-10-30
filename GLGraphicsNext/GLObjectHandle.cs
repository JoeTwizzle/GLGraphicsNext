using System.Diagnostics;
using System.Reflection.Metadata;

namespace GLGraphicsNext;

public readonly struct GLObjectHandle : IDisposable, IEquatable<GLObjectHandle>
{
    public readonly int Value;
    public readonly GLObjectType ObjectType;
    public bool IsValid => Value != 0 && ObjectType != GLObjectType.None;

    [Obsolete($"The paramaterless constructor creates an invalid {nameof(GLObjectHandle)}")]
    public GLObjectHandle()
    { }

    public GLObjectHandle(int value, GLObjectType objectType)
    {
        Value = value;
        ObjectType = objectType;
    }

    public void Dispose()
    {
        switch (ObjectType)
        {
            case GLObjectType.VertexArray:
                GL.DeleteVertexArray(in Value);
                break;
            case GLObjectType.Program:
                GL.DeleteProgram(Value);
                break;
            case GLObjectType.FrameBuffer:
                GL.DeleteFramebuffer(in Value);
                break;
            case GLObjectType.Sampler:
                GL.DeleteSampler(in Value);
                break;
            case GLObjectType.ProgramPipeline:
                GL.DeleteProgramPipeline(in Value);
                break;
            case GLObjectType.Texture:
                GL.DeleteTexture(in Value);
                break;
            case GLObjectType.Buffer:
                GL.DeleteBuffer(in Value);
                break;
            case GLObjectType.RenderBuffer:
                GL.DeleteRenderbuffer(in Value);
                break;
            case GLObjectType.Query:
                GL.DeleteQuery(in Value);
                break;
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
