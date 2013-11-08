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
using DirtyGame.game.Core.Components.Render;

namespace DirtyGame.game.Core.Systems
{
    class PlayerControlSystem : EntitySystem
    {
        private KeyboardState KeyboardState;
        private bool movingUpStart = false;
        private bool movingDownStart = false;
        private bool movingLeftStart = false;
        private bool movingRightStart = false;
        private bool attackingStart = false;

        private bool movingUpFinish = true;
        private bool movingDownFinish = true;
        private bool movingLeftFinish = true;
        private bool movingRightFinish = true;
        private bool attackingFinish = true;

        private bool movingUpPressing = false;
        private bool movingDownPressing = false;
        private bool movingLeftPressing = false;
        private bool movingRightPressing = false;
        private bool attackingPressing = false;

        private bool movingUpAnimationAdded = false;
        private bool movingDownAnimationAdded = false;
        private bool movingLeftAnimationAdded = false;
        private bool movingRightAnimationAdded = false;
        private bool attackingAnimationAdded = false; 

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
                Sprite sprite = e.GetComponent<Sprite>();
                Vector2 translateVector = new Vector2(0,0);

           //     if (!e.HasComponent<Animation>())
           //     {
   //             Animation animation = e.GetComponent<Animation>(); //Eugene is going to kill me for putting this in here - JP
           //     }


                if (!attackingPressing)   //!attackingStart)
                {
                    if (movingUpPressing)
                    {
                        if (movingUpStart && !movingUpFinish)   //These variables might be named backwards? -JP
                        {
                            //// How to implement the adding and removing AnimationComponent
                            // Entity will add the animation component when the button is pressed
                            // 
                            // Entity will remove the animation component when the button is released
                            ////
                            translateVector.Y -= 3;
                            direction.Heading = "Up";

                            if (!movingUpAnimationAdded && !e.HasComponent<AnimationComponent>())
                            {
                                //   Sprite sprite = e.GetComponent<Sprite>();
                                // sprite.SrcRect = new Rectangle(0, 0, 50, 50);
                                AnimationComponent animation = new AnimationComponent();
                                animation.CurrentAnimation = "Walk" + direction.Heading;
                                e.AddComponent(animation);
                                e.Refresh();
                                movingUpAnimationAdded = true;
                            }
                        }
                    }
                    else if (!movingUpPressing)
                    {
                        if (!movingUpStart && movingUpFinish && movingUpAnimationAdded)
                        {
                            AnimationComponent animationComponent = e.GetComponent<AnimationComponent>();
                            if ((animationComponent != null) && animationComponent.CurrentAnimation.Contains("Up"))
                            {
                                //entitiesToRemoveAnimationComponent.Add(e);
                                e.RemoveComponent<AnimationComponent>();
                                movingUpAnimationAdded = false;
                                sprite.SrcRect = sprite.SpriteSheet.Animation["Idle" + direction.Heading][0]; //I do not know if this is the best way to do this - JP
                            }
                        }
                    }

                    if (movingLeftPressing)
                    {
                        if (movingLeftStart && !movingLeftFinish)   //These variables might be named backwards? -JP
                        {
                            translateVector.X -= 3;
                            direction.Heading = "Left";
                            if (!movingLeftAnimationAdded && !e.HasComponent<AnimationComponent>())
                            {
                                AnimationComponent animation = new AnimationComponent();
                                animation.CurrentAnimation = "Walk" + direction.Heading;
                                e.AddComponent(animation);
                                e.Refresh();
                                movingLeftAnimationAdded = true;
                            }
                        }
                    }
                    else if (!movingLeftPressing)
                    {
                        if (!movingLeftStart && movingLeftFinish && movingLeftAnimationAdded)
                        {
                            AnimationComponent animationComponent = e.GetComponent<AnimationComponent>();
                            if ((animationComponent != null) && animationComponent.CurrentAnimation.Contains("Left"))
                            {
                                e.RemoveComponent<AnimationComponent>();
                                movingLeftAnimationAdded = false;
                                sprite.SrcRect = sprite.SpriteSheet.Animation["Idle" + direction.Heading][0]; //I do not know if this is the best way to do this - JP
                            }
                        }
                    }

                    if (movingDownPressing)
                    {
                        if (movingDownStart && !movingDownFinish)   //These variables might be named backwards? -JP
                        {
                            //// How to implement the adding and removing AnimationComponent
                            // Entity will add the animation component when the button is pressed
                            // 
                            // Entity will remove the animation component when the button is released
                            ////
                            translateVector.Y += 3;
                            direction.Heading = "Down";

                            if (!movingDownAnimationAdded && !e.HasComponent<AnimationComponent>())
                            {
                                //   Sprite sprite = e.GetComponent<Sprite>();
                                // sprite.SrcRect = new Rectangle(0, 0, 50, 50);
                                AnimationComponent animation = new AnimationComponent();
                                animation.CurrentAnimation = "Walk" + direction.Heading;
                                e.AddComponent(animation);
                                e.Refresh();
                                movingDownAnimationAdded = true;
                            }
                        }
                    }
                    else if (!movingDownPressing)
                    {
                        if (!movingDownStart && movingDownFinish && movingDownAnimationAdded)
                        {
                            AnimationComponent animationComponent = e.GetComponent<AnimationComponent>();
                            if ((animationComponent != null) && animationComponent.CurrentAnimation.Contains("Down"))
                            {
                                //entitiesToRemoveAnimationComponent.Add(e);
                                e.RemoveComponent<AnimationComponent>();
                                movingDownAnimationAdded = false;
                                sprite.SrcRect = sprite.SpriteSheet.Animation["Idle" + direction.Heading][0]; //I do not know if this is the best way to do this - JP
                            }
                        }
                    }

                    if (movingRightPressing)
                    {
                        if (movingRightStart && !movingRightFinish)   //These variables might be named backwards? -JP
                        {
                            //// How to implement the adding and removing AnimationComponent
                            // Entity will add the animation component when the button is pressed
                            // 
                            // Entity will remove the animation component when the button is released
                            ////
                            translateVector.X += 3;
                            direction.Heading = "Right";

                            if (!movingRightAnimationAdded && !e.HasComponent<AnimationComponent>())
                            {
                                //   Sprite sprite = e.GetComponent<Sprite>();
                                // sprite.SrcRect = new Rectangle(0, 0, 50, 50);
                                AnimationComponent animation = new AnimationComponent();
                                animation.CurrentAnimation = "Walk" + direction.Heading;
                                e.AddComponent(animation);
                                e.Refresh();
                                movingRightAnimationAdded = true;
                            }
                        }
                    }
                    else if (!movingRightPressing)
                    {
                        if (!movingRightStart && movingRightFinish && movingRightAnimationAdded)
                        {
                            AnimationComponent animationComponent = e.GetComponent<AnimationComponent>();
                            if ((animationComponent != null) && animationComponent.CurrentAnimation.Contains("Right"))
                            {
                                //entitiesToRemoveAnimationComponent.Add(e);
                                e.RemoveComponent<AnimationComponent>();
                                movingRightAnimationAdded = false;
                                sprite.SrcRect = sprite.SpriteSheet.Animation["Idle" + direction.Heading][0]; //I do not know if this is the best way to do this - JP
                            }
                        }
                    }
                    spatial.Translate(translateVector);
                }
                else if (attackingPressing)
                {
                    AnimationComponent animation = new AnimationComponent();
                    animation.CurrentAnimation = "Attack" + direction.Heading;
                    e.AddComponent(animation);
                    e.Refresh();
                }
                //if (attackingStart)
                //{
                //    //Creating an Animation component
                //    AnimationComponent animation = new AnimationComponent();
                //    //Changing the animation with the string property
                //    animation.CurrentAnimation = "Attack" + direction.Heading;


                //    //if (e.HasComponent<AnimationComponent>() && !e.GetComponent<AnimationComponent>().CurrentAnimation.Contains("Attack"))
                //    //{
                //    //    e.GetComponent<AnimationComponent>().CurrentAnimation = "Attack" + direction.Heading;
                //    //}
                //}
                //if (attackingFinish)
                //{
                //  //  entitiesToRemoveAnimationComponent.Add(e);
                //}
      //          else if (!attacking)
      //          {
      //              if (e.HasComponent<Animation>() && !e.GetComponent<Animation>().CurrentAnimation.Contains("Attack"))
      //              {
      //                  e.GetComponent<Animation>().CurrentAnimation = "Attack" + direction.Heading;
      //              }
      //          }
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
            movingLeftStart = !movingLeftStart;
            movingLeftFinish = !movingLeftFinish;
            movingLeftPressing = !movingLeftPressing;
        }
        private void MoveRight()
        {
            movingRightStart = !movingRightStart;
            movingRightFinish = !movingRightFinish;
            movingRightPressing = !movingRightPressing;
        }
        private void MoveUp()
        {
            movingUpStart = !movingUpStart;
            movingUpFinish = !movingUpFinish;
            movingUpPressing = !movingUpPressing;
        }
        private void MoveDown()
        {
            movingDownStart = !movingDownStart;
            movingDownFinish = !movingDownFinish;
            movingDownPressing = !movingDownPressing;
        }
        private void Attack()
        {
            attackingStart = !attackingStart;
            attackingFinish = !attackingFinish;
            attackingPressing = !attackingPressing;
        }
    }
}
