using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Systems;
using EntityFramework;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Components;
using Microsoft.Xna.Framework;
using CoreUI;
using DirtyGame.game.SGraphics;
using CoreUI.Elements;

namespace DirtyGame.game.Core.Systems
{
    class ProjectileSystem : EntitySystem
    {
        private Dirty game;
        public ProjectileSystem(Dirty game)
            : base(SystemDescriptions.ProjectileSystem.Aspect, SystemDescriptions.ProjectileSystem.Priority)
        {
            this.game = game;
        }
        public override void OnEntityAdded(EntityFramework.Entity e)
        {
        }

        public override void OnEntityRemoved(EntityFramework.Entity e)
        {
        }
        public override void ProcessEntities(IEnumerable<EntityFramework.Entity> entities, float dt)
        {
            for (int i = 0; i < entities.Count(); i++)
            {
                Entity e = entities.ElementAt(i);
                if (e.HasComponent<ProjectileComponent>())
                {
                    ProjectileComponent pc = e.GetComponent<ProjectileComponent>();

                    if (Vector2.Distance(pc.origin, e.GetComponent<SpatialComponent>().Position) > pc.range)
                    {
                        game.world.RemoveEntity(e);
                    }
                }
            }
        }
    }
}
