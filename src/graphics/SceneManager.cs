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

        public void Add(Entity entity)
        {
            _entities.Add(entity);
        }
        
        public void Render(GameTime gameTime)
        {
            _renderer.RenderBatch(_entities, camera, gameTime);
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
            IComponent comp = entity.GetComponent<PositionComponent>();
            if (comp != null) 
            {
                PositionComponent pos = (PositionComponent)comp;
                pos.incrementX(xDist);
                pos.incrementY(yDist);
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
                   Vector2 n = camera.Position;
                   n.X += x;
                   n.Y += y;
                   camera.Position = n;
                   //UpdateRenderComps();
               }
               
            }
               
        }

        //Changes the location of the render componenet rectangles based on where the camera is currently at.
        /*public void UpdateRenderComps()
        {
            foreach (Entity e in _entities)
            {
                RenderComponent renderComp = (RenderComponent)e.GetComponent<RenderComponent>();
                PositionComponent posComp = (PositionComponent)e.GetComponent<PositionComponent>();
                InputComponent inputComp = (InputComponent)e.GetComponent<InputComponent>();
                if (renderComp != null && posComp != null && inputComp == null) //okay this may need work
                {
                    renderComp.rectangle.X = posComp.x - _camera.x;
                    renderComp.rectangle.Y = posComp.y - _camera.y;
                }
            }
        }*/

        public void CenterOnPlayer()
        {
            /*
            foreach (Entity e in _entities)
            {
                if (e.HasComponent<InputComponent>())
                {
                    PositionComponent posComp = (PositionComponent)e.GetComponent<PositionComponent>();
                    if (posComp != null)
                    {
                        camera.Position = new Vector2(posComp.x, posComp.y);
                    }
                }

            }*/
        }

        
    }
}
