using OpenTK.Mathematics;

namespace GLGraphicsNext;

/// <summary>
/// An OpenGL Framebuffer object. Stores the result of render operations in the attached textures.
/// </summary>
public unsafe readonly struct GLFramebuffer : IDisposable, IEquatable<GLFramebuffer>
{
    public readonly GLObjectHandle Handle;
    public GLFramebuffer()
    {
        Handle = new GLObjectHandle(GL.CreateFramebuffer(), ObjectType.FrameBuffer);
    }

    public GLFramebuffer(GLObjectHandle handle)
    {
        Handle = handle;
    }

    public static int GetAttachmentIndex(FramebufferAttachment framebufferAttachment)
    {
        int res = framebufferAttachment switch
        {
            FramebufferAttachment.DepthStencilAttachment => 0,
            FramebufferAttachment.DepthAttachment => 0,
            FramebufferAttachment.StencilAttachment => 0,
            _ => (int)framebufferAttachment - (int)FramebufferAttachment.ColorAttachment0,
        };
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)res, 31u);
        return res;
    }

    public void AttachTexture(GLTextureBase texture, FramebufferAttachment framebufferAttachment, int mipLevel = 0)
    {
        GL.NamedFramebufferTexture(Handle.Value, framebufferAttachment, texture.Handle.Value, mipLevel);
    }

    public void DetachTexture(FramebufferAttachment framebufferAttachment)
    {
        GL.NamedFramebufferTexture(Handle.Value, framebufferAttachment, 0, 0);
    }

    public FramebufferStatus GetStatus()
    {
        return GL.CheckNamedFramebufferStatus(Handle.Value, FramebufferTarget.Framebuffer);
    }

    public void ClearColorAttachment(int attachmentIndex, uint* value)
    {
        GL.ClearNamedFramebufferuiv(Handle.Value, Buffer.Color, attachmentIndex, value);
    }

    public void ClearColorAttachment(int attachmentIndex, ref uint value)
    {
        fixed (uint* ptr = &value)
        {
            GL.ClearNamedFramebufferuiv(Handle.Value, Buffer.Color, attachmentIndex, ptr);
        }
    }

    public void ClearColorAttachment(int attachmentIndex, Vector4i value)
    {
        GL.ClearNamedFramebufferiv(Handle.Value, Buffer.Color, attachmentIndex, &value.X);
    }

    public void ClearColorAttachment(int attachmentIndex, Color4<Rgba> value)
    {
        GL.ClearNamedFramebufferfv(Handle.Value, Buffer.Color, attachmentIndex, &value.X);
    }

    public void ClearStencilAttachment(int value)
    {
        GL.ClearNamedFramebufferiv(Handle.Value, Buffer.Stencil, 0, &value);
    }

    public void ClearDepthAttachment(float value)
    {
        GL.ClearNamedFramebufferfv(Handle.Value, Buffer.Depth, 0, &value);
    }

    /// <summary>
    /// Copies a region from this framebuffer into another framebuffer.
    /// Performs a resolve operation if source framebuffer is multisampled and destination framebuffer is not / has less samples. 
    /// </summary>
    /// <remarks> <paramref name="filter"/> must be Must be 'Nearest' if <paramref name="clearBufferMask"/> contains DepthBufferBit or StencilBufferBit</remarks>
    /// <param name="dest">Framebuffer to blit into</param>
    /// <param name="srcRect">Source region to sample from</param>
    /// <param name="destRect">Destination region to write into</param>
    /// <param name="clearBufferMask">Which</param>
    /// <param name="filter">Specifies the interpolation to be applied if the image is stretched. Must be 'Nearest' if <paramref name="clearBufferMask"/> contains DepthBufferBit or StencilBufferBit</param>
    public void Blit(GLFramebuffer dest, Box2i srcRect, Box2i destRect, ClearBufferMask clearBufferMask, BlitFramebufferFilter filter)
    {
        GL.BlitNamedFramebuffer(Handle.Value,
            dest.Handle.Value,
            srcRect.Min.X,
            srcRect.Min.Y,
            srcRect.Max.X,
            srcRect.Max.Y,
            destRect.Min.X,
            destRect.Min.Y,
            destRect.Max.X,
            destRect.Max.Y,
            clearBufferMask,
            filter);
    }

    /// <summary>
    /// Binds this GLFramebuffer to the state as the active GLFramebuffer
    /// </summary>
    /// <param name="framebufferTarget">Specifies the framebuffer target of the binding operation</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindFramebuffer.xhtml"/></remarks>
    public void Bind(FramebufferTarget framebufferTarget = FramebufferTarget.Framebuffer)
    {
        GL.BindFramebuffer(framebufferTarget, Handle.Value);
    }

    /// <summary>
    /// Binds the default framebuffer to the state
    /// </summary>
    public static void Unbind(FramebufferTarget framebufferTarget = FramebufferTarget.Framebuffer)
    {
        GL.BindFramebuffer(framebufferTarget, 0);
    }

    public void Dispose()
    {
        Handle.Dispose();
    }

    public override bool Equals(object? obj)
    {
        return obj is GLFramebuffer gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public static bool operator ==(GLFramebuffer left, GLFramebuffer right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLFramebuffer left, GLFramebuffer right)
    {
        return !(left == right);
    }

    public bool Equals(GLFramebuffer other)
    {
        return Handle.Equals(other.Handle);
    }
}
