using System.Diagnostics;

namespace GLGraphicsNext;

/// <summary>
/// A handle representing one of the specifc OpenGL objects, as denoted by <see cref="ObjectType"/>.
/// </summary>
public readonly struct GLObjectHandle : IDisposable, IEquatable<GLObjectHandle>
{
    public readonly int Value;
    public readonly GLObjectType ObjectType;
    public bool IsValid => Value != 0 && ValidateGLObjectType();
    private readonly bool ValidateGLObjectType()
    {
        return ObjectType switch
        {
            GLObjectType.None => false,
            GLObjectType.VertexArray => GL.IsVertexArray(Value),
            GLObjectType.Program => GL.IsProgram(Value),
            GLObjectType.Sampler => GL.IsSampler(Value),
            GLObjectType.ProgramPipeline => GL.IsProgramPipeline(Value),
            GLObjectType.Buffer => GL.IsBuffer(Value),
            GLObjectType.Query => GL.IsQuery(Value),
            GLObjectType.Shader => GL.IsShader(Value),
            GLObjectType.Texture => GL.IsTexture(Value),
            GLObjectType.RenderBuffer => GL.IsRenderbuffer(Value),
            GLObjectType.FrameBuffer => GL.IsFramebuffer(Value),
            _ => false,
        };
    }

    [Obsolete($"The paramaterless constructor or default({nameof(GLObjectHandle)}) creates an invalid {nameof(GLObjectHandle)}", true)]
    public GLObjectHandle()
    {
        ThrowHelper.ThrowInvalidOperationException($"Creates an invalid {nameof(GLObjectHandle)}");
    }

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
                GL.DeleteVertexArray(Value);
                break;
            case GLObjectType.Program:
                GL.DeleteProgram(Value);
                break;
            case GLObjectType.FrameBuffer:
                GL.DeleteFramebuffer(Value);
                break;
            case GLObjectType.Sampler:
                GL.DeleteSampler(Value);
                break;
            case GLObjectType.ProgramPipeline:
                GL.DeleteProgramPipeline(Value);
                break;
            case GLObjectType.Texture:
                GL.DeleteTexture(Value);
                break;
            case GLObjectType.Buffer:
                GL.DeleteBuffer(Value);
                break;
            case GLObjectType.RenderBuffer:
                GL.DeleteRenderbuffer(Value);
                break;
            case GLObjectType.Query:
                GL.DeleteQuery(Value);
                break;
            case GLObjectType.Shader:
                GL.DeleteShader(Value);
                break;
            case GLObjectType.None:
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
