using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.Core.Components;
using CleanGame.Game.Core.Components.Render;
using CleanGame.Game.SGraphics;
using EntityFramework;
using EntityFramework.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CleanGame.Game.Core.Components.Movement;
using CleanGame.Game.Core.GameStates;

namespace CleanGame.Game.Core
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
            if (m != null)
            {
                foreach (Component c in m.Components)
                {
                    ret.AddComponent((Component)c.Clone());
                }
            }
            //ret.Refresh();
            return ret;
        }
        public Entity CreateRangedWeaponEntity(String name, String sprite, String portrait, float range, float baseDamage, float projSpeed, String projectileSprite, int ammo, float cooldown, float price, float ammoprice)
        {

            Entity proj = entityMgr.CreateEntity();

            WeaponComponent wc = new WeaponComponent();
            wc.BaseDamage = baseDamage;
            wc.WeaponName = name;
            wc.Type = WeaponComponent.WeaponType.Ranged;
            wc.Range = range;
            wc.Portrait = portrait;
            wc.ProjectileSpeed = projSpeed;
            wc.ProjectileSprite = projectileSprite;
            wc.Ammo = wc.MaxAmmo = ammo;
            wc.Cooldown = cooldown;
            wc.Price = price;
            wc.AmmoPrice = ammoprice;

            proj.AddComponent(wc);

            return proj;
        }

        public Entity CreateLaserEntity(String name, String sprite, Vector2 origin, Vector2 direction, float range)
        {

            Entity proj = entityMgr.CreateEntity();

            SpatialComponent spatial = new SpatialComponent();
            spatial.Position = new Vector2(origin.X, origin.Y);
            spatial.Width = 6;
            spatial.Height = 400;
            spatial.ShouldRotate = true;
            spatial.DefaultRotationCons = 1f;
            spatial.ConstantRotation = 1f;

            SpriteComponent sc = new SpriteComponent();
            if (sc.xmlName != null && sc.xmlName != "")
                sc.setSpritesheet(sc.spriteName, sc.xmlName, resourceMgr);
            else
                sc.setSprite(sprite, resourceMgr);
            sc.Angle = (float)Math.Atan2(direction.X, -direction.Y);
           
            sc.origin = new Vector2(.5f, 0);
            sc.Scale = range;
            

            PhysicsComponent pc = new PhysicsComponent();
            pc.Origin = new Vector2(0, 0);

            proj.AddComponent(spatial);
            proj.AddComponent(sc);
            proj.AddComponent(pc);
            proj.AddComponent(new LaserComponent());
            proj.AddComponent(new TimeComponent());

            return proj;
        }

        


        public Entity CreateBasicEntity()
        {
            return entityMgr.CreateEntity();
        }
        public Entity CreateMeleeWeaponEntity(String name, String portrait, float range, float baseDamage, int ammo, float cooldown, float price, float ammoprice, string spriteName, string spriteXml)
        {
            Entity proj = entityMgr.CreateEntity();

            WeaponComponent wc = new WeaponComponent();
            wc.BaseDamage = baseDamage;
            wc.WeaponName = name;
            wc.Type = WeaponComponent.WeaponType.Melee;
            wc.Range = range;
            wc.Portrait = portrait;
            wc.Ammo = wc.MaxAmmo = ammo;
            wc.Cooldown = cooldown;
            wc.ProjectileSprite = spriteName;
            wc.SpriteXml = spriteXml;
            wc.Price = price;
            wc.AmmoPrice = ammoprice;

            proj.AddComponent(wc);

            return proj;
        }
        public Entity CreateMeleeEntity(Entity owner, Entity weapon)
        {
            WeaponComponent wc = weapon.GetComponent<WeaponComponent>();

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

            PhysicsComponent pc = new PhysicsComponent();

            MeleeComponent mc = new MeleeComponent();
            mc.Weapon = weapon;
            mc.Owner = owner;

            SpatialComponent spatial = new SpatialComponent();
            spatial.ShouldRotate = true;
            switch (direction.Heading)
            {
                case "Up":
                    
                    spatial.Height = 20;
                    spatial.Width = 40;
                    if (owner.HasComponent<PlayerComponent>())
                    {
                        
                        spatial.Height = 20;
                        spatial.Width = 60;
                        mc.TimePresent = .3f;
                        pc.Origin = new Vector2(0, 0);
                        spatial.ConstantRotation = 16f;
                        spatial.Rotation = .75f;
                    }
                    break;

                case "Down":
                    
                    spatial.Height = 20;
                    spatial.Width = 40;
                    if (owner.HasComponent<PlayerComponent>())
                    {

                        spatial.Height = 25;
                        spatial.Width = 60;
                        mc.TimePresent = .3f;
                        pc.Origin = new Vector2(0, 0);
                        spatial.ConstantRotation = -9.5f;
                        spatial.Rotation = 2.8f;
                    }
                   
                    break;

                case "Right":
                    
                    spatial.Height = 40;
                    spatial.Width = 20;
                    if (owner.HasComponent<PlayerComponent>())
                    {
                        spatial.Height = 70;
                        spatial.Width = 20;
                        mc.TimePresent = .2f;
                        pc.Origin = new Vector2(0,0);
                        spatial.ConstantRotation = -4.8f;
                        spatial.Rotation = -.1f;
                    }
                    break;

                case "Left":
                    
                    spatial.Height = 40;
                    spatial.Width = 20;
                    if (owner.HasComponent<PlayerComponent>())
                    {
                        
                        spatial.Height = 70;
                        spatial.Width = 20;
                        mc.TimePresent = .2f;
                        pc.Origin = new Vector2(0, 0);
                        spatial.ConstantRotation = 8f;
                        spatial.Rotation = .1f;
                    }
                    break;
            }
            spatial.Position = new Vector2(ownerLocation.X, ownerLocation.Y);

            AnimationComponent animation = new AnimationComponent();
            animation.CurrentAnimation = "Attack" + direction.Heading;
            //   animation.CurrentAnimation = "Attack" + "Right";   //Need to change this for all the directions

            SpriteComponent sprite = new SpriteComponent();
            sprite.setSpritesheet(wc.ProjectileSprite, wc.SpriteXml, resourceMgr);
            //sprite.SpriteSheet = wc.MeleeSheet;

            
            //Adding the components to the melee entity
            meleeEntity.AddComponent(spatial);
            meleeEntity.AddComponent(direction);
            meleeEntity.AddComponent(animation);
            meleeEntity.AddComponent(sprite);
            meleeEntity.AddComponent(new TimeComponent());
            meleeEntity.AddComponent(mc);
            meleeEntity.AddComponent(new MovementComponent());
            meleeEntity.AddComponent(pc);

            return meleeEntity;
        }

        public Entity CreatePlayerEntity()
        {
            Entity e = entityMgr.CreateEntity();
            SpatialComponent spatial = new SpatialComponent();
            spatial.Position = new Vector2(150, 150); 
            InventoryComponent ic = new InventoryComponent();
            SpriteComponent sprite = new SpriteComponent();
            sprite.RenderLayer = RenderLayer.BACKGROUND;
            sprite.AnchorPoint = new Vector2(.25f, .25f);
            

            //stats component
            StatsComponent s = new StatsComponent();
            s.BaseDamage = 100;
            s.BaseMoveSpeed = 100;
            s.BaseHealth = 100;
            s.HealthScale = 1;
            s.CurrentHealth = s.MaxHealth;

            //Direction Component
            DirectionComponent direction = new DirectionComponent();
            direction.Heading = "Down";

            sprite.setSpritesheet("playerSheet", "Content\\PlayerAnimation.xml", resourceMgr);
            //sprite.SpriteSheet = spriteSheet;// new SpriteSheet(resourceMgr.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");
            //sprite.SrcRect = sprite.SpriteSheet.Animation["Idle" + direction.Heading][0];
            spatial.Height = (int)(sprite.SrcRect.Height * sprite.Scale / 1.5);
            spatial.Width = (int)(sprite.SrcRect.Width * sprite.Scale / 2.5);
            //sprite.SpriteSheet = new SpriteSheet(resourceMgr.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");
            //Creating an Animation component
            AnimationComponent animation = new AnimationComponent();
            //Changing the animation with the string property
            //  animation.CurrentAnimation = "Down";

            SpellComponent spellComponent = new SpellComponent(); //Includes the melee stuff
           

            //e.AddComponent(new MeleeComponent());

            e.AddComponent(spatial);
            e.AddComponent(sprite);

            e.AddComponent(s);
            e.AddComponent(ic);
            e.AddComponent(spellComponent);

            e.AddComponent(new PhysicsComponent());
            PlayerComponent controllable = new PlayerComponent();
            e.AddComponent(controllable);
            //   e.AddComponent(animation);
            e.AddComponent(direction);
            e.AddComponent(new MovementComponent());
            
            return e;
        }
        public Entity CreateAOEField(Entity owner, Vector2 origin, Vector2 size, String spritesheet, string xmlName, int ticks, float tickInterval, float ConstantRotation, Entity Weapon)
        {
            Entity proj = entityMgr.CreateEntity();

            SpatialComponent spatial = new SpatialComponent();
            spatial.Width = (int)size.X;
            spatial.Height = (int)size.Y;
            spatial.Position = origin + spatial.Size / 2;

            spatial.ConstantRotation = ConstantRotation;

            if (spatial.ConstantRotation > 0)
            {
                spatial.ShouldRotate = true;
            }

            AnimationComponent animation = new AnimationComponent();
            animation.CurrentAnimation = "Idle";

            PhysicsComponent pc = new PhysicsComponent();
            

            SpriteComponent sc = new SpriteComponent();
            sc.setSpritesheet(spritesheet, xmlName, resourceMgr);
            //sc.origin = new Vector2(.25f, .25f);
            //sc.AnchorPoint = new Vector2(.5f, .5f);


            if (Weapon.GetComponent<WeaponComponent>().WeaponName == "FlametowerWeapon")
            {
                spatial.Position = origin;
                spatial.Rotation = -2f;

                pc.Origin = new Vector2(0, 0);

                //sc.SpriteSheet = resourceMgr.GetResource<SpriteSheet>(spritesheet);
                sc.Scale = .5f;
                //sc.AnchorPoint = new Vector2(1, 1);
                sc.origin = new Vector2(.5f, 2.5f);
                //sc.AnchorPoint = new Vector2(0, 1);
                sc.Angle = 1.571f;
            }


            AOEComponent ac = new AOEComponent();
            ac.TickInterval = tickInterval;
            ac.Ticks = ticks;
            ac.Weapon = Weapon.reference;
            ac.Owner = owner.reference;

            proj.AddComponent(spatial);
            proj.AddComponent(animation);
            proj.AddComponent(sc);
            proj.AddComponent(ac);
            proj.AddComponent(pc);

            return proj;
        }
        public Entity CreateProjectile(Entity owner, Vector2 origin, Vector2 direction, String sprite, float range, float speed, Entity weapon)
        {
            Entity proj = entityMgr.CreateEntity();

            ProjectileComponent pc = new ProjectileComponent();
            pc.direction = direction;
            pc.origin = origin;
            pc.range = range;
            pc.owner = owner;
            pc.weapon = weapon;

            SpatialComponent spatial = new SpatialComponent();
            spatial.Position = new Vector2(origin.X, origin.Y);
            spatial.Width = 2;
            spatial.Height = 2;

            SpriteComponent sc = new SpriteComponent();
            if (sc.xmlName != null && sc.xmlName != "")
                sc.setSpritesheet(sc.spriteName, sc.xmlName, resourceMgr);
            else
                sc.setSprite(sprite, resourceMgr);
            sc.Angle = (float)Math.Atan2(direction.X, -direction.Y);
            sc.origin = new Vector2(.5f, 0);
            //sc.AnchorPoint = new Vector2(0, .25f);

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

        public Entity CreateSnipProjectile(Entity owner, Vector2 origin, Vector2 direction, String sprite, float range, float speed, Entity weapon)
        {
            Entity proj = entityMgr.CreateEntity();

            ProjectileComponent pc = new ProjectileComponent();
            pc.direction = direction;
            pc.origin = origin;
            pc.range = range;
            pc.owner = owner;
            pc.weapon = weapon;

            SpatialComponent spatial = new SpatialComponent();
            spatial.Position = new Vector2(origin.X, origin.Y);
            spatial.Width = 30;
            spatial.Height = 15;

            SpriteComponent sc = new SpriteComponent();
            if (sc.xmlName != null && sc.xmlName != "")
                sc.setSpritesheet(sc.spriteName, sc.xmlName, resourceMgr);
            else
                sc.setSprite(sprite, resourceMgr);
            sc.Angle = (float)Math.Atan2(direction.X, -direction.Y);
            sc.origin = new Vector2(0.4f, 0);
            sc.AnchorPoint = new Vector2(0, 0f);
            

            MovementComponent mc = new MovementComponent();
            Vector2 vel = direction * speed;
            mc.Vertical = vel.Y;
            mc.Horizontal = vel.X;

            PhysicsComponent phys = new PhysicsComponent();
            phys.Origin = new Vector2(0, 0);

            proj.AddComponent(pc);
            proj.AddComponent(spatial);
            proj.AddComponent(mc);
            proj.AddComponent(sc);
            proj.AddComponent(phys);

            return proj;
        }

        private Entity CreateMonsterBase(Vector2 pos, string spriteName, string xmlName, float scale) 
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

            //stats component
            StatsComponent s = new StatsComponent();
            s.BaseDamage = 100;
            s.BaseMoveSpeed = 100;
            s.BaseHealth = 100;
            s.CurrentHealth = 100;
            s.HealthScale = 1;

            //Create the Sprite for the new entity
            //  Sprite monsterSprite = sprite;
            SpriteComponent monsterSprite = new SpriteComponent();
            monsterSprite.setSpritesheet(spriteName, xmlName, resourceMgr);
            //monsterSprite.SpriteSheet = spriteSheet;
            monsterSprite.AnchorPoint = new Vector2(.25f, .25f);
            monsterSprite.Scale = scale;
            //monsterSprite.SrcRect = monsterSprite.SpriteSheet.Animation["Idle" + direction.Heading][0];

            AnimationComponent ac = new AnimationComponent();
            ac.CurrentAnimation = "IdleDown";

            //create movement component
            MovementComponent mc = new MovementComponent();

            //Create the TimeComponent for the new entity
            TimeComponent timeComponent = new TimeComponent();
            timeComponent.timeOfLastDraw = new TimeSpan(0, 0, 0, 0, 0);

            //Create AIMovementComponent for the new entity
            MovementComponent movementComponent = new MovementComponent();

            //Add the new components to the entity
            monster.AddComponent(m);
            monster.AddComponent(s);
            monster.AddComponent(mc);
            monster.AddComponent(ac);
            monster.AddComponent(spatial);
            monster.AddComponent(monsterSprite);
            monster.AddComponent(timeComponent);
            monster.AddComponent(movementComponent);

            monster.AddComponent(new PhysicsComponent());
            monster.AddComponent(direction);
            //     monster.AddComponent(new AnimationComponent());
            monster.AddComponent(new SeparationComponent());
            spatial.Height = (int)(monsterSprite.SrcRect.Height * monsterSprite.Scale / 1.5);
            spatial.Width = (int)(monsterSprite.SrcRect.Width * monsterSprite.Scale / 2.5);

            return monster;		
		}
		
		public Entity CreateGrenade(Entity owner, Vector2 origin, Vector2 direction, String sprite, float range, float speed, float fuseTime, Vector2 explosionSize, Entity weapon)
        {
            Entity grenade = entityMgr.CreateEntity();

            GrenadeComponent gc = new GrenadeComponent();
            gc.direction = direction;
            gc.origin = origin;
            gc.range = range;
            gc.owner = owner;
            gc.weapon = weapon;
            gc.fuseTime = fuseTime;
            gc.explosionSize = explosionSize;

            SpatialComponent spatial = new SpatialComponent();
            spatial.Position = new Vector2(origin.X, origin.Y);
            spatial.Width = 2;
            spatial.Height = 2;

            SpriteComponent sc = new SpriteComponent();
            if (sc.xmlName != null && sc.xmlName != "")
                sc.setSpritesheet(sc.spriteName, sc.xmlName, resourceMgr);
            else
                sc.setSprite(sprite, resourceMgr);
            sc.Angle = (float)Math.Atan2(direction.X, -direction.Y);
            sc.origin = new Vector2(.5f, 0);
            //sc.AnchorPoint = new Vector2(0, .25f);

            MovementComponent mc = new MovementComponent();
            Vector2 vel = direction * speed;
            mc.Vertical = vel.Y;
            mc.Horizontal = vel.X;

            grenade.AddComponent(gc);
            grenade.AddComponent(spatial);
            grenade.AddComponent(mc);
            grenade.AddComponent(sc);
            grenade.AddComponent(new PhysicsComponent());
            return grenade;
        }

        public Entity CreateWallEntity(Vector2 topLeft, Vector2 bottomLeft, Vector2 topRight, Vector2 bottomRight)
        {
            Entity wall = entityMgr.CreateEntity();

            wall.AddComponent(new PhysicsComponent());
            wall.AddComponent(new BorderComponent(topLeft, bottomLeft, topRight, bottomRight));

            return wall;
        }

        public Entity CreateSpawner(int xPos, int yPos, Rectangle rectangle, String MonsterType, String MonsterWeapon, int numMobs, TimeSpan timePerSpawn)
        {
            Entity spawner = entityMgr.CreateEntity();

            //Create the Spatial for the new entity
            SpatialComponent spatial = new SpatialComponent();
            spatial.Position = new Vector2(xPos, yPos);

            SpawnerComponent spawnerCmp = new SpawnerComponent();
            spawnerCmp.numMobs = numMobs;
            spawnerCmp.timeOfLastSpawn = new TimeSpan(0, 0, 0, 0, 0);
            spawnerCmp.timePerSpawn = timePerSpawn;
            spawnerCmp.MonsterType = MonsterType;
            spawnerCmp.MonsterWeapon = MonsterWeapon;
            //TODO Spawner Component needs a modifier

            //Add the new components to the entity
            spawner.AddComponent(spatial);
            spawner.AddComponent(spawnerCmp);
            return spawner;
        }

        public Entity CreateSpawner(Spawner s)
        {
            Entity e = entityMgr.CreateEntity();

            //Create the Spatial for the new entity
            SpatialComponent spatial = new SpatialComponent();
            spatial.Position = new Vector2(s.XPosition, s.YPosition);

            SpawnerComponent spawnerCmp = new SpawnerComponent();
            spawnerCmp.numMobs = s.NumberOfMonsters;
            spawnerCmp.timeOfLastSpawn = new TimeSpan(0, 0, 0, 0, 0);
            spawnerCmp.timePerSpawn = s.TimePerSpawn;
            spawnerCmp.MonsterType = s.MonsterType;
            spawnerCmp.MonsterWeapon = s.MonsterWeapon;
            spawnerCmp.HealthUpModifier = s.HealthUpModifier;
            spawnerCmp.DamageUpModifier = s.DamageUpModifier;

            //Add the new components to the entity
            e.AddComponent(spatial);
            e.AddComponent(spawnerCmp);

            return e;
        }
    }
}
