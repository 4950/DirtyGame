using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EntityFramework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;

namespace DirtyGame.game.Core
{
    public class Physics
    {
        private Dictionary<int, Entity> entityDictionary;
        private Dictionary<uint, Body> bodyDictionary;
        private FarseerPhysics.Dynamics.World physicsWorld;

        public Physics()
        {
            physicsWorld = new FarseerPhysics.Dynamics.World(new Vector2(0, 0));
            entityDictionary = new Dictionary<int, Entity>();
            bodyDictionary = new Dictionary<uint, Body>();
        }

        public FarseerPhysics.Dynamics.World World
        {
            get
            {
                return physicsWorld;
            }
        }

        public Dictionary<int, Entity> Entity
        {
            get
            {
                return entityDictionary;
            }
        }

        public Dictionary<uint, Body> Body
        {
            get
            {
                return bodyDictionary;
            }
        }

        public void AddEntity(int key, Entity e)
        {
            entityDictionary.Add(key, e);
        }

        public void RemoveEntity(int key)
        {
            if (entityDictionary.ContainsKey(key))
            {
                entityDictionary.Remove(key);
            }
        }

        public void AddBody(uint key, Body Body)
        {
            bodyDictionary.Add(key, Body);
        }

        public void RemoveBody(uint key)
        {
            if (bodyDictionary.ContainsKey(key))
            {
                bodyDictionary.Remove(key);
            }
        }

        public void Update(GameTime gameTime)
        {
            physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
        }

        public List<Entity> Query(Vector2 center, float width, float height)
        {
            List<int> Found = new List<int>();
            List<Entity> entity = new List<Entity>();
            List<Fixture> fixtures = new List<Fixture>();

            FarseerPhysics.Collision.AABB Box = new FarseerPhysics.Collision.AABB(
                ConvertUnits.ToSimUnits(center),
                ConvertUnits.ToSimUnits(width),
                ConvertUnits.ToSimUnits(height));

            fixtures = physicsWorld.QueryAABB(ref Box);
            
            foreach (Fixture f in fixtures)
            {

                if (!Found.Contains(f.Body.BodyId))
                {
                    Found.Add(f.Body.BodyId);
                    entity.Add(entityDictionary[f.Body.BodyId]);
                }
            }


            return entity;
        }

    }
}
