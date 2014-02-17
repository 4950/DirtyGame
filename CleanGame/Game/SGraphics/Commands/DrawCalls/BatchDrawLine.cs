using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CleanGame.Game.SGraphics.Commands.DrawCalls
{
    class BatchDrawLine : DrawCall
    {
        Color color1;
        Color color2;
        Vector2 start;
        Vector2 end;

        public BatchDrawLine(Vector2 start, Vector2 end, Color color) : this(start, end, color, color)
        {
        }
        public BatchDrawLine(Vector2 start, Vector2 end, Color color1, Color color2)
            : base(RenderCommand.CommandType.BatchDrawLine)
        {
            this.start = start;
            this.end = end;
            this.color1 = color1;
            this.color2 = color2;
        }

        public override void Execute(SpriteBatch spriteBatch, Renderer r)
        {
            int width = 1;

            Rectangle rect = new Rectangle((int)start.X, (int)start.Y, (int)(end - start).Length() + width, width);
            Vector2 v = Vector2.Normalize(start - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (start.Y > end.Y) angle = MathHelper.TwoPi - angle;

            SamplerState prev = spriteBatch.GraphicsDevice.SamplerStates[0];
            spriteBatch.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
            spriteBatch.Draw(r.Pixel, rect, null, color1, angle, Vector2.Zero, SpriteEffects.None, 0);
            if(color1 != color2)
                spriteBatch.Draw(r.GradientPixel, rect, null, color2, angle, Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.GraphicsDevice.SamplerStates[0] = prev;    
        }
    }
}
