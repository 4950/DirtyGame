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
using DirtyGame.game.Input;

namespace DirtyGame.game.Core.Systems
{
    class PlayerControlSystem : EntitySystem
    {
        private KeyboardState KeyboardState;
        private bool movingUp, movingDown, movingLeft, movingRight, attack;

        public PlayerControlSystem(InputContext context)
            : base(SystemDescriptions.PlayerControlSystem.Aspect, SystemDescriptions.PlayerControlSystem.Priority)
        {
            context.RegisterHandler(Keys.Left, MoveLeft, MoveLeft);
            context.RegisterHandler(Keys.Right, MoveRight, MoveRight);
            context.RegisterHandler(Keys.Up, MoveUp, MoveUp);
            context.RegisterHandler(Keys.Down, MoveDown, MoveDown);
            context.RegisterHandler(Keys.Space, Attack, Attack);
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            foreach (Entity e in entities)
            {
    //            if (!e.HasComponent<Player>()) continue;

                Spatial spatial = e.GetComponent<Spatial>();
                DirectionComponent direction = e.GetComponent<DirectionComponent>();

                Vector2 translateVector = new Vector2(0,0);

                if (attack)
                {
                    
                }

                if (movingUp)//W
                {
                    translateVector.Y -= 3;
                    direction.Heading = "Up";
                    if (e.HasComponent<Animation>() && !movingDown && !movingLeft && !movingRight)
                    {
                        e.GetComponent<Animation>().CurrentAnimation = "Walk" + direction.Heading;
                    }
                }
                if (movingLeft)//A
                {
                    translateVector.X -= 3;
                    direction.Heading = "Left";
                    if (e.HasComponent<Animation>() && !movingDown && !movingRight && ! movingUp)
                    {
                        e.GetComponent<Animation>().CurrentAnimation = "Walk" + direction.Heading;
                    }
                }
                if (movingDown)//S
                {
                    translateVector.Y += 3;
                    direction.Heading = "Down";
                    if (e.HasComponent<Animation>() && !movingLeft && !movingRight && !movingUp)
                    {
                        e.GetComponent<Animation>().CurrentAnimation = "Walk" + direction.Heading;
                    }
                }
                if (movingRight)//D
                {
                    translateVector.X += 3;
                    direction.Heading = "Right";
                    if (e.HasComponent<Animation>() && !movingDown && !movingLeft && !movingUp)
                    {
                        e.GetComponent<Animation>().CurrentAnimation = "Walk" + direction.Heading;
                    }
                }
                if (!movingLeft && !movingRight && !movingUp && !movingDown)
                {
                    if (e.HasComponent<Animation>())
                    {
                        e.GetComponent<Animation>().CurrentAnimation = "Idle" + direction.Heading;
                    }  
                }

                /*
                KeyboardState = Keyboard.GetState();
                if (KeyboardState.IsKeyDown(Keys.Left))
                {
                    //Arbitrarily chosen number of pixels... speed can easily be added if we want
                    translateVector.X -= 3;

                    direction.Heading = "Left";
                }
                if (KeyboardState.IsKeyDown(Keys.Right))
                {
                    translateVector.X += 3;

                    direction.Heading = "Right";
                }
                if (KeyboardState.IsKeyDown(Keys.Up))
                {
                    translateVector.Y -= 3;

                    direction.Heading = "Up";
                }
                if (KeyboardState.IsKeyDown(Keys.Down))
                {
                    translateVector.Y += 3;

                    direction.Heading = "Down";
                }
                */

         //       if (KeyboardState.IsKeyDown(Keys.Space))
         //       {
         //           if (e.HasComponent<Animation>())
         //           {
         //               e.GetComponent<Animation>().CurrentAnimation = "AttackUp";
         //           }
         //       }
        /*
                if (KeyboardState.IsKeyUp(Keys.Left) && 
                    KeyboardState.IsKeyUp(Keys.Right) && 
                    KeyboardState.IsKeyUp(Keys.Up) && 
                    KeyboardState.IsKeyUp(Keys.Down))
                {
                    if (!direction.Heading.Contains("Idle"))
                    {
                        direction.Heading = "Idle" + direction.Heading;
                    }
                }
        */

                spatial.Translate(translateVector);

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

        private void MoveLeft()
        {
            movingLeft = !movingLeft;
        }

        private void MoveRight()
        {
            movingRight = !movingRight;
        }

        private void MoveUp()
        {
            movingUp = !movingUp;
        }

        private void MoveDown()
        {
            movingDown = !movingDown;
        }
        private void Attack()
        {
            attack = !attack;
        }
    }
}
