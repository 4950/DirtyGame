using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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

        public void Render()
        {

        }
    }
}
