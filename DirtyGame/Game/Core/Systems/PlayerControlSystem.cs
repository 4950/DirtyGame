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
        private List<Keys> keysDown = new List<Keys>();

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

            for (int i = 1; i < 9; i++)
            {
                Keys k = (Keys)Enum.Parse(typeof(Keys), "D" + i);
                game.baseContext.RegisterHandler(k, changeWeapon, null);
            }
        }
        private void move(Keys key)
        {
            previousDirection = currentDirection;
            keysDown.Add(key);
            setDirection(key);
        }
        private void setDirection(Keys key)
        {
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
            keysDown.Remove(key);
            previousDirection = currentDirection;
            if (keysDown.Count > 0)
                setDirection(keysDown[keysDown.Count - 1]);
            else
                currentDirection = MoveDirection.Idle;
            /*
            move(key);
            if (previousDirection == currentDirection)
                currentDirection = MoveDirection.Idle;
            else
                currentDirection = previousDirection;*/
        }
        private void changeWeapon(Keys key)
        {
            InventoryComponent ic = game.player.GetComponent<InventoryComponent>();
            var weapons = ic.WeaponList;

            if (key == Keys.Tab)
            {
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
            else
            {
                String k = key.ToString();
                int i = 0;
                if (int.TryParse(k[1].ToString(), out i))
                {
                    if (i <= weapons.Count)
                    {
                        ic.setCurrentWeapon(weapons[i - 1]);
                    }
                }
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
                StatsComponent s = e.GetComponent<StatsComponent>();
                AnimationComponent animationComponent = e.GetComponent<AnimationComponent>();
                AttackingComponent attacking = e.GetComponent<AttackingComponent>();

                if (attacking.isAttacking)
                {
                    int junk = 0;
                    //do nothing
                    //this might have to change to forcing the player to be idle, which means that the player velocity is going need to be set to 0 and idle anim
                    movement.Velocity = new Vector2(0.0f, 0.0f);
                    movement.Vertical = 0.0f;
                    movement.Horizontal = 0.0f;

                    //animationComponent.
                }

                else if (previousDirection != currentDirection)//direction state changed
                {
                    float MoveSpeed = 5 * (s.MoveSpeed / 100);

                    movement.Vertical = 0;
                    movement.Horizontal = 0;
                    
                    if ((animationComponent != null))
                    {
                        e.RemoveComponent(e.GetComponent<AnimationComponent>());
                        e.Refresh();
                        sprite.SrcRect = sprite.SpriteSheet.Animation["Idle" + direction.Heading][0]; //I do not know if this is the best way to do this - JP
                    }
                    string anim = "Walk";
                    switch (currentDirection)
                    {
                        case MoveDirection.Up:
                            movement.Vertical = -MoveSpeed;
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
                            movement.Vertical = MoveSpeed;
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
                            movement.Horizontal = -MoveSpeed;
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
                            movement.Horizontal = MoveSpeed;
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


                //Attacking with the mouse
                prevMS = ms;
                ms = Mouse.GetState();
                if (prevMS == null)
                    prevMS = ms;
                if (ms.RightButton == ButtonState.Pressed && prevMS.RightButton == ButtonState.Released)//right mouse down
                {
                    Entity currentWeaponEntity = e.GetComponent<InventoryComponent>().CurrentWeapon;

                    //Change the Attacking Component of the player to be true when the weapon is a sword
                    //if (currentWeaponEntity.HasComponent<WeaponComponent>() && currentWeaponEntity.GetComponent<WeaponComponent>().Portrait.Equals("sword"))
                    if (currentWeaponEntity.GetComponent<WeaponComponent>().Portrait.Equals("sword"))
                    {
                        e.GetComponent<AttackingComponent>().isAttacking = true;
                    }

                    //game.weaponSystem.FireWeapon(e.GetComponent<InventoryComponent>().CurrentWeapon, e, new Vector2(ms.X, ms.Y) + renderer.ActiveCamera.Position);
                    game.weaponSystem.FireWeapon(currentWeaponEntity, e, new Vector2(ms.X, ms.Y) + renderer.ActiveCamera.Position);
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
