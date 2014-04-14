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

            Vector2 positionDif = new Vector2();

            Random rand = new Random();

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
                    if (Vector2.Distance(mySpatial.Position, otherMonster.Position) <= 32)
                    {
                        positionDif += (mySpatial.Position - otherMonster.Position);
                    }
                }

                // since these guys aren't birds, we dont want true flocking. So, add a bit of random to throw it off a bit.
                // actually I think it looks better with true flocking. But I'll leave this here just in case.
                e.GetComponent<MovementComponent>().Velocity += dt * (positionDif); //+ new Vector2(rand.Next(-20, 20), rand.Next(-20, 20)));

            }



        }

        public SeparationSystem()
            : base(SystemDescriptions.SeparationSystem.Aspect, SystemDescriptions.SeparationSystem.Priority)
        {

        }
    }
}
