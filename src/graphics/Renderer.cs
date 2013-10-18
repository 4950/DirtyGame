using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ShittyPrototype.src.core;
using ShittyPrototype.src.graphics;
using ShittyPrototype.src.application.core;
using ShittyPrototype.src.application;
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

        public void RenderBatch(ICollection<Entity> entities, Camera cam)
        {
            _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, cam.TransformMatrix);
            foreach (Entity entity in entities)
            {
                RenderComponent renderComp = (RenderComponent)entity.GetComponent<RenderComponent>();
                PositionComponent positionComp = (PositionComponent)entity.GetComponent<PositionComponent>();
                if (renderComp != null && positionComp != null)
                {
                    //Rectangle r = renderComp.rectangle;
                    //r.X += entity.GetHashCode() % 10;
                    //Debug.WriteLine(entity.GetHashCode() + " " + renderComp.rectangle.ToString());
                   Rectangle rect = renderComp.rectangle;
                   rect.Offset(positionComp.x, positionComp.y);
                    _spriteBatch.Draw(renderComp.texture, rect, Color.AntiqueWhite);
                }
            }
            _spriteBatch.End();
        }


    }
}
