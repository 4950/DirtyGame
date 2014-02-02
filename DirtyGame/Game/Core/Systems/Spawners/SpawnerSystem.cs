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
using DirtyGame;

namespace EntityFramework.Systems
{
    class SpawnerSystem : EntitySystem
    {
        public EntityFactory entityFactory;
        private Dirty game;
        public TimeSpan totalTime = new TimeSpan(0, 0, 0, 0, 0);

        public SpawnerSystem(EntityFactory eF, Dirty game)
            : base(SystemDescriptions.SpawnerSystem.Aspect, SystemDescriptions.SpawnerSystem.Priority)
        {
            this.game = game;
            this.entityFactory = eF;
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            Random r = new Random();

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

                    if (((spawner.timeOfLastSpawn + spawner.timePerSpawn) <= totalTime) && (spawner.numMobs != 0))
                    {
                        //Reset spawner timeOfLstSpawn
                        //int seconds = (int) Math.Floor(dt);
                        //int milliseconds = (int) Math.Floor((dt - seconds) * 1000);
                        TimeSpan t = totalTime;
                        spawner.timeOfLastSpawn = t;

                        spawner.numMobs--;

                        //Create new entity
                        Vector2 pos = new Vector2(e.GetComponent<SpatialComponent>().Position.X + r.Next(-25, 26), (int)e.GetComponent<SpatialComponent>().Position.Y + r.Next(-25, 26));
                        Entity monster = null;
                        // if (spawner.data.weapon != null)
                        //{
                        //monster = entityFactory.CreateBasicMonster(pos, e.GetComponent<SpawnerComponent>().sprite.SpriteSheet.spriteName, e.GetComponent<SpawnerComponent>().sprite.SpriteSheet.xmlFileLocation, spawner.data);
                        monster = entityFactory.CloneEntity(game.world.EntityMgr.GetEntityByName(spawner.MonsterType));

                        InventoryComponent ic = new InventoryComponent();
                        Entity weapon = entityFactory.CloneEntity(game.world.EntityMgr.GetEntityByName(spawner.MonsterWeapon));
                        weapon.Refresh();
                        ic.addWeapon(weapon, monster);
                        monster.AddComponent(ic);

                        monster.GetComponent<SpatialComponent>().Position = pos;
                        // }
                        //else
                        //    monster = entityFactory.CreateBasicMonster(pos, e.GetComponent<SpawnerComponent>().sprite.SpriteSheet, data);

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
