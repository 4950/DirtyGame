using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using EntityFramework.Systems;
using FarseerPhysics.Factories;
using CleanGame.Game.Core.Components.Movement;
using EntityFramework;
using CleanGame.Game.Core.Systems.Util;
using CleanGame.Game.Core.Components;
using Microsoft.Xna.Framework;
using CleanGame.Game.SGraphics;
using CleanGame.Game.SGraphics.Commands;
using CleanGame.Game.SGraphics.Commands.DrawCalls;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Collision;
using CleanGame.Game.Util;
using CleanGame.Game.Core.Components.Render;

namespace CleanGame.Game.Core.Systems
{
    class MeleeSystem : EntitySystem
    {
        private Dirty game;
        private float totalTime;

        public List<Entity> oldLandmines = new List<Entity>();
        public MeleeSystem(Dirty game)
            : base(SystemDescriptions.MeleeSystem.Aspect, SystemDescriptions.MeleeSystem.Priority)
        {
            this.game = game;
        }
        public override void OnEntityAdded(Entity e) 
        {
            e.GetComponent<TimeComponent>().timeDrawn = totalTime;
        }

        public override void OnEntityRemoved(Entity e)
        {
        }
        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            totalTime += (float)Math.Floor(dt * 1000);
            List<Entity> ToDestroy = new List<Entity>();

            for (int i = 0; i < entities.Count(); i++)
            {
                Entity e = entities.ElementAt(i);
                TimeComponent time = e.GetComponent<TimeComponent>();
                MeleeComponent melee = e.GetComponent<MeleeComponent>();

                if (totalTime - time.timeDrawn > (1000 * melee.TimePresent))
                {
                    game.world.DestroyEntity(e);
                }
                
            }



        }
    }
}
