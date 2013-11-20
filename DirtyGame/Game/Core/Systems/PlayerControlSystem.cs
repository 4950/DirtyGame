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

namespace DirtyGame.game.Core.Systems
{
    class PlayerControlSystem : EntitySystem
    {
        private KeyboardState KeyboardState;
        private MouseState ms;
        private MouseState prevMS;
        private EntityFactory entityFactory;
        private Renderer renderer;
        private Dirty game;

        public PlayerControlSystem(EntityFactory ef, Renderer renderer, Dirty game)
            : base(SystemDescriptions.PlayerControlSystem.Aspect, SystemDescriptions.PlayerControlSystem.Priority)
        {
            this.entityFactory = ef;
            this.renderer = renderer;
            this.game = game;
            game.baseContext.RegisterHandler(Keys.Tab, changeWeapon, null);
            
        }
        private void changeWeapon(Keys key)
        {
            InventoryComponent ic = game.player.GetComponent<InventoryComponent>();
            var weapons = ic.WeaponList;
            if (ic.CurrentWeapon == null)
            {
                if (weapons.Count > 0)
                    ic.setCurrentWeapon(weapons[0]);
            }
            else
            {
                int i = weapons.IndexOf(ic.CurrentWeapon);
                i++;
                if (i >= weapons.Count)
                    i = 0;
                ic.setCurrentWeapon(weapons[i]);
            }
        }
        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            foreach (Entity e in entities)
            {
    //            if (!e.HasComponent<Player>()) continue;

                SpatialComponent spatial = e.GetComponent<SpatialComponent>();
                DirectionComponent direction = e.GetComponent<DirectionComponent>();
                MovementComponent movement = e.GetComponent<MovementComponent>();
                

                KeyboardState = Keyboard.GetState();
                if (KeyboardState.IsKeyDown(Keys.Left) || KeyboardState.IsKeyDown(Keys.A))
                {
                    //Arbitrarily chosen number of pixels... speed can easily be added if we want
                    movement.Horizontal = -5;

                    direction.Heading = "Left";
                }
                else if (KeyboardState.IsKeyDown(Keys.Right) || KeyboardState.IsKeyDown(Keys.D))
                {
                    movement.Horizontal = 5;

                    direction.Heading = "Right";

                }
                else
                {
                    movement.Horizontal = 0;
                }

                if (KeyboardState.IsKeyDown(Keys.Up) || KeyboardState.IsKeyDown(Keys.W))
                {
                    movement.Vertical = -5;

                    direction.Heading = "Up";
                }
                else if (KeyboardState.IsKeyDown(Keys.Down) || KeyboardState.IsKeyDown(Keys.S))
                {
                    movement.Vertical = 5;

                    direction.Heading = "Down";
                }
                else
                {
                    movement.Vertical = 0;
                }

                

                prevMS = ms;
                ms = Mouse.GetState();
                if (prevMS == null)
                    prevMS = ms;
                if (ms.RightButton == ButtonState.Pressed && prevMS.RightButton == ButtonState.Released)//right mouse down
                {
                    Entity curWeapon = game.player.GetComponent<InventoryComponent>().CurrentWeapon;

                    if (curWeapon != null)
                    {
                        WeaponComponent wc = curWeapon.GetComponent<WeaponComponent>();

                        if (wc.Type == WeaponComponent.WeaponType.Ranged && (wc.Ammo > 0 || wc.MaxAmmo == -1) && wc.LastFire <= 0)
                        {
                            if(wc.Ammo > 0)
                                wc.Ammo--;
                            wc.LastFire = wc.Cooldown;
                            //projectile
                            Vector2 m = new Vector2(ms.X, ms.Y) + renderer.ActiveCamera.Position;
                            Vector2 dir = (m - spatial.Position);

                            dir.Normalize();
                            Entity proj = entityFactory.CreateProjectile(e, spatial.Position, dir, wc.ProjectileSprite, wc.Range, wc.ProjectileSpeed, wc.BaseDamage);

                            proj.Refresh();
                        }
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
