using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Movement;
using DirtyGame.game.Core.Systems.Util;
using EntityFramework;
using EntityFramework.Systems;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Core.Systems.Movement
{
    class SeekMovementSystem : EntitySystem
    {
        public SeekMovementSystem()
            : base(SystemDescriptions.SeekMovementSystem.Aspect, SystemDescriptions.SeekMovementSystem.Priority)
        {
            
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            foreach (Entity e in entities)
            {
                SeekMover mover = e.GetComponent<SeekMover>();
                Spatial spatial = e.GetComponent<Spatial>();
                if (!mover.Target.HasComponent<Spatial>()) continue;
                Spatial targetSpatial = mover.Target.GetComponent<Spatial>();
                
                float distance = Vector2.Distance(targetSpatial.Position, spatial.Position);
                if (distance < mover.SlowingDistance)
                {
                    mover.DesiredVelocity = Vector2.Normalize(targetSpatial.Position - spatial.Position) * mover.MaxVelocity * (distance / mover.SlowingDistance);
                }
                else
                {
                    mover.DesiredVelocity = Vector2.Normalize(targetSpatial.Position - spatial.Position) * mover.MaxVelocity;    
                }
                
                mover.SteerForce = mover.DesiredVelocity - mover.Velocity;
                mover.SteerForce = Vector2.Min(mover.SteerForce, mover.MaxForce);
                mover.Velocity = Vector2.Min(mover.Velocity + mover.SteerForce, mover.MaxVelocity);
                spatial.Position += mover.Velocity*dt;
            }
        }

        public override void OnEntityAdded(EntityFramework.Entity e)
        {            
        }

        public override void OnEntityRemoved(EntityFramework.Entity e)
        {            
        }
    }
}
