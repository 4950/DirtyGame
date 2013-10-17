using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ShittyPrototype.src.core;
using ShittyPrototype.src.graphics;
using ShittyPrototype.src.application;
using Microsoft.Xna.Framework.Input;
using ShittyPrototype.src.application.core;
using ShittyPrototype.src.Map;

namespace ShittyPrototype
{
    class SceneManager
    {
        private Renderer _renderer;
        private List<Entity> _entities;
        public Camera camera;

        public SceneManager(GraphicsDeviceManager graphicsDevice)
        {
            _renderer = new Renderer(graphicsDevice);
            _entities = new List<Entity>();
            camera = new Camera();
            camera.Position = new Vector2(0, 0);
            
        }

        public void setCameraBounds(Map map)
        {
            Vector2 n;
            n.X = map.getPixelWidth() - _renderer.GetViewportWidth();
            n.Y = map.getPixelHeight() - _renderer.GetViewportHeight();
            camera.MaximumPosition = n;
        }

        public void Add(Entity entity)
        {
            _entities.Add(entity);
        }
        
        public void Render()
        {
            _renderer.RenderBatch(_entities, camera);
        }


        public void updateRenderComp(Entity entity, PositionComponent pos)
        {
            IComponent comp = entity.GetComponent<RenderComponent>();
            if (comp != null)
            {
                RenderComponent ren = (RenderComponent)comp;
                ren.rectangle.X = pos.x;
                ren.rectangle.Y = pos.y;
            }
        }

        //Generic move for all entities with a position.  Could part of position component...
        public void Move(Entity entity, int xDist, int yDist)
        {
            IComponent posComp = entity.GetComponent<PositionComponent>();
            IComponent renComp = entity.GetComponent<RenderComponent>();
            if (posComp != null) 
            {
                PositionComponent pos = (PositionComponent)posComp;
                pos.x += xDist;
                pos.y += yDist;
            }
            if (renComp != null)
            {
                RenderComponent ren = (RenderComponent)renComp;
                ren.rectangle.X += xDist;
                ren.rectangle.Y += yDist;
            }
        }

        //Moves entities marked with an Input componenet.  It's here until I figure out how the input context shenanigans work, or could possibly be added to the componenet
        public void MovePlayer(int x, int y)
        {
            foreach(Entity e in _entities)
            {
                if (e.HasComponent<InputComponent>())
               {
                   Move(e, x, y);
                   CenterOnPlayer();
               }
               
            }
               
        }

        public void CenterOnPlayer()
        {
            
            foreach (Entity e in _entities)
            {
                if (e.HasComponent<InputComponent>())
                {
                    PositionComponent posComp = (PositionComponent)e.GetComponent<PositionComponent>();
                    if (posComp != null)
                    {
                        camera.Position = new Vector2(posComp.x-(_renderer.GetViewportWidth()/2), posComp.y-(_renderer.GetViewportHeight()/2));
                    }
                }

            }
            if (camera.Position.X < camera.MinimumPosition.X)
            {
                Vector2 n = camera.Position;
                n.X = camera.MinimumPosition.X;
                camera.Position = n;
            }
            if (camera.Position.Y < camera.MinimumPosition.Y)
            {
                Vector2 n = camera.Position;
                n.Y = camera.MinimumPosition.Y;
                camera.Position = n;
            }
            if (camera.Position.X > camera.MaximumPosition.X)
            {
                Vector2 n = camera.Position;
                n.X = camera.MaximumPosition.X;
                camera.Position = n;
            }
            if (camera.Position.Y > camera.MaximumPosition.Y)
            {
                Vector2 n = camera.Position;
                n.Y = camera.MaximumPosition.Y;
                camera.Position = n;
            }
             

        }

        
    }
}
