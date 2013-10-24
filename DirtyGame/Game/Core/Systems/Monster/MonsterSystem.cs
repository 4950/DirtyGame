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
        { }

        public MonsterSystem()
            : base(SystemDescriptions.MonsterSystem.Aspect, SystemDescriptions.MonsterSystem.Priority)
        {
            
        }
    }
}
