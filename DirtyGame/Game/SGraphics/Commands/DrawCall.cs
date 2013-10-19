using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.SGraphics.Commands
{
    abstract class DrawCall : RenderCommand
    {
        public DrawCall(CommandType type) : base(type)
        {
        }
    }
}
