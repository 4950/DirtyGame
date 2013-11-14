using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using EntityFramework.Systems;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Events;

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

            physics.AddCollisionCallback("Health", HealthCollisionCallback);
        }

        public override void OnEntityRemoved(Entity e)
        {
            PhysicsComponent physics = e.GetComponent<PhysicsComponent>();

            physics.RemoveCollisionCallback("Health", HealthCollisionCallback);
        }

        public void HealthCollisionCallback(Event e)
        {
           CollisionEvent collision = (CollisionEvent) e;

            if (collision.entityA.HasComponent<PlayerComponent>() && collision.entityB.HasComponent<MonsterComponent>())
            {
                collision.entityB.GetComponent<HealthComponent>().LoseHealth(1);
            }
        }
    }
}
