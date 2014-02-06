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
using Dirtygame.game.Util;
using DirtyGame.game.Core.Events;

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
            HealthComponent hc = Target.GetComponent<HealthComponent>();
            StatsComponent s = wc.Owner.GetComponent<StatsComponent>();

            int Damage = (int)Math.Floor(wc.BaseDamage * (s.Damage / 100.0f));

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

                

                if (wc.Type == WeaponComponent.WeaponType.Ranged)
                {
                    //projectile
                    //Vector2 m = new Vector2(ms.X, ms.Y) + renderer.ActiveCamera.Position;
                    Vector2 dir = (Target - spatial.Center);
                    dir.Normalize();

                    if (wc.Name == "Scattershot")
                    {
                        for (float f = -.5f; f <= .5f; f += .25f)
                        {
                            
                            Entity proj = game.entityFactory.CreateProjectile(Owner, spatial.Center, Vector2.Transform(dir, Matrix.CreateRotationZ(f)), wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, Weapon);
                            proj.Refresh();
                        }
                    }
                    else if (wc.Name == "FlametowerWeapon")
                    {
                        Entity proj = game.entityFactory.CreateAOEField(Owner, spatial.Center, new Vector2(wc.Range, 25), wc.ProjectileSprite, 6, .5f, 2.094f, Weapon);
                        proj.Refresh();
                    }
                    else if (wc.Name == "GrenadeLauncher")
                    {
                        Entity grenade = game.entityFactory.CreateGrenade(Owner, spatial.Center, dir, wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, 5.0f, 5.0f, Weapon);
                        grenade.Refresh();
               /*         //Entity proj = game.entityFactory.CreateProjectile(Owner, spatial.Center, dir, wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, Weapon);
                        Vector2 explosionCenter = new Vector2(spatial.Center.X - wc.Range/2, spatial.Center.Y - wc.Range/2);
                        //Entity proj = game.entityFactory.CreateExplosion(Owner, explosionCenter, new Vector2(wc.Range, wc.Range), wc.ProjectileSprite, 2, .5f, Weapon);
                        //proj.Refresh();
                        DetonateEvent detEvt = new DetonateEvent();
                        detEvt.center = explosionCenter;
                        detEvt.Owner = new EntityRef(Owner);
                        detEvt.Weapon = new EntityRef(Weapon);
                        detEvt.name = "Detonate";
                        EventManager.Instance.TriggerEvent(detEvt);
                */
                    }
                    else
                    {
                        Entity proj = game.entityFactory.CreateProjectile(Owner, spatial.Center, dir, wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, Weapon);
                        proj.Refresh();
                    }
                }
                else if (wc.Type == WeaponComponent.WeaponType.Melee)
                {
                    Entity meleeEntity = game.entityFactory.CreateMeleeEntity(Owner, Weapon);
                    meleeEntity.Refresh();
                }
            }
        }

        public void CreateExplosionCallback(Event e)
        {
            DetonateEvent detEvt = (DetonateEvent)e;
            WeaponComponent wc = detEvt.Weapon.entity.GetComponent<WeaponComponent>();
            Entity proj = game.entityFactory.CreateExplosion(detEvt.Owner.entity, detEvt.center, new Vector2(wc.Range, wc.Range), wc.ProjectileSprite, 2, .5f, detEvt.Weapon.entity);
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
