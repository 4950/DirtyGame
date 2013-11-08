using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Render;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.SGraphics;
using DirtyGame.game.SGraphics.Commands;
using DirtyGame.game.SGraphics.Commands.DrawCalls;
using EntityFramework;
using EntityFramework.Systems;
using EntityFramework.Managers;
using Microsoft.Xna.Framework;


namespace DirtyGame.game.Systems
{
    class AnimationSystem : EntitySystem
    {
        #region Variables
        //private bool startedFiniteAnimation;
        //private bool finishedFiniteAnimation;
        #endregion

        public AnimationSystem()
            : base(SystemDescriptions.AnimationSystem.Aspect, SystemDescriptions.AnimationSystem.Priority)
        {
          //  startedFiniteAnimation = false;
          //  finishedFiniteAnimation = true;
        }

        public override void OnEntityAdded(Entity e)
        {
            bool b = true;
        }

        public override void OnEntityRemoved(Entity e)
        {           
            
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            foreach (Entity e in entities)
            {
                //Getting components for this entity
                AnimationComponent animation = e.GetComponent<AnimationComponent>();
                Sprite sprite = e.GetComponent<Sprite>();
                DirectionComponent direction = e.GetComponent<DirectionComponent>();

                if (animation == null)
                {
                    continue;
                }

         //       animation.CurrentAnimation = direction.Heading;  //do not understand why this is here

                //Move the sprite to the next frame
                //Adding to the time since last draw
                animation.TimeElapsed += dt;
                //Saving the time between frames
                double timeBetweenFrames = 1.0f / sprite.SpriteSheet.Animation[animation.CurrentAnimation].Length;
                //Check to see if enough time has passed to render the next frame
                if (animation.TimeElapsed > timeBetweenFrames)
                {
                    //Subtracting the time to get ready for the next frame
                    animation.TimeElapsed -= timeBetweenFrames;

                    if (sprite.SpriteSheet.Finite[animation.CurrentAnimation] || animation.StartedFiniteAnimation)
                    {
                        if (!animation.StartedFiniteAnimation && animation.FinishedFiniteAnimation)
                        {
                            animation.StartedFiniteAnimation = true;
                            animation.FinishedFiniteAnimation = false;

                            //Checking to make sure we are not going over the number of frames
                            if (animation.CurrentFrame < (sprite.SpriteSheet.Animation[animation.CurrentAnimation].Length - 1))
                            {
                                animation.CurrentFrame++;
                            }
                        }
                        else if (animation.StartedFiniteAnimation && !animation.FinishedFiniteAnimation)
                        {
                            if (animation.CurrentFrame < (sprite.SpriteSheet.Animation[animation.CurrentAnimation].Length - 1))
                            {
                                animation.CurrentFrame++;
                            }
                            else
                            {
                                animation.StartedFiniteAnimation = false;
                                animation.FinishedFiniteAnimation = true;
                                //animation.CurrentAnimation = animation.CurrentAnimation.Remove(0, 6); //this needs to change the currentAnimation to just the direction
                                //animation.CurrentAnimation = "Down";
                                animation.CurrentAnimation = "Idle" + direction.Heading;
                            }
                        }
                    }
                    else
                    {
                        //Checking to make sure we are not going over the number of frames
                        if (animation.CurrentFrame < (sprite.SpriteSheet.Animation[animation.CurrentAnimation].Length - 1))
                        {
                            animation.CurrentFrame++;
                        }
                        //Starting back at frame 0
                        else
                        {
                            animation.CurrentFrame = 0;
                        }
                    }
                }

                //Setting the rectangle of the sprite sheet to draw
                sprite.SrcRect = sprite.SpriteSheet.Animation[animation.CurrentAnimation][animation.CurrentFrame];

                //Modifying the Spatial component to have the correct boundary box and offset
                if (e.HasComponent<Spatial>())
                {
                    Spatial spatial = e.GetComponent<Spatial>();
                    spatial.BoundaryBox = new Vector2(sprite.SpriteSheet.Animation[animation.CurrentAnimation][animation.CurrentFrame].Width,
                                                      sprite.SpriteSheet.Animation[animation.CurrentAnimation][animation.CurrentFrame].Height);
                    spatial.Offset = sprite.SpriteSheet.Offset[animation.CurrentAnimation];
                }
            }
        }
    }
}
