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
using DirtyGame.game.Input;
using DirtyGame.game.Core.Components.Render;

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

        enum MoveDirection
        {
            Up,
            Down,
            Left,
            Right,
            Idle
        };

        private MoveDirection currentDirection = MoveDirection.Idle;
        private MoveDirection previousDirection = MoveDirection.Idle;

        private bool meleeAttacking = false;

        //private bool movingUpStart = false;
        //private bool movingDownStart = false;
        //private bool movingLeftStart = false;
        //private bool movingRightStart = false;
        ////private bool attackingStart = false;

        //private bool movingUpFinish = true;
        //private bool movingDownFinish = true;
        //private bool movingLeftFinish = true;
        //private bool movingRightFinish = true;
        ////private bool attackingFinish = true;

        //private bool movingUpPressing = false;
        //private bool movingDownPressing = false;
        //private bool movingLeftPressing = false;
        //private bool movingRightPressing = false;
        ////private bool attackingPressing = false;

        //private bool movingUpAnimationAdded = false;
        //private bool movingDownAnimationAdded = false;
        //private bool movingLeftAnimationAdded = false;
        //private bool movingRightAnimationAdded = false;
        ////private bool attackingAnimationAdded = false;

        public PlayerControlSystem(EntityFactory ef, Renderer renderer, Dirty game)
            : base(SystemDescriptions.PlayerControlSystem.Aspect, SystemDescriptions.PlayerControlSystem.Priority)
        {
            this.entityFactory = ef;
            this.renderer = renderer;
            this.game = game;
            game.baseContext.RegisterHandler(Keys.Tab, changeWeapon, null);

            game.baseContext.RegisterHandler(Keys.Left, move, idle);
            game.baseContext.RegisterHandler(Keys.Right, move, idle);
            game.baseContext.RegisterHandler(Keys.Up, move, idle);
            game.baseContext.RegisterHandler(Keys.Down, move, idle);

            game.baseContext.RegisterHandler(Keys.A, move, idle);
            game.baseContext.RegisterHandler(Keys.D, move, idle);
            game.baseContext.RegisterHandler(Keys.W, move, idle);
            game.baseContext.RegisterHandler(Keys.S, move, idle);

            game.baseContext.RegisterHandler(Keys.Space, meleeAttack, null);
        }
        private void meleeAttack(Keys key)
        {
            this.meleeAttacking = !this.meleeAttacking;
        }
        private void move(Keys key)
        {
            previousDirection = currentDirection;
            switch (key)
            {
                case Keys.Up:
                case Keys.W:
                    currentDirection = MoveDirection.Up;
                    break;
                case Keys.Down:
                case Keys.S:
                    currentDirection = MoveDirection.Down;
                    break;
                case Keys.Left:
                case Keys.A:
                    currentDirection = MoveDirection.Left;
                    break;
                case Keys.Right:
                case Keys.D:
                    currentDirection = MoveDirection.Right;
                    break;
                default:
                    currentDirection = MoveDirection.Idle;
                    break;
            }
        }
        private void idle(Keys key)
        {
            move(key);
            if (previousDirection == currentDirection)
                currentDirection = MoveDirection.Idle;
            else
                currentDirection = previousDirection;
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
                SpriteComponent sprite = e.GetComponent<SpriteComponent>();

                //<<<<<<< HEAD
                //                KeyboardState = Keyboard.GetState();
                //                if (KeyboardState.IsKeyDown(Keys.Left) || KeyboardState.IsKeyDown(Keys.A))
                //                {
                //                    //Arbitrarily chosen number of pixels... speed can easily be added if we want
                //                    movement.Horizontal = -5;

                //                    direction.Heading = "Left";
                //                }
                //                else if (KeyboardState.IsKeyDown(Keys.Right) || KeyboardState.IsKeyDown(Keys.D))
                //                {
                //                    movement.Horizontal = 5;
                //=======
                //if (!attackingPressing)
                //{

                //Attacking
                if (meleeAttacking)
                {
                    meleeAttacking = false;
                    Entity meleeEntity = entityFactory.CreateMeleeEntity(e);
                    meleeEntity.Refresh();

                    //Entity testEntity = entityFactory.CreateBasicMonster(new Vector2(50.0f, 50.0f), e.GetComponent<SpriteComponent>().SpriteSheet);
                    //testEntity.Refresh();
                    
                }

                if (previousDirection != currentDirection)//direction state changed
                {
                    movement.Vertical = 0;
                    movement.Horizontal = 0;
                    AnimationComponent animationComponent = e.GetComponent<AnimationComponent>();
                    if ((animationComponent != null))
                    {
                        e.RemoveComponent<AnimationComponent>();
                        e.Refresh();
                        sprite.SrcRect = sprite.SpriteSheet.Animation["Idle" + direction.Heading][0]; //I do not know if this is the best way to do this - JP
                    }
                    string anim = "Walk";
                    switch (currentDirection)
                    {
                        case MoveDirection.Up:
                            movement.Vertical = -5;
                            direction.Heading = "Up";
                            if (!e.HasComponent<AnimationComponent>())
                            {
                                AnimationComponent animation = new AnimationComponent();
                                animation.CurrentAnimation = anim + direction.Heading;
                                e.AddComponent(animation);
                                e.Refresh();
                            }
                            break;
                        case MoveDirection.Down:
                            movement.Vertical = 5;
                            direction.Heading = "Down";
                            if (!e.HasComponent<AnimationComponent>())
                            {
                                AnimationComponent animation = new AnimationComponent();
                                animation.CurrentAnimation = anim + direction.Heading;
                                e.AddComponent(animation);
                                e.Refresh();
                            }
                            break;
                        case MoveDirection.Left:
                            movement.Horizontal = -5;
                            direction.Heading = "Left";
                            if (!e.HasComponent<AnimationComponent>())
                            {
                                AnimationComponent animation = new AnimationComponent();
                                animation.CurrentAnimation = anim + direction.Heading;
                                e.AddComponent(animation);
                                e.Refresh();
                            }
                            break;
                        case MoveDirection.Right:
                            movement.Horizontal = 5;
                            direction.Heading = "Right";
                            if (!e.HasComponent<AnimationComponent>())
                            {
                                AnimationComponent animation = new AnimationComponent();
                                animation.CurrentAnimation = anim + direction.Heading;
                                e.AddComponent(animation);
                                e.Refresh();
                            }
                            break;
                        case MoveDirection.Idle:
                            
                            break;
                    }
                    previousDirection = currentDirection;
                }
                //                    if (movingUpPressing && !movingDownPressing)
                //                    {
                //                        if (movingUpStart && !movingUpFinish)   //These variables might be named backwards? -JP
                //                        {
                //                            movement.Vertical = -5;
                //                            direction.Heading = "Up";
                //                            if (!movingUpAnimationAdded && !e.HasComponent<AnimationComponent>())
                //                            {
                //                                AnimationComponent animation = new AnimationComponent();
                //                                animation.CurrentAnimation = "Walk" + direction.Heading;
                //                                e.AddComponent(animation);
                //                                e.Refresh();
                //                                movingUpAnimationAdded = true;
                //                            }
                //                        }
                //                    }
                //                    else if (!movingUpPressing)
                //                    {
                //                        if (!movingUpStart && movingUpFinish && movingUpAnimationAdded)
                //                        {
                //                            AnimationComponent animationComponent = e.GetComponent<AnimationComponent>();
                //                            if ((animationComponent != null) && animationComponent.CurrentAnimation.Contains("Up"))
                //                            {
                //                                e.RemoveComponent<AnimationComponent>();
                //                                movingUpAnimationAdded = false;
                //                                sprite.SrcRect = sprite.SpriteSheet.Animation["Idle" + direction.Heading][0]; //I do not know if this is the best way to do this - JP
                //                            }
                //                        }
                //                    }

                //                    if (movingLeftPressing && !movingRightPressing)
                //                    {
                //                        if (movingLeftStart && !movingLeftFinish)   //These variables might be named backwards? -JP
                //                        {
                //                            movement.Horizontal = -5;
                //                            direction.Heading = "Left";
                //                            if (!movingLeftAnimationAdded && !e.HasComponent<AnimationComponent>())
                //                            {
                //                                AnimationComponent animation = new AnimationComponent();
                //                                animation.CurrentAnimation = "Walk" + direction.Heading;
                //                                e.AddComponent(animation);
                //                                e.Refresh();
                //                                movingLeftAnimationAdded = true;
                //                            }
                //                        }
                //                    }
                //                    else if (!movingLeftPressing)
                //                    {
                //                        if (!movingLeftStart && movingLeftFinish && movingLeftAnimationAdded)
                //                        {
                //                            AnimationComponent animationComponent = e.GetComponent<AnimationComponent>();
                //                            if ((animationComponent != null) && animationComponent.CurrentAnimation.Contains("Left"))
                //                            {
                //                                e.RemoveComponent<AnimationComponent>();
                //                                movingLeftAnimationAdded = false;
                //                                sprite.SrcRect = sprite.SpriteSheet.Animation["Idle" + direction.Heading][0]; //I do not know if this is the best way to do this - JP
                //                            }
                //                        }
                //                    }
                //>>>>>>> origin/poetter/melee&animation

                //                    if (movingDownPressing && !movingUpPressing)
                //                    {
                //                        if (movingDownStart && !movingDownFinish)   //These variables might be named backwards? -JP
                //                        {
                //                            movement.Vertical = 5;
                //                            direction.Heading = "Down";

                //                            if (!movingDownAnimationAdded && !e.HasComponent<AnimationComponent>())
                //                            {
                //                                AnimationComponent animation = new AnimationComponent();
                //                                animation.CurrentAnimation = "Walk" + direction.Heading;
                //                                e.AddComponent(animation);
                //                                e.Refresh();
                //                                movingDownAnimationAdded = true;
                //                            }
                //                        }
                //                    }
                //                    else if (!movingDownPressing)
                //                    {
                //                        if (!movingDownStart && movingDownFinish && movingDownAnimationAdded)
                //                        {
                //                            AnimationComponent animationComponent = e.GetComponent<AnimationComponent>();
                //                            if ((animationComponent != null) && animationComponent.CurrentAnimation.Contains("Down"))
                //                            {
                //                                e.RemoveComponent<AnimationComponent>();
                //                                movingDownAnimationAdded = false;
                //                                sprite.SrcRect = sprite.SpriteSheet.Animation["Idle" + direction.Heading][0]; //I do not know if this is the best way to do this - JP
                //                            }
                //                        }
                //                    }

                //<<<<<<< HEAD
                //                if (KeyboardState.IsKeyDown(Keys.Up) || KeyboardState.IsKeyDown(Keys.W))
                //                {
                //                    movement.Vertical = -5;

                //                    direction.Heading = "Up";
                //                }
                //                else if (KeyboardState.IsKeyDown(Keys.Down) || KeyboardState.IsKeyDown(Keys.S))
                //=======
                //                    if (movingRightPressing && !movingLeftPressing)
                //                    {
                //                        if (movingRightStart && !movingRightFinish)   //These variables might be named backwards? -JP
                //                        {
                //                            movement.Horizontal = 5;
                //                            direction.Heading = "Right";
                //                            if (!movingRightAnimationAdded && !e.HasComponent<AnimationComponent>())
                //                            {
                //                                AnimationComponent animation = new AnimationComponent();
                //                                animation.CurrentAnimation = "Walk" + direction.Heading;
                //                                e.AddComponent(animation);
                //                                e.Refresh();
                //                                movingRightAnimationAdded = true;
                //                            }
                //                        }
                //                    }
                //                    else if (!movingRightPressing)
                //                    {
                //                        if (!movingRightStart && movingRightFinish && movingRightAnimationAdded)
                //                        {
                //                            AnimationComponent animationComponent = e.GetComponent<AnimationComponent>();
                //                            if ((animationComponent != null) && animationComponent.CurrentAnimation.Contains("Right"))
                //                            {
                //                                e.RemoveComponent<AnimationComponent>();
                //                                movingRightAnimationAdded = false;
                //                                sprite.SrcRect = sprite.SpriteSheet.Animation["Idle" + direction.Heading][0]; //I do not know if this is the best way to do this - JP
                //                            }
                //                        }
                //                    }
                //                //}

                //                if (!movingUpPressing && !movingDownPressing)
                //>>>>>>> origin/poetter/melee&animation
                //                {
                //    movement.Vertical = 0;
                //}
                //if (!movingLeftPressing && !movingRightPressing)
                //{
                //    movement.Horizontal = 0;
                //}

                //else if (attackingPressing)
                //{
                //    AnimationComponent animation = new AnimationComponent();
                //    animation.CurrentAnimation = "Attack" + direction.Heading;
                //    e.AddComponent(animation);
                //    e.Refresh();
                //}

                //Attacking with the mouse
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
                            if (wc.Ammo > 0)
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

        #region Functions
        //private void MoveLeft()
        //{
        //    movingLeftStart = !movingLeftStart;
        //    movingLeftFinish = !movingLeftFinish;
        //    movingLeftPressing = !movingLeftPressing;
        //}
        //private void MoveRight()
        //{
        //    movingRightStart = !movingRightStart;
        //    movingRightFinish = !movingRightFinish;
        //    movingRightPressing = !movingRightPressing;
        //}
        //private void MoveUp()
        //{
        //    movingUpStart = !movingUpStart;
        //    movingUpFinish = !movingUpFinish;
        //    movingUpPressing = !movingUpPressing;
        //}
        //private void MoveDown()
        //{
        //    movingDownStart = !movingDownStart;
        //    movingDownFinish = !movingDownFinish;
        //    movingDownPressing = !movingDownPressing;
        //}
        //private void Attack()
        //{
        //    attackingStart = !attackingStart;
        //    attackingFinish = !attackingFinish;
        //    attackingPressing = !attackingPressing;
        //}
        #endregion
    }
}
