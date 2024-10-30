using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLGraphicsNext;

public enum GLObjectType
{
    None,
    VertexArray,
    Program,
    Sampler,
    ProgramPipeline,
    Buffer,
    Query,
    Shader,
    Texture,
    RenderBuffer,
    FrameBuffer,
}
