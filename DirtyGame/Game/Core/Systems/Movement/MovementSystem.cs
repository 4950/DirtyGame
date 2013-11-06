using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Movement;
using DirtyGame.game.Core.Components.Render;
using DirtyGame.game.Core.Systems.Monster;
using DirtyGame.game.Core.Systems.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityFramework.Systems
{
    class MovementSystem : EntitySystem
    {
        public TimeSpan totalTime = new TimeSpan(0, 0, 0, 0, 0);
        private AISystem aiSystem;

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

            Random r = new Random();
            TimeSpan t = new TimeSpan(0, 0, 0, 0, 500);

            foreach (Entity e in entities)
            {
                //Check for entities with an AI movement component
                if (e.HasComponent<MonsterComponent>())
                {
                    //Console.WriteLine("found one");
                    if (e.GetComponent<TimeComponent>().timeOfLastDraw + t <= totalTime)
                    {
                        e.GetComponent<TimeComponent>().timeOfLastDraw = totalTime;
                        double[] moveVector = aiSystem.calculateMoveVector(entities, e);
                        float f = (float) (moveVector[0] * 10.0);
                        e.GetComponent<Spatial>().MoveTo( e.GetComponent<Spatial>().Position.X + (float)(moveVector[0] * 10.0), e.GetComponent<Spatial>().Position.Y + (float)(moveVector[1] * 10.0));
         
                    }
                }

                //Can check for different types of entities down here!
            }
        }

        public MovementSystem()
            : base(SystemDescriptions.MovementSystem.Aspect, SystemDescriptions.MovementSystem.Priority)
        {

        }

        public MovementSystem(AISystem aiSystem)
            : base(SystemDescriptions.MovementSystem.Aspect, SystemDescriptions.MovementSystem.Priority)
        {
            this.aiSystem = aiSystem;
        }
    }
}

