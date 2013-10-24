using DirtyGame.game.Core.Components;
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
    class MonsterSystem : EntitySystem
    {
        public TimeSpan totalTime = new TimeSpan(0, 0, 0, 0, 0);
        private AISystem aiSystem;
        
        //public void Update(GameTime gameTime)
        //{
        //    Random r = new Random();
        //    int i;
        //    TimeSpan t = new TimeSpan(0, 0, 0, 0, 500);
        //    List<Monster> currentState = new List<Monster>();
        //    foreach (Monster m in monsters)
        //    {
        //        currentState.Add(m);
        //    }
        //    foreach (Monster m in monsters)
        //    {
        //        if (m.render.timeOfLastDraw + t <= gameTime.TotalGameTime)
        //        {
        //            m.render.timeOfLastDraw = gameTime.TotalGameTime;
        //            double[] moveVector = ai.calculateMoveVector(currentState, m);
        //            m.render.rectangle.X += (int)(moveVector[0] * 10.0);
        //            m.pos.x += (int)(moveVector[0] * 10.0);
        //            m.render.rectangle.Y += (int)(moveVector[1] * 10.0);
        //            m.pos.y += (int)(moveVector[1] * 10.0);
        //        }
        //    }
        //}
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
            int i;
            TimeSpan t = new TimeSpan(0, 0, 0, 0, 500);

            foreach (Entity e in entities)
            {
                if (e.HasComponent<MonsterComponent>())
                {
                    if (e.GetComponent<TimeComponent>().timeOfLastDraw + t <= totalTime)
                    {
                        e.GetComponent<TimeComponent>().timeOfLastDraw = totalTime;
                        double[] moveVector = aiSystem.calculateMoveVector(entities, e);
                        float f = (float) (moveVector[0] * 10.0);
                        e.GetComponent<Spatial>().MoveTo( e.GetComponent<Spatial>().Position.X + (float)(moveVector[0] * 10.0), e.GetComponent<Spatial>().Position.Y + (float)(moveVector[1] * 10.0));
         
                    }
                }
            }

            //foreach (Monster m in monsters)
            //{
            //    currentState.Add(m);
            //}
            //foreach (Monster m in monsters)
            //{
            //    if (m.render.timeOfLastDraw + t <= gameTime.TotalGameTime)
            //    {
            //        m.render.timeOfLastDraw = gameTime.TotalGameTime;
            //        double[] moveVector = aiS.calculateMoveVector(entities, m);
            //        m.render.rectangle.X += (int)(moveVector[0] * 10.0);
            //        m.pos.x += (int)(moveVector[0] * 10.0);
            //        m.render.rectangle.Y += (int)(moveVector[1] * 10.0);
            //        m.pos.y += (int)(moveVector[1] * 10.0);
            //    }
            //}
        }

        public MonsterSystem()
            : base(SystemDescriptions.MonsterSystem.Aspect, SystemDescriptions.MonsterSystem.Priority)
        {
            
        }

        public MonsterSystem(AISystem aiSystem)
            : base(SystemDescriptions.MonsterSystem.Aspect, SystemDescriptions.MonsterSystem.Priority)
        {
            this.aiSystem = aiSystem;
        }
    }
}
