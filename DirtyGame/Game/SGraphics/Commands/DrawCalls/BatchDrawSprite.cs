using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.SGraphics.Commands.DrawCalls
{
    class BatchDrawSprite : DrawCall
    {
        private Texture2D texture;
        private Vector2 position;
        private Rectangle srcRect;
        private Color color;

        public BatchDrawSprite(Texture2D texture, Vector2 position, Rectangle srcRect, Color color) : base(RenderCommand.CommandType.BatchDrawSprite)
        {
            this.texture = texture;
            this.position = position;
            this.srcRect = srcRect;
            this.color = color;
        }

        public override void Execute(SpriteBatch spriteBatch)
        {
        	spriteBatch.Draw(texture, position, srcRect, color);          
        }
    }
}
