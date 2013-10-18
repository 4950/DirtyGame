using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ShittyPrototype.src.application;
using ShittyPrototype.src.core;
using ShittyPrototype.src.graphics;
using ShittyPrototype.src.application.core;
using System.Diagnostics;

namespace ShittyPrototype
{
    class Renderer
    {
        private GraphicsDeviceManager _graphics;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch; 

        public Renderer(GraphicsDeviceManager graphicsManager) 
        {
            _graphics = graphicsManager;
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
        }

        public void RenderBatch(ICollection<Entity> entities, Camera cam, GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, cam.TransformMatrix);
            foreach (Entity entity in entities)
            {
                RenderComponent renderComp = (RenderComponent)entity.GetComponent<RenderComponent>();
                PositionComponent posComp = (PositionComponent) entity.GetComponent<PositionComponent>();
                if (renderComp != null)
                {
                    //Rectangle r = renderComp.rectangle;
                    //r.X += entity.GetHashCode() % 10;
                    //Debug.WriteLine(entity.GetHashCode() + " " + renderComp.rectangle.ToString());

                    if (renderComp.spriteRectangles != null)
                    {
                        _spriteBatch.Draw(renderComp.texture,
                                      new Vector2(posComp.x , posComp.y),
                                      renderComp.spriteRectangles[renderComp.currentFrame],
                                      Color.White);

                        //Incrementing the frameIndex on the renderComp to switch to the next frame
                        renderComp.nextFrame(gameTime);
                    }
                    else
                    {
                        _spriteBatch.Draw(renderComp.texture, renderComp.rectangle, Color.AntiqueWhite);
                    }
                }
            }
            _spriteBatch.End();
        }


    }
}
