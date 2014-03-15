using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.Core.Components;
using CleanGame.Game.Core.Components.Render;
using CleanGame.Game.Core.Systems.Util;
using CleanGame.Game.SGraphics;
using CleanGame.Game.SGraphics.Commands;
using CleanGame.Game.SGraphics.Commands.DrawCalls;
using EntityFramework;
using EntityFramework.Systems;
using EntityFramework.Managers;
using Microsoft.Xna.Framework;


namespace CleanGame.Game.Systems
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
                //DirectionComponent direction = e.GetComponent<DirectionComponent>();
                SpatialComponent sp = e.GetComponent<SpatialComponent>();

                if (animation == null) //This is a weird bug when adding and removing the animation component. It might have to do with priorities of the systems
                {
                    continue;
                }

                //Move the sprite to the next frame
                //Adding to the time since last draw
                animation.TimeElapsed += dt;
                //Saving the time between frames
                double timeBetweenFrames = sprite.SpriteSheet.Time[animation.CurrentAnimation] / sprite.SpriteSheet.Animation[animation.CurrentAnimation].Length;
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
                                //Removing the player's melee entity from the world when the animation is finished
                                if (e.HasComponent<MeleeComponent>())
                                {
                                    entitiesToDelete.Add(e);
                                }
                                
                                //Removing the entity's AnimationComponent when the finite animation is finished
                                else
                                {
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

                //Setting the rectangle of the sprite sheet to draw
                sprite.SrcRect = sprite.SpriteSheet.Animation[animation.CurrentAnimation][animation.CurrentFrame];
            }

            foreach (Entity e in entitiesToRemoveAnimationComponent)
            {
                //World.RemoveEntity(e);
                AnimationComponent animation = e.GetComponent<AnimationComponent>();
                if (animation.CurrentAnimation.Contains("BigSlash"))
                {
                    SpriteComponent sprite = e.GetComponent<SpriteComponent>();
                    if(e.GetComponent<MovementComponent>().Velocity == new Vector2 (0,0))
                        animation.CurrentAnimation = "Idle" + e.GetComponent<DirectionComponent>().Heading;
                    else
                        animation.CurrentAnimation = "Walk" + e.GetComponent<DirectionComponent>().Heading;
                    
                    sprite.SrcRect = sprite.SpriteSheet.Animation[animation.CurrentAnimation][animation.CurrentFrame];
                }
                //else
                //    e.RemoveComponent(animation);
                
                                
            }

            foreach (Entity e in entitiesToDelete)
            {
                game.world.DestroyEntity(e);
            }
        }
    }
}
