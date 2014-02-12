using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Components;
using EntityFramework;
using EntityFramework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DirtyGame.game.SGraphics;
using DirtyGame.game.Util;
using DirtyGame.game.Core.Events;
using DirtyGame.game.Core.Components.Render;

namespace DirtyGame.game.Core.Systems
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
            WeaponComponent wc = Weapon.GetComponent<WeaponComponent>();
            StatsComponent hc = Target.GetComponent<StatsComponent>();
            StatsComponent os = wc.Owner.GetComponent<StatsComponent>();
            StatsComponent ts = Target.GetComponent<StatsComponent>();

            if (wc == null || hc == null || os == null || ts == null)
                return;

            if (ts.RangedImmune && wc.Type == WeaponComponent.WeaponType.Ranged)
                return;

            int Damage = (int)Math.Floor(wc.BaseDamage * (os.Damage / 100.0f));

            hc.CurrentHealth -= Damage;

            CaptureEventType t = Target.HasComponent<PlayerComponent>() ? CaptureEventType.PlayerDamageTaken : CaptureEventType.MonsterDamageTaken;
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
                        SetShootAnimation(Owner, "Shoot");

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
                        Entity grenade = game.entityFactory.CreateGrenade(Owner, spatial.Center, dir, wc.ProjectileSprite, (float) dist, wc.ProjectileSpeed, 2.0f, new Vector2(128, 128), Weapon);
                        grenade.Refresh();
					}
					
                    else if(wc.WeaponName == "SnipWeapon")
                    {
                        Entity proj = game.entityFactory.CreateSnipProjectile(Owner, spatial.Center, dir, wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, Weapon);
                        proj.Refresh();
                    }
                    else
                    {
                        SetShootAnimation(Owner, "Shoot");
                        Entity proj = game.entityFactory.CreateProjectile(Owner, spatial.Center, dir, wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, Weapon);
                        proj.Refresh();
                    }
                }
                else if (wc.Type == WeaponComponent.WeaponType.Melee)
                {
                    SetShootAnimation(Owner, "Attack");
                    Entity meleeEntity = game.entityFactory.CreateMeleeEntity(Owner, Weapon);
                    meleeEntity.Refresh();
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
            if (detEvt.Owner.entity == null)
                return;
            Entity proj = game.entityFactory.CreateAOEField(detEvt.Owner.entity, detEvt.center, detEvt.size, "BombExplosion", wc.SpriteXml, 1, 1, 0, detEvt.Weapon.entity);
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
