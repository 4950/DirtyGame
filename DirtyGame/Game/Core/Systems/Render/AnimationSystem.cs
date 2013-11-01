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
        public AnimationSystem()
            : base(SystemDescriptions.AnimationSystem.Aspect, SystemDescriptions.AnimationSystem.Priority)
        {
        }

        public override void OnEntityAdded(Entity e)
        {           
         
        }

        public override void OnEntityRemoved(Entity e)
        {           
            
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            foreach (Entity e in entities)
            {
                //Getting components for this entity
                Animation animation = e.GetComponent<Animation>();
                Sprite sprite = e.GetComponent<Sprite>();
                DirectionComponent direction = e.GetComponent<DirectionComponent>();

                animation.CurrentAnimation = direction.Heading;

                //Setting the next frame
                sprite.SpriteSheet.NextFrame(animation.CurrentAnimation, dt);
                //Setting the rectangle of the sprite sheet to draw
                sprite.SrcRect = sprite.SpriteSheet.Animation[animation.CurrentAnimation][sprite.SpriteSheet.CurrentFrame];
            }
        }
    }
}
