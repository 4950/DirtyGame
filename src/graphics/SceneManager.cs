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
        GraphicsDeviceManager gD;

        public SceneManager(GraphicsDeviceManager graphicsDevice)
        {
            gD = graphicsDevice;
            _renderer = new Renderer(graphicsDevice);
            _entities = new List<Entity>();
            camera = new Camera();
            camera.Position = new Vector2(0, 0);
            
        }

        public List<Entity> getEntities()
        {
            return _entities;
        }

        public void Add(Entity entity)
        {
            _entities.Add(entity);
        }

        public void Remove(Entity entity)
        {
            _entities.Remove(entity);
        }
        
        public void Render()
        {
            _renderer.RenderBatch(_entities, camera);
        }

        public void DetectCollision(List<Monster> monsters, MonsterManager monsterManager)
        {
            Entity p = new Entity();
            foreach (Entity entity in this.getEntities())
            {
                if (entity.HasComponent<InputComponent>())
                {
                    p = entity;
                }
            }

            PositionComponent playerPosition = (PositionComponent)p.GetComponent<PositionComponent>();
            int playerX = playerPosition.x;
            int playerY = playerPosition.y;

            //Console.WriteLine("player:  " + playerX.ToString() + "  " + playerY.ToString());

            foreach (Monster m in monsters)
            {
                Console.WriteLine("player:  " + playerX.ToString() + "  " + playerY.ToString() + "  monster:  " + m.pos.x.ToString() + "  " + m.pos.y.ToString());
                if ((Math.Abs(playerX - m.pos.x) < 80) && ((Math.Abs(playerY - m.pos.y) < 80)))
                {
                    Console.WriteLine("Collision Detected.  Monster location: " + m.pos.x.ToString() + "," + m.pos.y.ToString());
                    RenderComponent rc = (RenderComponent)m.GetComponent<RenderComponent>();
                    rc.texture = new Texture2D(gD.GraphicsDevice, 1, 1);
                    rc.texture.SetData(new Color[] { Color.AliceBlue }); this.Remove(m);
                    monsterManager.Remove(m);
                }
            }

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
