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
                SpatialComponent mySpatial = e.GetComponent<SpatialComponent>();
                //Check for entities with an AI movement component
                if (e.HasComponent<MonsterComponent>())
                {
                    //Console.WriteLine("found one");
                    if (e.GetComponent<TimeComponent>().timeOfLastDraw + t <= totalTime)
                    {
                        e.GetComponent<TimeComponent>().timeOfLastDraw = totalTime;
                        double[] moveVector = aiSystem.calculateMoveVector(entities, e);
                        float f = (float) (moveVector[0] * 10.0);
                        e.GetComponent<MovementComponent>().Horizontal = (float)moveVector[0];
                        e.GetComponent<MovementComponent>().Vertical = (float)moveVector[1];

                        //e.GetComponent<SpatialComponent>().MoveTo( e.GetComponent<SpatialComponent>().Position.X + (float)(moveVector[0] * 10.0), e.GetComponent<Spatial>().Position.Y + (float)(moveVector[1] * 10.0));
                        DirectionComponent direction = e.GetComponent<DirectionComponent>();
                        if (Math.Abs(moveVector[0]) > Math.Abs(moveVector[1]))
                        {
                            if (moveVector[0] > 0)
                            {
                                direction.Heading = "Right";
                                //e.GetComponent<MovementComponent>().Horizontal = 1;
                            }
                            else if (moveVector[0] < 0)
                            {
                                direction.Heading = "Left";
                                //e.GetComponent<MovementComponent>().Horizontal = -1;
                            }
                        }
                        else
                        {
                            if (moveVector[1] > 0)
                            {
                                direction.Heading = "Down";
                                //e.GetComponent<MovementComponent>().Vertical = 1;
                            }
                            else if (moveVector[1] < 0)
                            {
                                direction.Heading = "Up";
                                //e.GetComponent<MovementComponent>().Vertical = -1;
                            }
                        }
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

