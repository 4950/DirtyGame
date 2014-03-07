using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Systems;
using CleanGame.Game.Core.Components;
using EntityFramework.Managers;
using CleanGame.Game.Core.Components.Render;
using CleanGame.Game.Core.Systems.Util;
using CleanGame.Game.Core;
using CleanGame;
using CleanGame.Game.Util;
using GameService;

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
                        GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.MonsterSpawned, spawner.MonsterType);

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

                        
                        
                        if (spawner.HealthUpModifier != 0)
                        {
                           StatsComponent stats = monster.GetComponent<StatsComponent>();

                           stats.BaseHealth = (int)Math.Floor(stats.BaseHealth * spawner.HealthUpModifier);
                           stats.CurrentHealth = (int)Math.Floor(stats.CurrentHealth * spawner.HealthUpModifier);
                        }

                        if (spawner.DamageUpModifier != 0)
                        {
                           WeaponComponent wc = weapon.GetComponent<WeaponComponent>();

                           wc.BaseDamage = (int)Math.Floor(wc.BaseDamage * spawner.DamageUpModifier);
                        }    

                        

                        ic.addWeapon(weapon, monster);
                        monster.AddComponent(ic);

                        SpatialComponent monsterSpatial = monster.GetComponent<SpatialComponent>();
                        monsterSpatial.Position = pos;

                        if (spawner.MonsterType != "Flametower")
                        {
                            
                            SpriteComponent monsterSprite = monster.GetComponent<SpriteComponent>();
                            monsterSpatial.Height = (int)(monsterSprite.SrcRect.Height * monsterSprite.Scale / 1.5);
                            monsterSpatial.Width = (int)(monsterSprite.SrcRect.Width * monsterSprite.Scale / 2.5);
                        }
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
