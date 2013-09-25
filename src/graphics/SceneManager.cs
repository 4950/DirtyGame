using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShittyPrototype
{
    class SceneManager
    {
        private Renderer _renderer;

        public SceneManager(GraphicsDeviceManager graphicsDevice)
        {
            _renderer = new Renderer(graphicsDevice);
        }

        public void Add()
        {

        }
        public void Render()
        {
            _renderer.Render();
        }
    }
}
