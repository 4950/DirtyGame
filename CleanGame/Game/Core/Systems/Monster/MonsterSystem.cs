using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Render;
using DirtyGame.game.Core.Systems.Monster;
using DirtyGame.game.Core.Systems.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//------------------------------------------------------------------------------
//MonsterSystem has been replaced with a generic movement system, MovementSystem
//------------------------------------------------------------------------------

namespace EntityFramework.Systems
{
    class MonsterSystem : EntitySystem
    {
        public TimeSpan total_Time = new TimeSpan(0, 0, 0, 0, 0);
        private AISystem aiSys;
        
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
            total_Time = total_Time + new TimeSpan(0, 0, 0, 0, elapsed);
            
            Random r = new Random();
            int i;
            TimeSpan t = new TimeSpan(0, 0, 0, 0, 500);

            foreach (Entity e in entities)
            {
                if (e.HasComponent<MonsterComponent>())
                {
                    e.GetComponent<SpatialComponent>().isMoving = true;
                    if (e.GetComponent<TimeComponent>().timeOfLastDraw + t <= total_Time)
                    {
                        e.GetComponent<TimeComponent>().timeOfLastDraw = total_Time;
                        double[] moveVector = aiSys.calculateMoveVector(entities, e);
                        float f = (float)(moveVector[0] * 10.0);
                        e.GetComponent<MovementComponent>().Horizontal = (float)moveVector[0];
                        e.GetComponent<MovementComponent>().Vertical = (float)moveVector[1];
                        //e.GetComponent<Spatial>().MoveTo(e.GetComponent<Spatial>().Position.X + (float)(moveVector[0] * 10.0), e.GetComponent<Spatial>().Position.Y + (float)(moveVector[1] * 10.0));
                        


                        DirectionComponent direction = e.GetComponent<DirectionComponent>();
                        if (Math.Abs(moveVector[0]) > Math.Abs(moveVector[1]))
                        {
                            if (moveVector[0] > 0)
                            {
                                direction.Heading = "Right";
                                e.GetComponent<MovementComponent>().Horizontal = 1;
                                AnimationComponent animation = new AnimationComponent();
                                animation.CurrentAnimation = "Walk" + direction.Heading;
                                e.AddComponent(animation);
                                e.Refresh();

                            }
                            else if (moveVector[0] < 0)
                            {
                                direction.Heading = "Left";
                                e.GetComponent<MovementComponent>().Horizontal = -1;
                                AnimationComponent animation = new AnimationComponent();
                                animation.CurrentAnimation = "Walk" + direction.Heading;
                                e.AddComponent(animation);
                                e.Refresh();
                            }
                        }
                        else
                        {
                            if (moveVector[1] > 0)
                            {
                                direction.Heading = "Down";
                                e.GetComponent<MovementComponent>().Vertical = 1;
                                AnimationComponent animation = new AnimationComponent();
                                animation.CurrentAnimation = "Walk" + direction.Heading;
                                e.AddComponent(animation);
                                e.Refresh();
                            }
                            else if (moveVector[1] < 0)
                            {
                                direction.Heading = "Up";
                                e.GetComponent<MovementComponent>().Vertical = -1;
                                AnimationComponent animation = new AnimationComponent();
                                animation.CurrentAnimation = "Walk" + direction.Heading;
                                e.AddComponent(animation);
                                e.Refresh();
                            }
                        }
                    }
                }
            }
        }

        public MonsterSystem()
            : base(SystemDescriptions.MonsterSystem.Aspect, SystemDescriptions.MonsterSystem.Priority)
        {
            
        }

        public MonsterSystem(AISystem aiSystem)
            : base(SystemDescriptions.MonsterSystem.Aspect, SystemDescriptions.MonsterSystem.Priority)
        {
            this.aiSys = aiSystem;
        }
    }
}
