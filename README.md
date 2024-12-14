# GL Graphics NEXT
![](https://github.com/JoeTwizzle/GLGraphicsNext/blob/main/Logo.png)

A modern lightweight object oriented wrapper around the OpenGL API using OpenTK

# Nuget
GL Graphics NEXT is available as a nuget package 

|  Name  | Release |
|:---|----------------:|
|**GL Graphics NEXT**| [![NuGet](https://img.shields.io/nuget/v/GLGraphicsNext)](https://www.nuget.org/packages/GLGraphicsNext) |

# Features
* ### Lightweight struct based abstraction over almost all modern OpenGL primitives.
* ### Extensive API documentation and correct usage hints on API functions.
* ### A great middle ground between classic OpenGL and modern APIs like Vulkan
* ### No clunky old "bind" style OpenGL APIs are used.

# Remarks
This library requires support for OpenGL 4.5+.

This should include all dedicated GPUs and most integrated GPUs, with the notable exception of MacOS, as there OpenGL is considered 'deprecated' in favour of Metal.

It may still be possible to use this library and other modern OpenGL libraries on MacOS, by using the [Zink](https://docs.mesa3d.org/drivers/zink.html) implenmentation of OpenGL through [MoltenVK](https://github.com/KhronosGroup/MoltenVK) on MacOS.

# Implemented Features
* [x] FrameBuffers
* [x] All kinds of Buffers
* [x] All kinds of Textures
* [x] All kinds of TextureViews
* [x] All kinds of Samplers
* [x] Shader objects
* [x] ShaderProgram objects
* [x] Timer queries
* [x] Sample queries
* [ ] Texture only sample parameters (use sampler objects instead)

# Getting started
The following code renders a quad on the screen. 

It should serve as a starting point for any more complex renderer implementations.

Also see the implementation in the [Examples](https://github.com/JoeTwizzle/GLGraphicsNext/tree/main/Example) directory.
```cs
internal sealed class ExampleGame : BasePal2Window
{
    //The VertexArrayObject holding information about
    //how our vertices are layed out in memory and
    //from which buffers to pull data
    GLVertexArray VAO;

    //The GPU side buffer that holds our vertex data
    GLBuffer VertexBuffer;
    //The GPU side buffer that holds our index data
    GLBuffer IndexBuffer;

    //The GLProgram that decides how things are rendered
    GLProgram ShaderProgram;

    //The source code for our vertex shader
    const string vertexShaderSource =
        @"#version 330 core
        layout (location = 0) in vec3 pos;
        
        void main()
        {
            gl_Position = vec4(pos, 1.0);
        }";

    //The source code for our fragment shader
    const string fragmentShaderSource =
        @"#version 330 core
        out vec4 color;
        
        void main()
        {
            color = vec4(0.980, 0.118, 0.506, 1.0);
        }";

    readonly float[] vertices = //Vertices for our Square
    {
             0.5f,  0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            -0.5f,  0.5f, 0.0f
    };

    readonly uint[] indices = { 0, 1, 3, 1, 2, 3 }; //Indices for our Square
    protected override void InitRenderer()
    {
        //The OpenGL vertex input location 0 consists of 3 float values. 
        //The data is read with an offset of zero bytes.
        VAO = new GLVertexArray();
        VAO.ConfigureLayoutFloat(0, VertexAttribType.Float, 3, 0);


        //Initialize the buffer and fill it with the data of our vertices
        VertexBuffer = new GLBuffer<float>(vertices);


        //Initialize the buffer and fill it with the data of our indices
        IndexBuffer = new GLBuffer<uint>(indices);

        //Define how many bytes we need to advance, in order to find the next vertex
        //AKA how many bytes does one vertex take up
        const int stride = 3 * sizeof(float);

        //Assign our vertex and index buffers as the current targets of
        VAO.SetVertexBuffer(VertexBuffer, stride);
        VAO.SetIndexBuffer(IndexBuffer);

        //The VertexShader part of our shader Program
        GLShader vertexShader = new(ShaderType.VertexShader, vertexShaderSource);

        //The FragmentShader part of our shader Program
        GLShader fragmentShader = new(ShaderType.FragmentShader, fragmentShaderSource);


        //Create the Compiled Shader Program
        ShaderProgram = new GLProgram();
        //Add our shader parts to the final Shader Program
        ShaderProgram.AddShader(vertexShader);
        ShaderProgram.AddShader(fragmentShader);
        //Compile the program
        ShaderProgram.LinkProgram();

        //We can remove the fragment and vertex shader parts after compiling, to free memory
        ShaderProgram.RemoveShader(vertexShader);
        ShaderProgram.RemoveShader(fragmentShader);
        vertexShader.Dispose();
        fragmentShader.Dispose();
    }

    protected override void FramebufferResized(Vector2i newSize)
    {
        GL.Viewport(0, 0, newSize.X, newSize.Y);
    }

    protected override void Render()
    {
        GL.ClearColor(0.600f, 0.630f, 0.535f, 1f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        //Set our VAO as the active VAO
        VAO.Bind();
        //Set our shader program as the active Shader Program
        ShaderProgram.Bind();
        //Render our Square
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

        Toolkit.OpenGL.SwapBuffers(glContext);
    }
}
```
