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

namespace DirtyGame.game.Core.Systems
{
    public class WeaponSystem : EntitySystem
    {
        private Dirty game;

        public WeaponSystem(Dirty game)
            : base(SystemDescriptions.WeaponSystem.Aspect, SystemDescriptions.WeaponSystem.Priority)
        {
            this.game = game;
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

                if (Owner.HasComponent<PlayerComponent>())
                    GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.PlayerWeaponFired, wc.WeaponName);
                else
                    GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.MonsterWeaponFired, wc.WeaponName);

                if (wc.WeaponName == "BomberWeapon")
                {
                    Entity proj = game.entityFactory.CreateAOEField(Owner, spatial.Center, new Vector2(128, 128), wc.ProjectileSprite, wc.SpriteXml, 1, 1, 0, Weapon);
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
