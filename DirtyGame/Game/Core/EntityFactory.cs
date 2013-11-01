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
using DirtyGame.game.Core.Components.Movement;

namespace DirtyGame.game.Core
{
    public class EntityFactory
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
            return CreateTestEntity(new SpriteSheet(resourceMgr.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml"), new Vector2(0.0f, 0.0f), "Down");
        }

        public Entity CreateTestEntity(SpriteSheet spriteSheet, Vector2 entityPosition, string animationName)
        {
            Entity e = entityMgr.CreateEntity();
            Spatial spatial = new Spatial();
            spatial.MoveTo(entityPosition);

            Sprite sprite = new Sprite();
            sprite.RenderLayer = RenderLayer.BACKGROUND;
            sprite.SpriteSheet = spriteSheet;

            //Creating an Animation component
            Animation animation = new Animation();
            //Changing the animation with the string property
            animation.CurrentAnimation = animationName;

            e.AddComponent(spatial);
            e.AddComponent(sprite);
            e.AddComponent(animation);
            return e;
        }

        public Entity CreatePlayerEntity()
        {
            Entity e = entityMgr.CreateEntity();
            Spatial spatial = new Spatial();
            spatial.MoveTo(0, 0);

            Sprite sprite = new Sprite();
            sprite.RenderLayer = RenderLayer.BACKGROUND;
            sprite.SpriteSheet = new SpriteSheet(resourceMgr.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");
            sprite.SrcRect = new Rectangle(0, 0, 100, 100);
   
            e.AddComponent(spatial);
            e.AddComponent(sprite);
            e.AddComponent(new Collidable());
            Player controllable = new Player();
            e.AddComponent(controllable);
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

            //Create the TimeComponent for the new entity
            TimeComponent timeComponent = new TimeComponent();
            timeComponent.timeOfLastDraw = new TimeSpan(0,0,0,0,0);

            //Create AIMovementComponent for the new entity
            AIMovementComponent movementComponent = new AIMovementComponent();

            //Add the new components to the entity
            monster.AddComponent(m);
            monster.AddComponent(spatial);
            monster.AddComponent(sprite);
            monster.AddComponent(timeComponent);
            monster.AddComponent(movementComponent);
            monster.AddComponent(new Collidable());


            return monster;
        }

        public Entity CreateSpawner(int xPos, int yPos, SpriteSheet texture, Rectangle rectangle, int numMobs, TimeSpan timePerSpawn)
        {
            Entity spawner = entityMgr.CreateEntity();
            

            //Create the Spatial for the new entity
            Spatial spatial = new Spatial();
            spatial.MoveTo(xPos, yPos);

            //Create the Sprite for the new entity
            Sprite sprite = new Sprite();
            sprite.SpriteSheet = texture;
            sprite.SrcRect = rectangle;

            SpawnerComponent spawnerCmp = new SpawnerComponent();
            spawnerCmp.numMobs = numMobs;
            spawnerCmp.timeOfLastSpawn = new TimeSpan(0, 0, 0, 0, 0);
            spawnerCmp.timePerSpawn = timePerSpawn;
            spawnerCmp.sprite = sprite;


            //Add the new components to the entity
            spawner.AddComponent(spatial);
            spawner.AddComponent(spawnerCmp);
            return spawner;
        }
    }
}
