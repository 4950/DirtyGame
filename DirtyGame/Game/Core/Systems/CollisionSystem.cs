using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Systems;
using EntityFramework;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Components;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Core.Systems
{
    public class CollisionSystem : EntitySystem
    {
        List<Entity> playerEntities; 
        public CollisionSystem() : base(SystemDescriptions.CollisionSystem.Aspect, SystemDescriptions.CollisionSystem.Priority)
        {
            playerEntities = new List<Entity>();
        }

        public override void OnEntityAdded(EntityFramework.Entity e)
        {
            if (e.HasComponent<Player>())
            {
                playerEntities.Add(e);
            }
        }

        public override void OnEntityRemoved(EntityFramework.Entity e)
        {
            if (e.HasComponent<Player>() && playerEntities.Contains(e))
            {
                playerEntities.Remove(e);                
            }
        }

        public override void ProcessEntities(IEnumerable<EntityFramework.Entity> entities, float dt)
        {
            List<Entity> entitiesToRemove = new List<Entity>();
            foreach (Entity player in playerEntities)
            {
                Spatial playerSpatial = player.GetComponent<Spatial>();
                Rectangle bbox = new Rectangle((int)playerSpatial.Position.X + (int)playerSpatial.Offset.X, 
                                               (int)playerSpatial.Position.Y + (int)playerSpatial.Offset.Y,
                                               (int)playerSpatial.BoundaryBox.X, 
                                               (int) playerSpatial.BoundaryBox.Y);    //Needs work

                foreach (Entity e in entities)
                {
                    if (e.Id == player.Id) continue;

                    Spatial s = e.GetComponent<Spatial>();
                    Rectangle bbox2 = new Rectangle((int)s.Position.X, (int)s.Position.Y, 20, 20);

                    if (bbox.Intersects(bbox2))
                    {
                        entitiesToRemove.Add(e);
                    }
                }
            }

            foreach (Entity e in entitiesToRemove)
            {
                World.RemoveEntity(e);
            }
        }
    }
}
