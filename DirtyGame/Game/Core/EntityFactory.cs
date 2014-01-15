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
        public Entity CloneEntity(Entity m)
        {
            Entity ret = entityMgr.CreateEntity();

            foreach (Component c in m.Components)
            {
                ret.AddComponent((Component)c.Clone());
            }
            ret.Refresh();
            return ret;
        }
        public Entity CreateTestEntity()
        {
            return CreateTestEntity(new SpriteSheet(resourceMgr.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml"), new Vector2(0.0f, 0.0f), "Down");
        }

        public Entity CreateTestEntity(SpriteSheet spriteSheet, Vector2 entityPosition, string animationName)
        {
            Entity e = entityMgr.CreateEntity();
            SpatialComponent spatial = new SpatialComponent();
            spatial.Position = entityPosition;

            SpriteComponent sprite = new SpriteComponent();
            sprite.RenderLayer = RenderLayer.BACKGROUND;
            sprite.SpriteSheet = spriteSheet;

            //Creating an Animation component
            AnimationComponent animation = new AnimationComponent();
            //Changing the animation with the string property
            animation.CurrentAnimation = animationName;
            
            e.AddComponent(spatial);
            e.AddComponent(sprite);
            e.AddComponent(animation);
            return e;
        }
        public Entity CreateRangedWeaponEntity(String name, String sprite, String portrait, float range, float baseDamage, float projSpeed, String projectileSprite, int ammo, float cooldown)
        {
            
            Entity proj = entityMgr.CreateEntity();

            WeaponComponent wc = new WeaponComponent();
            wc.BaseDamage = baseDamage;
            wc.Name = name;
            wc.Type = WeaponComponent.WeaponType.Ranged;
            wc.Range = range;
            wc.Portrait = portrait;
            wc.ProjectileSpeed = projSpeed;
            wc.ProjectileSprite = projectileSprite;
            wc.Ammo = wc.MaxAmmo = ammo;
            wc.Cooldown = cooldown;

            proj.AddComponent(wc);

            return proj;
        }
        public Entity CreateBasicEntity()
        {
            return entityMgr.CreateEntity();
        }
        public Entity CreateMeleeWeaponEntity(String name, String portrait, float range, float baseDamage, int ammo, float cooldown, SpriteSheet meleeSprite)
        {
            Entity proj = entityMgr.CreateEntity();

            WeaponComponent wc = new WeaponComponent();
            wc.BaseDamage = baseDamage;
            wc.Name = name;
            wc.Type = WeaponComponent.WeaponType.Melee;
            wc.Range = range;
            wc.Portrait = portrait;
            wc.Ammo = wc.MaxAmmo = ammo;
            wc.Cooldown = cooldown;
            wc.MeleeSheet = meleeSprite;

            proj.AddComponent(wc);

            return proj;
        }
        public Entity CreateMeleeEntity(Entity owner, WeaponComponent wc)
        {
            Entity meleeEntity = entityMgr.CreateEntity();

            //TODO: Need to put in the parts of the melee entity
            //Things in this entity
            //      - Physics = Brandon
            //      - Boundary Box
            //      - Animation = Jared
            //      - Effect
            //      - Directional = Jared
            //      - Sprite = Jared


            //Current player location
            Vector2 ownerLocation = owner.GetComponent<SpatialComponent>().Position;

            DirectionComponent direction = new DirectionComponent();
            direction.Heading = owner.GetComponent<DirectionComponent>().Heading;

            SpatialComponent spatial = new SpatialComponent();
            float xOffset = 0.0f;
            float yOffset = 0.0f;
            switch (direction.Heading)
            {
                case "Up":
                    xOffset = 0.0f;
                    yOffset = -20.0f;
                    spatial.Height = 20;
                    spatial.Width = 40;
                    break;

                case "Down":
                    xOffset = 0.0f;
                    yOffset = 50.0f;
                    spatial.Height = 20;
                    spatial.Width = 40;
                    break;

                case "Right":
                    xOffset = 40.0f;
                    yOffset = 0.0f;
                    spatial.Height = 40;
                    spatial.Width = 20;
                    break;

                case "Left":
                    xOffset = -15.0f;
                    yOffset = 0.0f;
                    spatial.Height = 40;
                    spatial.Width = 20;
                    break;
            }
            spatial.Position = new Vector2(ownerLocation.X + xOffset, ownerLocation.Y + yOffset);

            AnimationComponent animation = new AnimationComponent();
            animation.CurrentAnimation = "Attack" + direction.Heading;
         //   animation.CurrentAnimation = "Attack" + "Right";   //Need to change this for all the directions

            SpriteComponent sprite = new SpriteComponent();
            sprite.SpriteSheet = wc.MeleeSheet;

            MeleeComponent mc = new MeleeComponent();
            mc.Damage = wc.BaseDamage;
            mc.Owner = owner;

            //Adding the components to the melee entity
            meleeEntity.AddComponent(spatial);
            meleeEntity.AddComponent(direction);
            meleeEntity.AddComponent(animation);
            meleeEntity.AddComponent(sprite);
            meleeEntity.AddComponent(mc);
            meleeEntity.AddComponent(new PhysicsComponent());

            return meleeEntity;
        }
        public Entity CreatePlayerEntity(SpriteSheet spriteSheet)
        {
            Entity e = entityMgr.CreateEntity();
            SpatialComponent spatial = new SpatialComponent();
            spatial.Position = new Vector2(2, 2);

            InventoryComponent ic = new InventoryComponent();

            SpriteComponent sprite = new SpriteComponent();
            sprite.RenderLayer = RenderLayer.BACKGROUND;

                        
            //Direction Component
            DirectionComponent direction = new DirectionComponent();
            direction.Heading = "Down";

            sprite.SpriteSheet = spriteSheet;// new SpriteSheet(resourceMgr.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");
            sprite.SrcRect = sprite.SpriteSheet.Animation["Idle" + direction.Heading][0];

            //sprite.SpriteSheet = new SpriteSheet(resourceMgr.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");
            //Creating an Animation component
            AnimationComponent animation = new AnimationComponent();
            //Changing the animation with the string property
        //  animation.CurrentAnimation = "Down";

            SpellComponent spellComponent = new SpellComponent(); //Includes the melee stuff
            

            HealthComponent hc = new HealthComponent();
            hc.MaxHealth = 200;
            hc.CurrentHealth = 200;

            e.AddComponent(new MeleeComponent());

            e.AddComponent(spatial);
            e.AddComponent(sprite);
            e.AddComponent(hc);
            e.AddComponent(ic);
            e.AddComponent(spellComponent);
         
            e.AddComponent(new PhysicsComponent());
            PlayerComponent controllable = new PlayerComponent();
            e.AddComponent(controllable);
         //   e.AddComponent(animation);
            e.AddComponent(direction);
            e.AddComponent(new MovementComponent());
            e.GetComponent<SpatialComponent>().Height = 20;
            e.GetComponent<SpatialComponent>().Width = 20;
            return e;
        }
        public Entity CreateProjectile(Entity owner, Vector2 origin, Vector2 direction, String sprite, float range, float speed, float damage)
        {
            Entity proj = entityMgr.CreateEntity();

            ProjectileComponent pc = new ProjectileComponent();
            pc.direction = direction;
            pc.origin = origin;
            pc.range = range;
            pc.owner = owner;
            pc.damage = damage;

            SpatialComponent spatial = new SpatialComponent();
            spatial.Position = new Vector2(origin.X, origin.Y);
            spatial.Width = 2;
            spatial.Height = 2;

            SpriteComponent sc = new SpriteComponent();
            sc.sprite = resourceMgr.GetResource<Texture2D>(sprite);
            sc.SrcRect = sc.sprite.Bounds;
            sc.Angle = (float)Math.Atan2(direction.X, -direction.Y);
            sc.origin = new Vector2(.5f, 0);

            MovementComponent mc = new MovementComponent();
            Vector2 vel = direction * speed;
            mc.Vertical = vel.Y;
            mc.Horizontal = vel.X;

            proj.AddComponent(pc);
            proj.AddComponent(spatial);
            proj.AddComponent(mc);
            proj.AddComponent(sc);
            proj.AddComponent(new PhysicsComponent());

            return proj;
        }
        private Entity CreateMonsterBase(Vector2 pos, SpriteSheet spriteSheet)
        {
            Entity monster = entityMgr.CreateEntity();

            //Create the MonsterComponent for the new entity
            MonsterComponent m = new MonsterComponent();
            //m.data = data;

            //Create the Spatial for the new entity
            SpatialComponent spatial = new SpatialComponent();
            spatial.Position = pos;

            //Direction Component
            DirectionComponent direction = new DirectionComponent();
            direction.Heading = "Down";

            //Create the Sprite for the new entity
            //  Sprite monsterSprite = sprite;
            SpriteComponent monsterSprite = new SpriteComponent();
            monsterSprite.SpriteSheet = spriteSheet;
            monsterSprite.origin = new Vector2(.5f, 1);
            monsterSprite.SrcRect = monsterSprite.SpriteSheet.Animation["Idle" + direction.Heading][0];

            //create movement component
            MovementComponent mc = new MovementComponent();

            HealthComponent hc = new HealthComponent();

            //Create the TimeComponent for the new entity
            TimeComponent timeComponent = new TimeComponent();
            timeComponent.timeOfLastDraw = new TimeSpan(0, 0, 0, 0, 0);

            //Create AIMovementComponent for the new entity
            MovementComponent movementComponent = new MovementComponent();

            //Add the new components to the entity
            monster.AddComponent(m);
            monster.AddComponent(mc);
            monster.AddComponent(spatial);
            monster.AddComponent(monsterSprite);
            monster.AddComponent(timeComponent);
            monster.AddComponent(movementComponent);
            monster.AddComponent(hc);

            monster.AddComponent(new PhysicsComponent());
            monster.AddComponent(direction);
       //     monster.AddComponent(new AnimationComponent());
            monster.AddComponent(new SeparationComponent());
            monster.GetComponent<SpatialComponent>().Height = 20;
            monster.GetComponent<SpatialComponent>().Width = 20;

            return monster;
        }
        public Entity CreateBasicMonster(Vector2 pos, SpriteSheet spriteSheet)
        {
            Entity monster = CreateMonsterBase(pos, spriteSheet);

            HealthComponent hc = monster.GetComponent<HealthComponent>();
            hc.CurrentHealth = hc.MaxHealth = 100;

            return monster;
        }
        public Entity CreateRangedMonster(Vector2 pos, SpriteSheet spriteSheet, Entity rangedWeapon)
        {
            Entity monster = CreateBasicMonster(pos, spriteSheet);

            InventoryComponent ic = new InventoryComponent();
            ic.addWeapon(rangedWeapon);

            monster.AddComponent(ic);

            return monster;
        }

        public Entity CreateWallEntity(Vector2 topLeft, Vector2 bottomLeft, Vector2 topRight, Vector2 bottomRight)
        {
            Entity wall = entityMgr.CreateEntity();

            wall.AddComponent(new PhysicsComponent());
            wall.AddComponent(new BorderComponent(topLeft, bottomLeft, topRight, bottomRight));

            return wall;
        }

        public Entity CreateSpawner(int xPos, int yPos, SpriteSheet texture, Rectangle rectangle, MonsterData data, int numMobs, TimeSpan timePerSpawn)
        {
            Entity spawner = entityMgr.CreateEntity();

            //Create the Spatial for the new entity
            SpatialComponent spatial = new SpatialComponent();
            spatial.Position = new Vector2(xPos, yPos);

            //Create the Sprite for the new entity
            SpriteComponent sprite = new SpriteComponent();
            sprite.SpriteSheet = texture;
            sprite.SrcRect = rectangle;

            SpawnerComponent spawnerCmp = new SpawnerComponent();
            spawnerCmp.numMobs = numMobs;
            spawnerCmp.timeOfLastSpawn = new TimeSpan(0, 0, 0, 0, 0);
            spawnerCmp.timePerSpawn = timePerSpawn;
            spawnerCmp.sprite = sprite;
            spawnerCmp.data = data;

            //Add the new components to the entity
            spawner.AddComponent(spatial);
            spawner.AddComponent(spawnerCmp);
            return spawner;
        }
    }
}
