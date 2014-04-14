using CleanGame.Game.Core.Components;
using CleanGame.Game.Core.Components.Movement;
using CleanGame.Game.Core.Components.Render;
using CleanGame.Game.Core.Systems.Monster;
using CleanGame.Game.Core.Systems.Util;
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
            TimeSpan t = new TimeSpan(0, 0, 0, 0, 100);

            foreach (Entity e in entities)
            {
                SpatialComponent mySpatial = e.GetComponent<SpatialComponent>();
                StatsComponent s = e.GetComponent<StatsComponent>();
                //Check for entities with an AI movement component
                if (e.HasComponent<MonsterComponent>())
                {
                    //Console.WriteLine("found one");
                    if (e.GetComponent<TimeComponent>().timeOfLastDraw + t <= totalTime)
                    {
                        e.GetComponent<TimeComponent>().timeOfLastDraw = totalTime;
                        Vector2 moveVector = aiSystem.calculateMoveVector(entities, e, dt, totalTime);

                        //float f = (float) (moveVector[0] * 10.0 * (s.MoveSpeed / 100));
                        e.GetComponent<MovementComponent>().Horizontal = moveVector.X;
                        e.GetComponent<MovementComponent>().Vertical = moveVector.Y;

                        //e.GetComponent<SpatialComponent>().MoveTo( e.GetComponent<SpatialComponent>().Position.X + (float)(moveVector[0] * 10.0), e.GetComponent<Spatial>().Position.Y + (float)(moveVector[1] * 10.0));
                        DirectionComponent direction = e.GetComponent<DirectionComponent>();
                        if (Math.Abs(moveVector.X) > Math.Abs(moveVector.Y))
                        {
                            if (moveVector.X > 0)
                            {
                                direction.Heading = "Right";
                                //e.GetComponent<MovementComponent>().Horizontal = 1;
                            }
                            else if (moveVector.X < 0)
                            {
                                direction.Heading = "Left";
                                //e.GetComponent<MovementComponent>().Horizontal = -1;
                            }
                        }
                        else
                        {
                            if (moveVector.Y > 0)
                            {
                                direction.Heading = "Down";
                                //e.GetComponent<MovementComponent>().Vertical = 1;
                            }
                            else if (moveVector.Y < 0)
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

