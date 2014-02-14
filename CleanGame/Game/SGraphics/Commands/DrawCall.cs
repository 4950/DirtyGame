using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.SGraphics.Commands
{
    public abstract class DrawCall : RenderCommand
    {
        public DrawCall(CommandType type) : base(type)
        {
        }
    }
}
