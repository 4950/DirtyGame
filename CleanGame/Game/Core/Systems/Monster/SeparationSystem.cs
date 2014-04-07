using CleanGame.Game.Core.Components;
using CleanGame.Game.Core.Systems.Util;
using EntityFramework;
using EntityFramework.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanGame.Game.Core.Systems.Movement
{
    class SeparationSystem : EntitySystem   
    {
        public TimeSpan totalTime = new TimeSpan(0, 0, 0, 0, 0);

        public override void OnEntityAdded(Entity e)
        {
            // do nothing
        }

        public override void OnEntityRemoved(Entity e)
        {
            // do nothing
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            int elapsed = (int)Math.Floor(dt * 1000);
            totalTime = totalTime + new TimeSpan(0, 0, 0, 0, elapsed);

            TimeSpan t = new TimeSpan(0, 0, 0, 0, 500);

            foreach (Entity e in entities)
            {
                SpatialComponent mySpatial = e.GetComponent<SpatialComponent>();
                foreach (Entity e2 in entities)
                {
                    if (e2 == e)
                    {
                        continue;
                    }
                    SpatialComponent otherMonster = e2.GetComponent<SpatialComponent>();
                    if (Vector2.Distance(mySpatial.Center, otherMonster.Center) < 50)
                    {

                        e.GetComponent<MovementComponent>().Velocity += 0.5f * Vector2.Normalize(mySpatial.Center - otherMonster.Center);
                        e.GetComponent<MovementComponent>().Velocity = Vector2.Min(e.GetComponent<MovementComponent>().Velocity, new Vector2(1, 1));
                        e.GetComponent<MovementComponent>().Velocity = Vector2.Max(e.GetComponent<MovementComponent>().Velocity, new Vector2(-1, -1));
                    }
                }

                

                //Can check for different types of entities down here!
            }
        }

        public SeparationSystem()
            : base(SystemDescriptions.SeparationSystem.Aspect, SystemDescriptions.SeparationSystem.Priority)
        {

        }
    }
}
