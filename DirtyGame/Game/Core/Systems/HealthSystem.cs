using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using EntityFramework.Systems;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Components;

namespace DirtyGame.game.Core.Systems
{
    class HealthSystem : EntitySystem
    {

        public HealthSystem()
            : base(SystemDescriptions.HealthSystem.Aspect, SystemDescriptions.HealthSystem.Priority)
        {

        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {

            List<Entity> ToRemove = new List<Entity>();

            foreach (Entity e in entities)
            {
                HealthComponent health = e.GetComponent<HealthComponent>();
                if (health.CurrentHealth == 0)
                {
                    ToRemove.Add(e);
                }
            }


            foreach (Entity e in ToRemove)
            {
                World.RemoveEntity(e);
            }

            
        }

        public override void OnEntityAdded(Entity e)
        {
            PhysicsComponent physics = e.GetComponent<PhysicsComponent>();

            physics.AddCollisionCallback(HealthCollisionCallback);
        }

        public override void OnEntityRemoved(Entity e)
        {
            PhysicsComponent physics = e.GetComponent<PhysicsComponent>();

            physics.RemoveCollisionCallback(HealthCollisionCallback);
        }

        public void HealthCollisionCallback(Entity entityA, Entity entityB)
        {
            if (entityA.HasComponent<PlayerComponent>() && entityB.HasComponent<MonsterComponent>())
            {
                entityB.GetComponent<HealthComponent>().LoseHealth(1);
            }
        }
    }
}
