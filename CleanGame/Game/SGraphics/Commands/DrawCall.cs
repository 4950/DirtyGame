using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanGame.Game.SGraphics.Commands
{
    public abstract class DrawCall : RenderCommand
    {
        public DrawCall(CommandType type) : base(type)
        {
        }
    }
}
