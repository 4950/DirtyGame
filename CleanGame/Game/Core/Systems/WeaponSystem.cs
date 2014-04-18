using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.Core.Systems.Util;
using CleanGame.Game.Core.Components;
using EntityFramework;
using EntityFramework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CleanGame.Game.SGraphics;
using CleanGame.Game.Util;
using CleanGame.Game.Core.Events;
using CleanGame.Game.Core.Components.Render;
using GameService;

namespace CleanGame.Game.Core.Systems
{
    public class WeaponSystem : EntitySystem
    {
        private Dirty game;

        public WeaponSystem(Dirty game)
            : base(SystemDescriptions.WeaponSystem.Aspect, SystemDescriptions.WeaponSystem.Priority)
        {
            this.game = game;
            EventManager.Instance.AddListener("Detonate", CreateExplosionCallback);
        }
        public override void OnEntityAdded(Entity e)
        {
            // do nothing
        }

        public override void OnEntityRemoved(Entity e)
        {
            // do nothing
        }
        public void DealDamage(Entity Weapon, Entity Target)
        {
            if (Weapon == null || Target == null)
                return;

            WeaponComponent wc = Weapon.GetComponent<WeaponComponent>();
            StatsComponent hc = Target.GetComponent<StatsComponent>();
            StatsComponent ts = Target.GetComponent<StatsComponent>();
            int Damage; //Ling's bad code
            CaptureEventType t; //Ling's bad code

            if (wc.WeaponName == "LandmineWeapon") //Ling's code
            {
                Damage = 23;//TODO: Make this get the value from the xml
                hc.CurrentHealth -= Damage;
                t = Target.HasComponent<PlayerComponent>() ? CaptureEventType.PlayerDamageTaken : CaptureEventType.MonsterDamageTaken;
                GameplayDataCaptureSystem.Instance.LogEvent(t, Damage.ToString());
                return;
            }

            if (wc == null || wc.Owner == null)
                return;

            StatsComponent os = wc.Owner.GetComponent<StatsComponent>();

            if (hc == null || os == null || ts == null)
                return;

            if (ts.ImmuneTo != null)
                foreach (string weapon in ts.ImmuneTo)
                {
                    if (weapon == wc.WeaponName)
                    {
                        return;
                    }
                }

            if (wc.Owner == game.player)
                game.gLogicSystem.PlayerDealtDamage();
            else if (Target == game.player)
                game.gLogicSystem.PlayerTookDamage();

            Damage = (int)Math.Floor(wc.BaseDamage * (os.Damage / 100.0f));

            if (Target == wc.Owner)
            {
                if (Target.HasComponent<PropertyComponent<String>>())
                {
                    if (Target.GetComponent<PropertyComponent<String>>("MonsterType").value == "SuicideBomber")
                    {
                        Damage = ts.BaseHealth;
                    }
                }
            }
            hc.CurrentHealth -= Damage;
            if (hc.CurrentHealth < 0)
            {
                Damage += (int)hc.CurrentHealth;
                hc.CurrentHealth = 0;
            }

            t = Target.HasComponent<PlayerComponent>() ? CaptureEventType.PlayerDamageTaken : CaptureEventType.MonsterDamageTaken;
            GameplayDataCaptureSystem.Instance.LogEvent(t, Damage.ToString());
            if (t == CaptureEventType.MonsterDamageTaken)
                GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.MonsterType, Target.GetComponent<PropertyComponent<String>>("MonsterType").value);
            t = Target.HasComponent<PlayerComponent>() ? CaptureEventType.PlayerHitByWeapon : CaptureEventType.MonsterHitByWeapon;
            GameplayDataCaptureSystem.Instance.LogEvent(t, wc.WeaponName);
        }

        public void DealDamage(int Damage, Entity Target)
        {
            if (Target == null)
                return;


            StatsComponent hc = Target.GetComponent<StatsComponent>();
            CaptureEventType t;


            if (hc == null)
                return;



            if (Target == game.player)
                game.gLogicSystem.PlayerTookDamage();



            hc.CurrentHealth -= Damage;
            if (hc.CurrentHealth < 0)
            {
                Damage += (int)hc.CurrentHealth;
                hc.CurrentHealth = 0;
            }

            t = Target.HasComponent<PlayerComponent>() ? CaptureEventType.PlayerDamageTaken : CaptureEventType.MonsterDamageTaken;
            GameplayDataCaptureSystem.Instance.LogEvent(t, Damage.ToString());

        }

        public void FireWeapon(Entity Weapon, Entity Owner, Vector2 Target)
        {
            if (Weapon == null || Owner == null)
                return;

            WeaponComponent wc = Weapon.GetComponent<WeaponComponent>();
            SpatialComponent spatial = Owner.GetComponent<SpatialComponent>();


            if (wc == null || spatial == null)
                return;


            if ((wc.Ammo > 0 || wc.MaxAmmo == -1) && wc.LastFire <= 0)
            {
                if (wc.Ammo > 0)
                    wc.Ammo--;
                wc.LastFire = wc.Cooldown;

                //sound
                if (wc.FireSound != null && wc.FireSound != "")
                    SoundSystem.Instance.PlayEffect(wc.FireSound);

                if (Owner.HasComponent<PlayerComponent>())
                    GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.PlayerWeaponFired, wc.WeaponName);
                else
                    GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.MonsterWeaponFired, wc.WeaponName);

                if (wc.WeaponName == "BomberWeapon")
                {
                    Entity proj = game.entityFactory.CreateAOEField(Owner, spatial.Center, new Vector2(128, 128), wc.ProjectileSprite, wc.SpriteXml, 1, 1, 0, Weapon);
                    proj.Refresh();
                }
                else if (wc.WeaponName == "LandmineWeapon")
                {
                    Vector2 dir = (Target - spatial.Center);
                    dir.Normalize();

                    SetShootAnimation(Owner, "Shoot");
                    Entity proj = game.entityFactory.CreateProjectile(Owner, spatial.Center, dir, wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, Weapon);
                    proj.Refresh();
                }
                else if (wc.Type == WeaponComponent.WeaponType.Ranged)
                {
                    //projectile
                    //Vector2 m = new Vector2(ms.X, ms.Y) + renderer.ActiveCamera.Position;
                    Vector2 dir = (Target - spatial.Center);
                    dir.Normalize();

                    if (wc.WeaponName == "Scattershot")
                    {
                        SetShootAnimation(Owner, "Shoot", setShootAnimationDirection(dir, Owner));

                        for (float f = -.5f; f <= .5f; f += .25f)
                        {
                            Entity proj = game.entityFactory.CreateProjectile(Owner, spatial.Center, Vector2.Transform(dir, Matrix.CreateRotationZ(f)), wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, Weapon);
                            proj.Refresh();
                        }
                    }
                    else if (wc.WeaponName == "FlametowerWeapon")
                    {
                        Entity proj = game.entityFactory.CreateAOEField(Owner, spatial.Center, new Vector2(wc.Range, 25), wc.ProjectileSprite, wc.SpriteXml, 6, .5f, 2.094f, Weapon);
                        proj.Refresh();
                    }

                    else if (wc.WeaponName == "GrenadeLauncher")
                    {
                        double dist = getDistance(spatial.Center.X, spatial.Center.Y, Target.X, Target.Y);
                        Entity grenade = game.entityFactory.CreateGrenade(Owner, spatial.Center, dir, wc.ProjectileSprite, (float) dist, wc.ProjectileSpeed, 0.9f, new Vector2(128, 128), Weapon);
                        grenade.Refresh();
                    }

                    else if (wc.WeaponName == "SnipWeapon")
                    {
                        SetShootAnimation(Owner, "Shoot");
                        Entity proj = game.entityFactory.CreateSnipProjectile(Owner, spatial.Center, dir, wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, Weapon);
                        proj.Refresh();
                    }
                    else
                    {
                        SetShootAnimation(Owner, "Shoot", setShootAnimationDirection(dir, Owner));
                        Entity proj = game.entityFactory.CreateProjectile(Owner, spatial.Center, dir, wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, Weapon);
                        proj.Refresh();
                    }
                }
                else if (wc.Type == WeaponComponent.WeaponType.Melee)
                {

                    //Entity meleeEntity;

                    if (wc.Owner.HasComponent<PlayerComponent>()) //Owned by player
                    {
                        Entity meleeEntity;
                        SetShootAnimation(Owner, "BigSlash");
                        meleeEntity = game.entityFactory.CreateMeleeEntity(Owner, Weapon);
                        meleeEntity.Refresh();

                    }
                    else
                    {
                        //SetShootAnimation(Owner, "Attack");
                        //meleeEntity = game.entityFactory.CreateMeleeEntity(Owner, Weapon);

                    }

                    //meleeEntity.Refresh();
                }

            }
        }

        private static void SetShootAnimation(Entity Owner, string attack)
        {
            AnimationComponent ac;
            if (!Owner.HasComponent<AnimationComponent>())
            {
                ac = new AnimationComponent();
                ac.CurrentAnimation = attack + Owner.GetComponent<DirectionComponent>().Heading;

                Owner.AddComponent(ac);
                Owner.Refresh();
            }
            else
            {
                ac = Owner.GetComponent<AnimationComponent>();
                ac.CurrentAnimation = attack + Owner.GetComponent<DirectionComponent>().Heading;
            }
        }

        private static void SetShootAnimation(Entity Owner, string attack, string heading)
        {
            AnimationComponent ac;
            if (!Owner.HasComponent<AnimationComponent>())
            {
                ac = new AnimationComponent();
                ac.CurrentAnimation = attack + heading;

                Owner.AddComponent(ac);
                Owner.Refresh();
            }
            else
            {
                ac = Owner.GetComponent<AnimationComponent>();
                ac.CurrentAnimation = attack + heading;
            }
        }

        private string setShootAnimationDirection(Vector2 floatDir, Entity m)
        {
            DirectionComponent direction = m.GetComponent<DirectionComponent>();
            string directionHeading;
            directionHeading = direction.Heading;
            double[] dir = new double[2];

            double angle = Math.Atan2(floatDir.Y, floatDir.X); // This is opposite y angle.
            dir[0] = Math.Cos(angle);
            dir[1] = Math.Sin(angle);

            if (Math.Abs(dir[0]) > Math.Abs(dir[1]))
            {
                if (dir[0] > 0)
                {
                    directionHeading = "Right";

                }
                else if (dir[0] < 0)
                {
                    directionHeading = "Left";

                }
            }

            else
            {
                if (dir[1] > 0)
                {
                    directionHeading = "Down";

                }
                else if (dir[1] < 0)
                {
                    directionHeading = "Up";

                }
            }

            return directionHeading;
        }

        private double getDistance(double x, double y, double ox, double oy)
        {
            return Math.Sqrt(
                (Math.Pow(ox - x, 2.0))
                + (Math.Pow(oy - y, 2.0))
                );
        }

        public void CreateExplosionCallback(Event e)
        {
            DetonateEvent detEvt = (DetonateEvent)e;
            WeaponComponent wc = detEvt.Weapon.entity.GetComponent<WeaponComponent>();
            // If grenadier that owned grenade has died, the grenade weapon entity becomes its own owner.
            EntityRef owner;
            if (detEvt.Owner.entity == null)
            {
                owner = detEvt.Weapon;
            }
            else
            {
                owner = detEvt.Owner;
            }
            Entity proj = game.entityFactory.CreateAOEField(owner.entity, detEvt.center, detEvt.size, "BombExplosion", wc.SpriteXml, 1, 1, 0, detEvt.Weapon.entity);
            proj.Refresh();
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            foreach (Entity e in entities)
            {
                if (e.HasComponent<WeaponComponent>())
                {
                    WeaponComponent wc = e.GetComponent<WeaponComponent>();
                    if (wc.LastFire > 0)
                        wc.LastFire -= dt;
                }
            }
        }
    }
}
