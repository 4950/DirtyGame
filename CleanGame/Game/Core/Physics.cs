using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EntityFramework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;
using CleanGame.Game.Core.Systems;

namespace CleanGame.Game.Core
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
            
                List<Entity> entity = new List<Entity>();
                List<Fixture> fixtures = new List<Fixture>();

                FarseerPhysics.Collision.AABB Box = new FarseerPhysics.Collision.AABB(
                    ConvertUnits.ToSimUnits(center),
                    ConvertUnits.ToSimUnits(width),
                    ConvertUnits.ToSimUnits(height));

                fixtures = physicsWorld.QueryAABB(ref Box);

                foreach (Fixture f in fixtures)
                {
                    if(BodyIdToEntityId.ContainsKey(f.Body.BodyId))
                    {
                        if (!entity.Contains(entityManager.GetEntity(BodyIdToEntityId[f.Body.BodyId])))
                        {
                            entity.Add(entityManager.GetEntity(BodyIdToEntityId[f.Body.BodyId]));
                        }
                    }
                }


                return entity;
            
        }

        public List<Entity> RayCast(Vector2 point1, Vector2 point2)
        {
            List<Entity> entity = new List<Entity>();
            List<Fixture> fixtures = new List<Fixture>();

            fixtures = physicsWorld.RayCast(ConvertUnits.ToSimUnits(point1), ConvertUnits.ToSimUnits(point2));

            foreach (Fixture f in fixtures)
            {
                if (BodyIdToEntityId.ContainsKey(f.Body.BodyId))
                {
                    if (!entity.Contains(entityManager.GetEntity(BodyIdToEntityId[f.Body.BodyId])))
                    {
                        entity.Add(entityManager.GetEntity(BodyIdToEntityId[f.Body.BodyId]));
                    }
                }
            }

            return entity;
        }

        public void Update(float dT)
        {
            physicsWorld.Step(dT);
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
