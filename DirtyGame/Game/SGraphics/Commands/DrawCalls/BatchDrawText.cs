using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.SGraphics.Commands.DrawCalls
{
    class BatchDrawText : DrawCall
    {
        public BatchDrawText()
            : base(RenderCommand.CommandType.BatchDrawText)
        {

        }

        public override void Execute(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}
