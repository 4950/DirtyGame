using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Render;
using DirtyGame.game.Core.Systems.Monster;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Events;
using Microsoft.Xna.Framework;
using EntityFramework.Systems;
using EntityFramework;

namespace DirtyGame.game.Core.Systems
{
    class GameLogicSystem : EntitySystem
    {

        private int monstersdefeated;

        public override void OnEntityAdded(Entity e)
        {
            monstersdefeated++;
        }

        public override void OnEntityRemoved(Entity e)
        {
            if (--monstersdefeated == 0)
            {
                Event gamestate = new Event();
                gamestate.name = "GameStateGameOver";
                EventManager.Instance.TriggerEvent(gamestate);
            }
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            //Does Nothing
        }

        public override void Initialize()
        {
            monstersdefeated = 0;
        }

        public GameLogicSystem()
            : base(SystemDescriptions.GameLogicSystem.Aspect, SystemDescriptions.GameLogicSystem.Priority)
        {
           
        }
    }
}
