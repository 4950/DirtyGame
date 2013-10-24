using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Render;
using DirtyGame.game.SGraphics;
using EntityFramework;
using EntityFramework.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Core
{
    class EntityFactory
    {
        private EntityManager entityMgr;
        private ResourceManager resourceMgr;

        public EntityFactory(EntityManager em, ResourceManager resourceMgr)
        {
            entityMgr = em;
            this.resourceMgr = resourceMgr;
        }

        public Entity CreateTestEntity()
        {
            Entity e = entityMgr.CreateEntity();
            Spatial spatial = new Spatial();
            spatial.MoveTo(0, 0);

            Sprite sprite = new Sprite();
            sprite.RenderLayer = RenderLayer.BACKGROUND;
            sprite.Texture = resourceMgr.GetResource<Texture2D>("Player");
            sprite.SrcRect = new Rectangle(0, 0, 100, 100);

            e.AddComponent(spatial);
            e.AddComponent(sprite);
            return e;
        }

        public Entity CreateMonster(string type, int xPos, int yPos, Sprite sprite)
        {
            Entity monster = entityMgr.CreateEntity();

            //Create the MonsterComponent for the new entity
            MonsterComponent m = new MonsterComponent();
            m.monsterType = type;

            //Create the Spatial for the new entity
            Spatial spatial = new Spatial();
            spatial.MoveTo(xPos, yPos);

            //Create the Sprite for the new entity
            Sprite monsterSprite = sprite;

            //Add the new components to the entity
            monster.AddComponent(m);
            monster.AddComponent(spatial);
            monster.AddComponent(sprite);

            return monster;
        }

        public Entity CreateSpawner(int xPos, int yPos, Texture2D texture, Rectangle rectangle, int numMobs, TimeSpan timePerSpawn)
        {
            Entity spawner = entityMgr.CreateEntity();
            

            //Create the Spatial for the new entity
            Spatial spatial = new Spatial();
            spatial.MoveTo(xPos, yPos);

            //Create the Sprite for the new entity
            Sprite sprite = new Sprite();
            sprite.Texture = texture;
            sprite.SrcRect = rectangle;

            SpawnerComponent spawnerCmp = new SpawnerComponent();
            spawnerCmp.numMobs = numMobs;
            spawnerCmp.timeOfLastSpawn = new TimeSpan(0, 0, 0, 0, 0);
            spawnerCmp.timePerSpawn = timePerSpawn;


            //Add the new components to the entity
            spawner.AddComponent(spatial);
            spawner.AddComponent(sprite);
            spawner.AddComponent(spawnerCmp);
            return spawner;
        }
    }
}
