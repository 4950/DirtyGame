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
        private Dirty game;

        public AnimationSystem(Dirty game)
            : base(SystemDescriptions.AnimationSystem.Aspect, SystemDescriptions.AnimationSystem.Priority)
        {
            this.game = game;
        }

        public override void OnEntityAdded(Entity e)
        {           
         
        }

        public override void OnEntityRemoved(Entity e)
        {           
            
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            List<Entity> entitiesToRemoveAnimationComponent = new List<Entity>();
            List<Entity> entitiesToDelete = new List<Entity>();

            foreach (Entity e in entities)
            {
                //Getting components for this entity
                AnimationComponent animation = e.GetComponent<AnimationComponent>();
                SpriteComponent sprite = e.GetComponent<SpriteComponent>();
                DirectionComponent direction = e.GetComponent<DirectionComponent>();
                SpatialComponent sp = e.GetComponent<SpatialComponent>();

                //animation.CurrentAnimation = direction.Heading;

                if (animation == null) //This is a weird bug when adding and removing the animation component. It might have to do with priorities of the systems
                {
                    continue;
                }

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
                                if (e.GetComponent<NameComponent>().Name.Equals("PLAYER MELEE"))
                                {
                                    entitiesToDelete.Add(e);
                                }
                                else
                                {
                                    //animation.CurrentAnimation = "Idle" + direction.Heading;

                                    entitiesToRemoveAnimationComponent.Add(e);
                                }
                            }
                        }
                    }
                    //Starting back at frame 0
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

                //if (sp.isMoving)
                //{
                //    //Move the sprite to the next frame
                //    //Adding to the time since last draw
                //    animation.TimeElapsed += dt;
                //    //Saving the time between frames
                //    double timeBetweenFrames = 1.0f / sprite.SpriteSheet.Animation[animation.CurrentAnimation].Length;
                //    //Check to see if enough time has passed to render the next frame
                //    if (animation.TimeElapsed > timeBetweenFrames)
                //    {
                //        //Subtracting the time to get ready for the next frame
                //        animation.TimeElapsed -= timeBetweenFrames;
                //        //Checking to make sure we are not going over the number of frames
                //        if (animation.CurrentFrame < (sprite.SpriteSheet.Animation[animation.CurrentAnimation].Length - 1))
                //        {
                //            animation.CurrentFrame++;
                //        }
                //        //Starting back at frame 0
                //        else
                //        {
                //            animation.CurrentFrame = 0;
                //        }
                //    }
                //}
                //else
                //    animation.CurrentFrame = 0;

                if(e.HasComponent<MeleeComponent>() ){

                    }

                //Setting the rectangle of the sprite sheet to draw
                sprite.SrcRect = sprite.SpriteSheet.Animation[animation.CurrentAnimation][animation.CurrentFrame];
            }

            foreach (Entity e in entitiesToRemoveAnimationComponent)
            {
                //World.RemoveEntity(e);
                e.RemoveComponent<AnimationComponent>();
            }

            foreach (Entity e in entitiesToDelete)
            {
                game.world.RemoveEntity(e);
            }
        }
    }
}
