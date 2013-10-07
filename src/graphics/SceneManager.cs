using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ShittyPrototype.src.core;
using ShittyPrototype.src.graphics;
using ShittyPrototype.src.application.core;

namespace ShittyPrototype
{
    class SceneManager
    {
        private Renderer _renderer;
        private List<Entity> _entities;

        public SceneManager(GraphicsDeviceManager graphicsDevice)
        {
            _renderer = new Renderer(graphicsDevice);
            _entities = new List<Entity>();
        }

        public void Add(Entity entity)
        {
            _entities.Add(entity);
        }
        
        public void Render()
        {
            _renderer.RenderBatch(_entities);
        }

        
    }
}
