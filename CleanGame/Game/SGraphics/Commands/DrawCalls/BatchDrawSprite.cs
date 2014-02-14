using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CleanGame.Game.SGraphics.Commands.DrawCalls
{
    class BatchDrawSprite : DrawCall
    {
        private Texture2D texture;
        private Vector2 position;
        private Rectangle srcRect;
        private Color color;
        private float angle;
        private Vector2 origin;
        private float scale;

        public BatchDrawSprite(Texture2D texture, Vector2 position, Rectangle srcRect, Color color) : this(texture, position, srcRect, color, 0, 1, Vector2.Zero)
        {
            
        }
        public BatchDrawSprite(Texture2D texture, Vector2 position, Rectangle srcRect, Color color, float angle, float scale, Vector2 origin) : base(RenderCommand.CommandType.BatchDrawSprite)
        {
            this.texture = texture;
            this.position = position;
            this.srcRect = srcRect;
            this.color = color;
            this.angle = angle;
            this.origin = origin;
            this.scale = scale;
        }

        public override void Execute(SpriteBatch spriteBatch, Renderer r)
        {
        	spriteBatch.Draw(texture, position, srcRect, color, angle, origin, scale, SpriteEffects.None, 0);          
        }
    }
}
