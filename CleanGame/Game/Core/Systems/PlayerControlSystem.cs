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
using CleanGame.Game.Input;
using CleanGame.Game.Core.Components.Render;

namespace CleanGame.Game.Core.Systems
{
    class PlayerControlSystem : EntitySystem
    {
        private MouseState ms;
        private MouseState prevMS;
        private EntityFactory entityFactory;
        private Renderer renderer;
        private Dirty game;
        private List<Keys> keysDown = new List<Keys>();

        //scroll wheel
        private int prevScrollWheel;

        enum MoveDirection
        {
            Up,
            Down,
            Left,
            Right,
            Idle
        };

        private MoveDirection currentDirection = MoveDirection.Idle;
        private bool directionChanged;


        public PlayerControlSystem(EntityFactory ef, Renderer renderer, Dirty game)
            : base(SystemDescriptions.PlayerControlSystem.Aspect, SystemDescriptions.PlayerControlSystem.Priority)
        {
            this.entityFactory = ef;
            this.renderer = renderer;
            this.game = game;

            // arrow keys, useful for mouse + scroll wheel
            game.baseContext.RegisterHandler(Keys.Left, move, idle);
            game.baseContext.RegisterHandler(Keys.Right, move, idle);
            game.baseContext.RegisterHandler(Keys.Up, move, idle);
            game.baseContext.RegisterHandler(Keys.Down, move, idle);

            // scroll wheel
            //if (ms.ScrollWheelValue < prevScrollWheel)
            //move(Keys.E);
            //else if (ms.ScrollWheelValue > prevScrollWheel)
            //  move(Keys.Q);
            //prevScrollWheel = ms.ScrollWheelValue;


            // left handed controls
            // movement
            game.baseContext.RegisterHandler(Keys.U, move, idle);
            game.baseContext.RegisterHandler(Keys.H, move, idle);
            game.baseContext.RegisterHandler(Keys.J, move, idle);
            game.baseContext.RegisterHandler(Keys.K, move, idle);

            // weapon cycling
            game.baseContext.RegisterHandler(Keys.T, changeWeapon, null);
            game.baseContext.RegisterHandler(Keys.Y, changeWeapon, null);
            game.baseContext.RegisterHandler(Keys.I, changeWeapon, null);

            // right handed controls
            // movement
            game.baseContext.RegisterHandler(Keys.W, move, idle);
            game.baseContext.RegisterHandler(Keys.A, move, idle);
            game.baseContext.RegisterHandler(Keys.S, move, idle);
            game.baseContext.RegisterHandler(Keys.D, move, idle);

            // weapon cycling
            game.baseContext.RegisterHandler(Keys.Tab, changeWeapon, null);
            game.baseContext.RegisterHandler(Keys.Q, changeWeapon, null);
            game.baseContext.RegisterHandler(Keys.E, changeWeapon, null);

            // weapon quick access
            for (int i = 0; i <= 9; i++)
            {
                // terrible hack for only two weapon prune
                if (i == 1 || i == 2 || i == 6 || i == 7)
                {
                    Keys k = (Keys)Enum.Parse(typeof(Keys), "D" + i);
                    game.baseContext.RegisterHandler(k, changeWeapon, null);
                }
            }

            //Weapon fire
            game.baseContext.RegisterHandler(Keys.Space, fire, null);

        }
        private void fire(Keys key)
        {
            game.weaponSystem.FireWeapon(game.player.GetComponent<InventoryComponent>().CurrentWeapon, game.player, new Vector2(ms.X, ms.Y) + renderer.ActiveCamera.Position);
        }
        private void move(Keys key)
        {
            keysDown.Remove(key);
            keysDown.Add(key);
            setDirection(key);
        }
        private void setDirection(Keys key)
        {
            switch (key)
            {
                case Keys.U:
                case Keys.Up:
                case Keys.W:
                    currentDirection = MoveDirection.Up;
                    break;
                case Keys.J:
                case Keys.Down:
                case Keys.S:
                    currentDirection = MoveDirection.Down;
                    break;
                case Keys.H:
                case Keys.Left:
                case Keys.A:
                    currentDirection = MoveDirection.Left;
                    break;
                case Keys.K:
                case Keys.Right:
                case Keys.D:
                    currentDirection = MoveDirection.Right;
                    break;
                default:
                    currentDirection = MoveDirection.Idle;
                    break;
            }
            directionChanged = true;
        }
        private void idle(Keys key)
        {
            keysDown.Remove(key);
            if (keysDown.Count > 0)
                setDirection(keysDown[keysDown.Count - 1]);
            else
            {
                currentDirection = MoveDirection.Idle;
                directionChanged = true;
            }
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

            // another cheap hack to "disable" additional weapons
            int weaponsCount = 2; // should be weapons.Count

            if (ic.CurrentWeapon == null)
            {
                if (weapons.Count > 0)
                    ic.setCurrentWeapon(weapons[0]);
            }

            int i = weapons.IndexOf(ic.CurrentWeapon);
            switch (key)
            {
                case Keys.T:
                case Keys.I:
                case Keys.E:
                case Keys.Tab:
                    i++;
                    if (i >= weaponsCount)
                        i = 0;

                    break;
                case Keys.Y:
                case Keys.Q:
                    i--;
                    if (i < 0)
                        i = weaponsCount - 1;
                    break;
                default:
                    if (int.TryParse(key.ToString()[1].ToString(), out i))
                    {
                        i--;
                        if (i >= weapons.Count)
                        {
                            i = i - weapons.Count;
                        }

                        if (i < 0)
                            i = weapons.Count - 1;

                    }
                    break;

            }

            ic.setCurrentWeapon(weapons[i]);

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

                //Debugging code
            //    String debugString = "" + spatial.Position;
            //    Console.Out.WriteLine(debugString);

                int monsterX = (int)((e.GetComponent<SpatialComponent>().Position.X) / 32);
                int monsterY = (int)((e.GetComponent<SpatialComponent>().Position.Y + 32) / 32);

                if (directionChanged)//direction state changed
                {
                    directionChanged = false;
                    float MoveSpeed = 5 * (s.MoveSpeed / 100);

                    movement.Vertical = 0;
                    movement.Horizontal = 0;
                    AnimationComponent animationComponent = e.GetComponent<AnimationComponent>();
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
                            break;
                        case MoveDirection.Down:
                            movement.Vertical = MoveSpeed;
                            direction.Heading = "Down";
                            break;
                        case MoveDirection.Left:
                            movement.Horizontal = -MoveSpeed;
                            direction.Heading = "Left";
                            break;
                        case MoveDirection.Right:
                            movement.Horizontal = MoveSpeed;
                            direction.Heading = "Right";
                            break;
                    }
                    if (currentDirection != MoveDirection.Idle)
                    {
                        if (!e.HasComponent<AnimationComponent>())
                        {
                            AnimationComponent animation = new AnimationComponent();
                            animation.CurrentAnimation = anim + direction.Heading;
                            e.AddComponent(animation);
                            e.Refresh();
                        }
                    }
                }

                //int mapWidth = renderer.ActiveMap.getPixelWidth();
                //int mapHeight = renderer.ActiveMap.getPixelHeight();

                //calcualte position of sprite using anchorpoint
                //Vector2 size = new Vector2(sprite.SrcRect.Width, sprite.SrcRect.Height) * sprite.Scale;
                //Vector2 pos = spatial.Position - size * sprite.AnchorPoint;

                // This is kinda hacked. The first run, this works like it should. 
                // After a "Quit to Menu", there is some other vodoo system that does a similar thing, but not as clean (aka drifting health bars).
                //while (spatial.Position.X >= mapWidth - 64)
                //{
                //    spatial.Position = new Vector2(mapWidth - 65, spatial.Position.Y);
                //    movement.Horizontal = -1;
                //    directionChanged = true;
                //}

                //while (spatial.Position.X <= 0)
                //{
                //    spatial.Position = new Vector2(1, spatial.Position.Y);
                //    movement.Horizontal = 1;
                //    directionChanged = true;
                //}

                //while (spatial.Position.Y <= 0)
                //{
                //    spatial.Position = new Vector2(spatial.Position.X, 1);
                //    movement.Vertical = 1;
                //    directionChanged = true;
                //}

                //while (spatial.Position.Y >= mapHeight - 64)
                //{
                //    spatial.Position = new Vector2(spatial.Position.X, mapHeight - 65);
                //    movement.Vertical = -1;
                //    directionChanged = true;
                //}

                //System.Diagnostics.Debug.Write("monsterX: " + monsterX + " monsterY: " + monsterY + "\n");
                //System.Diagnostics.Debug.Write("mapHeight: " + mapHeight + "playerY: " + spatial.Position.Y);
                //System.Diagnostics.Debug.Write("\n");

                //switch (currentDirection)
                //{
                //    case MoveDirection.Left:
                //        if (collMap[Math.Max(monsterX - 1, 0), monsterY])
                //        {
                //            movement.Horizontal = 0;
                //        }
                //        break;
                //    case MoveDirection.Right:
                //        if (collMap[Math.Min(monsterX + 1, mapHeight / 32 - 1), monsterY])
                //        {
                //            movement.Horizontal = 0;
                //        }
                //        break;
                //    case MoveDirection.Down:
                //        if (collMap[monsterX, Math.Min(monsterY + 1, mapWidth / 32 - 1)])
                //        {
                //            movement.Vertical = 0;
                //        }
                //        break;
                //    case MoveDirection.Up:
                //        if (collMap[monsterX, Math.Max(monsterY - 1, 0)])
                //        {
                //            movement.Vertical = 0;
                //        }
                //        break;
                //}


                //Attacking with the mouse
                prevMS = ms;
                ms = Mouse.GetState();
                if (prevMS == null)
                    prevMS = ms;
                if ((ms.RightButton == ButtonState.Pressed && prevMS.RightButton == ButtonState.Released) || (ms.LeftButton == ButtonState.Pressed && prevMS.LeftButton == ButtonState.Released))//right mouse down or left mouse
                {
                    fire(Keys.None);

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
