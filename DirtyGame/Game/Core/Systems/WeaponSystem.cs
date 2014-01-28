﻿using System;
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
        public void FireWeapon(Entity Weapon, Entity Owner, Vector2 Target)
        {
            if (Weapon == null || Owner == null)
                return;

            WeaponComponent wc = Weapon.GetComponent<WeaponComponent>();
            SpatialComponent spatial = Owner.GetComponent<SpatialComponent>();
            StatsComponent s = Owner.GetComponent<StatsComponent>();

            if (wc == null || spatial == null || s == null)
                return;

            if ((wc.Ammo > 0 || wc.MaxAmmo == -1) && wc.LastFire <= 0)
            {
                if (wc.Ammo > 0)
                    wc.Ammo--;
                wc.LastFire = wc.Cooldown;

                int Damage = (int)Math.Floor(wc.BaseDamage * (s.Damage / 100.0f));

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
                            
                            Entity proj = game.entityFactory.CreateProjectile(Owner, spatial.Center, Vector2.Transform(dir, Matrix.CreateRotationZ(f)), wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, Damage);
                            proj.Refresh();
                        }
                    }
                    else
                    {
                        Entity proj = game.entityFactory.CreateProjectile(Owner, spatial.Center, dir, wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, Damage);
                        proj.Refresh();
                    }
                }
                else if (wc.Type == WeaponComponent.WeaponType.Melee)
                {
                    Entity meleeEntity = game.entityFactory.CreateMeleeEntity(Owner, wc);
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
