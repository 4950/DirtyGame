using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Components;
using EntityFramework;
using EntityFramework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DirtyGame.game.SGraphics;

namespace DirtyGame.game.Core.Systems
{
    class WeaponSystem : EntitySystem
    {
        public WeaponSystem()
            : base(SystemDescriptions.WeaponSystem.Aspect, SystemDescriptions.WeaponSystem.Priority)
        {
        }
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
            foreach (Entity e in entities)
            {
                if (e.HasComponent<WeaponComponent>())
                {
                    WeaponComponent wc = e.GetComponent<WeaponComponent>();

                    if (wc.LastFire > 0)
                        wc.LastFire -= dt;
                }
            }
        }
    }
}
