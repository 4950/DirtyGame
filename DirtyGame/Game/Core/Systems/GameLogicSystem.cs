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
    public class GameLogicSystem : EntitySystem
    {

        public int monstersdefeated;
        public int monstersalive;


        public override void OnEntityAdded(Entity e)
        {
            if (e.HasComponent<MonsterComponent>())
            {
                monstersalive++;
            }
        }

        public override void OnEntityRemoved(Entity e)
        {
            if (e.HasComponent<MonsterComponent>())
            {
                monstersdefeated++;
                if (--monstersalive == 0)
                {
                    Event gamestate = new Event();
                    gamestate.name = "GameStateGameOver";
                    EventManager.Instance.TriggerEvent(gamestate);
                }
            }
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            for (int i = 0; i < entities.Count(); i++)
            {
                Entity e = entities.ElementAt(i);

                HealthComponent hc = e.GetComponent<HealthComponent>();
                if (hc.CurrentHealth <= 0)//dead
                {
                    World.RemoveEntity(e);
                    i--;
                }

            }
        }

        public override void Initialize()
        {
            monstersdefeated = 0;
        }

        public GameLogicSystem() : base(SystemDescriptions.GameLogicSystem.Aspect, SystemDescriptions.GameLogicSystem.Priority)
        {
           
        }
    }
}
