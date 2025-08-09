using OpenTK.Mathematics;

namespace GLGraphicsNext;

/// <summary>
/// Represents an OpenGL Sampler object. Do not confuse with GLSL samplers.
/// </summary>
/// <remarks>
/// <para><see href="https://www.khronos.org/opengl/wiki/sampler_Object"/></para>
/// <para><see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_sampler_objects.txt"/></para>
/// </remarks>
public readonly unsafe struct GLSampler : IDisposable, IEquatable<GLSampler>
{
    public readonly GLObjectHandle Handle;

    public GLSampler()
    {
        Handle = new GLObjectHandle(GL.CreateSampler(), GLObjectType.Sampler);
    }

    public GLSampler(GLObjectHandle handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Binds this GLSampler to the state as the active GLSampler for this binding index
    /// </summary>
    /// <param name="bindingIndex">Specifies the binding index that this sampler should be bound to</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindSampler.xhtml"/></remarks>
    public void Bind(uint bindingIndex = 0)
    {
        GL.BindSampler(bindingIndex, Handle.Value);
    }

    /// <summary>
    /// Unbinds any GLSampler from the state for <paramref name="bindingIndex"/>
    /// </summary>
    /// <param name="bindingIndex">Specifies the binding index that should be unbound</param>
    /// <remarks><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindSampler.xhtml"/></remarks>
    public static void Unbind(uint bindingIndex = 0)
    {
        GL.BindSampler(bindingIndex, 0);
    }

    /// <summary>
    /// The filter used when sampling the texture at a lower than native resolution
    /// </summary>
    /// <param name="linearFiltering">Should linear filtering be performed</param>
    /// <param name="useMipmapping">Should mipmaps be used</param>
    /// <param name="mipmapLinearFiltering">Should linear filtering be performed between mip levels</param>
    /// <remarks><see href="https://www.khronos.org/opengl/wiki/sampler_Object#Filtering"/></remarks>
    public void SetMinFilter(bool linearFiltering, bool useMipmapping, bool mipmapLinearFiltering)
    {
        TextureMinFilter filter;
        if (useMipmapping)
        {
            if (mipmapLinearFiltering)
            {
                filter = linearFiltering ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.NearestMipmapLinear;
            }
            else
            {
                filter = linearFiltering ? TextureMinFilter.LinearMipmapNearest : TextureMinFilter.NearestMipmapNearest;
            }
        }
        else
        {
            filter = linearFiltering ? TextureMinFilter.Linear : TextureMinFilter.Nearest;
        }

        GL.SamplerParameteri(Handle.Value, SamplerParameterI.TextureMinFilter, (int)filter);
    }

    /// <summary>
    /// The filter used when sampling the texture at a higher than native resolution 
    /// </summary>
    /// <param name="linearFiltering">Should linear filtering be performed</param>
    /// <remarks><see href="https://www.khronos.org/opengl/wiki/sampler_Object#Filtering"/></remarks>
    public void SetMagFilter(bool linearFiltering)
    {
        GL.SamplerParameteri(Handle.Value, SamplerParameterI.TextureMagFilter, (int)(linearFiltering ? TextureMagFilter.Linear : TextureMagFilter.Nearest));
    }

    /// <summary>
    /// Sets the 'maximum degree of anisotropy' used for sampling
    /// </summary>
    /// <param name="value">A value >= 1.0f describing the 'maximum degree of anisotropy' used when sampling</param>
    /// <remarks>
    /// <para><see href="https://registry.khronos.org/OpenGL/extensions/EXT/EXT_texture_filter_anisotropic.txt"/> </para>
    /// <para><see href="https://www.khronos.org/opengl/wiki/sampler_Object#Anisotropic_filtering"/>              </para>
    /// </remarks>
    public void SetAnisotropicFilter(float value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, 1f);
        GL.SamplerParameterf(Handle.Value, SamplerParameterF.TextureMaxAnisotropy, value);
    }
    /// <summary>
    /// Sets the bias for texture level of detail calculations
    /// </summary>
    /// <param name="bias">Value to bias texture level of detail calculations by</param>
    /// <remarks><see href="https://www.khronos.org/opengl/wiki/sampler_Object#LOD_bias"/></remarks>
    public void SetLodBias(float bias)
    {
        GL.SamplerParameterf(Handle.Value, SamplerParameterF.TextureLodBias, bias);
    }

    /// <summary>
    /// Sets the sampling behaviour for texture coordinates outside of the 0.0 .. 1.0 range. 
    /// <para />
    /// Allows for four different modes: 
    /// <code>
    /// (false, false) => TextureWrapMode.Repeat,
    /// (true, false) => TextureWrapMode.MirroredRepeat,
    /// (false, true) => TextureWrapMode.ClampToEdge,
    /// (true, true) => TextureWrapMode.MirrorClampToEdge,
    /// </code>
    /// </summary>
    /// <param name="texcoord">Selects which texture coordinate 'uvw' to modify (valid range 0 .. 2)</param>
    /// <param name="mirror">Should this texture coordinate be mirrored</param>
    /// <param name="clamp">Should this texture coordinate be clamped</param>
    /// <remarks><see href="https://www.khronos.org/opengl/wiki/sampler_Object#Edge_value_sampling"/></remarks>
    public void SetEdgeSamplingMode(int texcoord, bool mirror, bool clamp)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(texcoord);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(texcoord, 3);

        SamplerParameterI wrapDim = texcoord switch
        {
            0 => SamplerParameterI.TextureWrapS,
            1 => SamplerParameterI.TextureWrapT,
            2 => SamplerParameterI.TextureWrapR,
            _ => 0
        };

        TextureWrapMode wrapMode = (mirror, clamp) switch
        {
            (false, false) => TextureWrapMode.Repeat,
            (true, false) => TextureWrapMode.MirroredRepeat,
            (false, true) => TextureWrapMode.ClampToEdge,
            (true, true) => (TextureWrapMode)All.MirrorClampToEdge,
        };

        GL.SamplerParameteri(Handle.Value, wrapDim, (int)wrapMode);
    }

    /// <summary>
    /// Sets the sampling behaviour for texture coordinates outside of the 0.0 .. 1.0 range to <c>TextureWrapMode.ClampToBorder</c> 
    /// </summary>
    /// <param name="texcoord">Selects which texture coordinate 'uvw' to modify (valid range 0 .. 2)</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetEdgeSamplingModeBorderClamp(int texcoord)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(texcoord);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(texcoord, 3);

        SamplerParameterI wrapDim = texcoord switch
        {
            0 => SamplerParameterI.TextureWrapS,
            1 => SamplerParameterI.TextureWrapT,
            2 => SamplerParameterI.TextureWrapR,
            _ => 0
        };

        GL.SamplerParameteri(Handle.Value, wrapDim, (int)TextureWrapMode.ClampToBorder);
    }

    /// <summary>
    /// Sets the border color used when <see cref="SetEdgeSamplingModeBorderClamp"/> is used
    /// </summary>
    /// <param name="color">The RGBA color value</param>
    public void SetBorderColor(Color4<Rgba> color)
    {
        GL.SamplerParameterfv(Handle.Value, SamplerParameterF.TextureBorderColor, &color.X);
    }

    /// <inheritdoc cref="SetBorderColor(Color4{Rgba})"/>
    public void SetBorderColor(Vector4 color)
    {
        GL.SamplerParameterfv(Handle.Value, SamplerParameterF.TextureBorderColor, &color.X);
    }

    /// <inheritdoc cref="SetBorderColor(Color4{Rgba})"/>
    public void SetBorderColor(Vec4 color)
    {
        GL.SamplerParameterfv(Handle.Value, SamplerParameterF.TextureBorderColor, &color.X);
    }

    /// <summary>
    /// Enables the texture comparison mode for currently bound depth textures.
    /// </summary>
    /// <param name="enabled"></param>
    /// <remarks><see cref="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glSamplerParameter.xhtml"/></remarks>
    public void SetCompareMode(bool enabled)
    {
        int state = enabled ? 1 : 0;
        GL.SamplerParameteriv(Handle.Value, SamplerParameterI.TextureCompareMode, &state);
    }

    /// <summary>
    /// Specifies the texture comparison mode for currently bound depth textures, the compare mode must be enabled for this setting to show an effect.
    /// </summary>
    /// <param name="enabled"></param>
    /// <remarks>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glSamplerParameter.xhtml"/></para>
    /// <para><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexImage2D.xhtml"/></para>
    /// </remarks>
    public void SetCompareFunction(DepthFunction compareFunction)
    {
        int func = (int)compareFunction;
        GL.SamplerParameteriv(Handle.Value, SamplerParameterI.TextureCompareFunc, &func);
    }

    public void Dispose()
    {
        Handle.Dispose();
    }

    public override bool Equals(object? obj)
    {
        return obj is GLSampler gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public static bool operator ==(GLSampler left, GLSampler right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLSampler left, GLSampler right)
    {
        return !(left == right);
    }

    public bool Equals(GLSampler other)
    {
        return Handle.Equals(other.Handle);
    }
}
