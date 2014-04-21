using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.Core.Components;
using CleanGame.Game.Core.Components.Render;
using CleanGame.Game.Core.Systems.Util;
using CleanGame.Game.SGraphics;
using CleanGame.Game.SGraphics.Commands;
using CleanGame.Game.SGraphics.Commands.DrawCalls;
using EntityFramework;
using EntityFramework.Systems;
using Microsoft.Xna.Framework;

namespace CleanGame.Game.Core.Systems
{
    class AOESystem : EntitySystem
    {
        private Dirty game;

        public AOESystem(Dirty game)
            : base(SystemDescriptions.AOESystem.Aspect, SystemDescriptions.AOESystem.Priority)
        {
            this.game = game;
        }
        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            foreach (Entity e in entities.ToList())
            {
                AOEComponent ac = e.GetComponent<AOEComponent>();
                if (ac.Ticks > 0 && ac.Owner.entity != null)
                {
                    ac.Timer += dt;
                    if (ac.Timer >= ac.TickInterval)
                    {
                        ac.HitList.Clear();
                        ac.Ticks--;
                        ac.Timer = 0;
                    }
                }
                else
                {
                    game.world.DestroyEntity(e);
                }
            }
        }

        public override void OnEntityAdded(Entity e)
        {
            
        }

        public override void OnEntityRemoved(Entity e)
        {
            
        }
    }
}
