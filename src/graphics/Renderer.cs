using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ShittyPrototype.src.core;
using ShittyPrototype.src.graphics;

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

        public void RenderBatch(ICollection<Entity> entities)
        {
            _spriteBatch.Begin();
            foreach (Entity entity in entities)
            {
                RenderComponent renderComp = (RenderComponent) entity.GetComponent<RenderComponent>();
                if (renderComp != null)
                {
                    _spriteBatch.Draw(renderComp.texture, renderComp.rectangle, Color.AntiqueWhite);
                }
            }
            _spriteBatch.End();
        }
    }
}
