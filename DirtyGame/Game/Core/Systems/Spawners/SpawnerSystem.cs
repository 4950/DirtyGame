using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Systems;
using DirtyGame.game.Core.Components;
using EntityFramework.Managers;
using DirtyGame.game.Core.Components.Render;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core;

namespace EntityFramework.Systems
{
    class SpawnerSystem : EntitySystem
    {
        public EntityFactory entityFactory;
        public TimeSpan totalTime = new TimeSpan(0,0,0,0,0);

        public SpawnerSystem(EntityFactory eF)
            : base(SystemDescriptions.SpawnerSystem.Aspect, SystemDescriptions.SpawnerSystem.Priority)
        {
            this.entityFactory = eF;
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            Random r  = new Random();

            //Keep track of total gameTime
            int elapsed = (int)Math.Floor(dt * 1000);
            totalTime = totalTime + new TimeSpan(0, 0, 0, 0, elapsed);
            foreach (Entity e in entities)
            {
                if (e.HasComponent<SpawnerComponent>())
                {
                    SpawnerComponent spawner = e.GetComponent<SpawnerComponent>();

                    

                  //  Console.Write(dt);
                  //  Console.WriteLine(totalTime);

                    if (((spawner.timeOfLastSpawn + spawner.timePerSpawn) <= totalTime)  && (spawner.numMobs!=0))
                    {
                        //Reset spawner timeOfLstSpawn
                        //int seconds = (int) Math.Floor(dt);
                        //int milliseconds = (int) Math.Floor((dt - seconds) * 1000);
                        TimeSpan t = totalTime;
                        spawner.timeOfLastSpawn = t;

                        spawner.numMobs--;

                        //Create new entity
                        Entity monster = entityFactory.CreateMonster(   e.GetComponent<SpawnerComponent>().sprite.SpriteSheet.SpriteSheetTexture.Name, 
                                                                        (int) e.GetComponent<Spatial>().Position.X + r.Next(-75, 76), 
                                                                        (int) e.GetComponent<Spatial>().Position.Y + r.Next(-75, 76), 
                                                                        e.GetComponent<SpawnerComponent>().sprite);
                        monster.Refresh();
                    }
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
