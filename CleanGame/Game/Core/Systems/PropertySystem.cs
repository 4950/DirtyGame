using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using EntityFramework.Systems;
using CleanGame.Game.Core.Systems.Util;
using CleanGame.Game.Core.Components;

namespace CleanGame.Game.Core.Systems
{
    class PropertySystem : EntitySystem
    {
        public PropertySystem()
            : base(SystemDescriptions.PropertySystem.Aspect, SystemDescriptions.PropertySystem.Priority)
        {
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            foreach (Entity e in entities)
            {
                foreach (Component c in e.Components)
                {
                    if (c.GetType().BaseType.Equals(typeof(PropertyComponent)))
                        ((PropertyComponent)c).IsModified = false;
                }
            }
        }

        public override void OnEntityAdded(Entity e)
        {
            // do nothing
        }

        public override void OnEntityRemoved(Entity e)
        {
            // do nothing
        }
    }
}
