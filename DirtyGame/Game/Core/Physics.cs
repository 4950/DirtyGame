using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EntityFramework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;
using DirtyGame.game.Core.Systems;

namespace DirtyGame.game.Core
{
    public class Physics
    {
        
        private FarseerPhysics.Dynamics.World physicsWorld;
        private EntityFramework.Managers.EntityManager entityManager;
        private Dictionary<int, uint> BodyIdToEntityId;

        public Physics(EntityFramework.Managers.EntityManager entityManager)
        {
            physicsWorld = new FarseerPhysics.Dynamics.World(new Vector2(0, 0));
            this.entityManager = entityManager;
            BodyIdToEntityId = new Dictionary<int, uint>();
        }

        public FarseerPhysics.Dynamics.World World
        {
            get
            {
                return physicsWorld;
            }
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
                        entity.Add(entityManager.GetEntity(BodyIdToEntityId[f.Body.BodyId]));
                    }
                }


                return entity;
            
        }

        

        public void Update(GameTime gameTime)
        {
            physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
        }


        public void AddEntityId(int key, uint entityId)
        {
            BodyIdToEntityId.Add(key, entityId);
        }

        public int RemoveEntityId(int key)
        {
            if (BodyIdToEntityId.ContainsKey(key))
            {
                BodyIdToEntityId.Remove(key);
                return 0;
            }

            else
            {
                return 1; //For error Usage, Most likely to never occur
            }
        }
    }
}
