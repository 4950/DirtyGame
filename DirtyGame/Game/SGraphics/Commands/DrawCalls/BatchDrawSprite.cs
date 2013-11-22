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
        private float angle;
        private Vector2 origin;

        public BatchDrawSprite(Texture2D texture, Vector2 position, Rectangle srcRect, Color color) : this(texture, position, srcRect, color, 0, Vector2.Zero)
        {
        }
        public BatchDrawSprite(Texture2D texture, Vector2 position, Rectangle srcRect, Color color, float angle, Vector2 origin) : base(RenderCommand.CommandType.BatchDrawSprite)
        {
            this.texture = texture;
            this.position = position;
            this.srcRect = srcRect;
            this.color = color;
            this.angle = angle;
            this.origin = origin;
        }

        public override void Execute(SpriteBatch spriteBatch)
        {
        	spriteBatch.Draw(texture, position, srcRect, color, angle, origin, 1, SpriteEffects.None, 0);          
        }
    }
}
