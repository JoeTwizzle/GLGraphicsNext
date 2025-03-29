using OpenTK.Core.Utility;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example;
internal abstract class BasePal2Window
{
    protected WindowHandle window;
    protected OpenGLContextHandle contextHandle;
    public Vector2i FramebufferSize { get; private set; }

    public BasePal2Window()
    {
        GraphicsApi graphicsApi = GraphicsApi.OpenGL;
        ToolkitOptions options = new()
        {
            // ApplicationName is the name of the application
            // this is used on some platforms to show the application name.
            ApplicationName = "OpenTK tutorial",
            // Set the logger to use
            Logger = new ConsoleLogger()
        };

        Toolkit.Init(options);
        GraphicsApiHints contextSettings = graphicsApi switch
        {
            GraphicsApi.Vulkan => new VulkanGraphicsApiHints(),
            GraphicsApi.OpenGL => new OpenGLGraphicsApiHints()
            {
                // Here different options of the opengl context can be set.
                Version = new Version(4, 6),
                Profile = OpenGLProfile.Core,
                DebugFlag = true,
                DepthBits = ContextDepthBits.Depth24,
                StencilBits = ContextStencilBits.Stencil8,
            },
            _ => throw new NotImplementedException(),
        };
        window = Toolkit.Window.Create(contextSettings);

        switch (graphicsApi)
        {
            case GraphicsApi.OpenGL:
                {
                    var handle = Toolkit.OpenGL.CreateFromWindow(window);
                    contextHandle = handle;
                    // Set the current opengl context and load the bindings.
                    Toolkit.OpenGL.SetCurrentContext(handle);
                    GLLoader.LoadBindings(Toolkit.OpenGL.GetBindingsContext(handle));
                }
                break;
            default:
                throw new NotImplementedException();
        }

        //Register event handlers
        EventQueue.EventRaised += EventRaised;
        // Set the title of the window
        Toolkit.Window.SetTitle(window, "OpenTK window");
        // Set the size of the window
        Toolkit.Window.SetSize(window, new(800, 600));
        // Bring the window out of the default Hidden window mode
        Toolkit.Window.SetMode(window, WindowMode.Normal);
    }

    void EventRaised(PalHandle? handle, PlatformEventType type, EventArgs args)
    {
        if (args is CloseEventArgs closeArgs)
        {
            // Destroy the window that the user wanted to close.
            Toolkit.Window.Destroy(closeArgs.Window);
        }
        if (args is WindowResizeEventArgs resizeEventArgs)
        {
            FramebufferResized(new(resizeEventArgs.NewClientSize.X, resizeEventArgs.NewClientSize.Y));
        }
        if (args is WindowFramebufferResizeEventArgs framebufferResizeEventArgs)
        {
            FramebufferResized(new(framebufferResizeEventArgs.NewFramebufferSize.X, framebufferResizeEventArgs.NewFramebufferSize.Y));
        }
    }

    public void Run()
    {
        InitRenderer();
        while (true)
        {
            // This will process events for all windows and
            //  post those events to the event queue.
            Toolkit.Window.ProcessEvents(false);
            // Check if the window was destroyed after processing events.
            if (Toolkit.Window.IsWindowDestroyed(window))
            {
                break;
            }
            Render();
        }
    }

    protected abstract void Render();
    protected abstract void FramebufferResized(Vector2i newSize);
    protected abstract void InitRenderer();
}
