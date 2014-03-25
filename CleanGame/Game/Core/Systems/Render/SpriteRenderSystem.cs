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
using Microsoft.Xna.Framework;


namespace CleanGame.Game.Systems
{
    class SpriteRenderSystem : EntitySystem
    {
        private Renderer renderer;

        public SpriteRenderSystem(Renderer renderer)
            : base(SystemDescriptions.SpriteRenderSystem.Aspect, SystemDescriptions.SpriteRenderSystem.Priority)
        {
            this.renderer = renderer;
        }

        public override void OnEntityAdded(Entity e)
        {           
            // do nothing
        }

        public override void OnEntityRemoved(Entity e)
        {           
            // do nothing
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            RenderGroup renderGroup = new RenderGroup();
            renderGroup.AddCommand(new BeginBatchDraw(renderer.ActiveCamera.Transform));
            foreach (Entity e in entities)
            {
                //Getting the components for this entity
                SpatialComponent spatial = e.GetComponent<SpatialComponent>();
                SpriteComponent sprite = e.GetComponent<SpriteComponent>();

                //calcualte position of sprite using anchorpoint
                Vector2 size = new Vector2(sprite.SrcRect.Width, sprite.SrcRect.Height) * sprite.Scale;
                Vector2 pos = spatial.Position - size * sprite.AnchorPoint;

                // create RenderInstance
               
                if (e.HasComponent<AnimationComponent>())
                {
                   
                    AnimationComponent animation = e.GetComponent<AnimationComponent>();
                    if (size == new Vector2(sprite.SpriteSheet.Animation[animation.CurrentAnimation][animation.CurrentFrame].Width,
                            sprite.SpriteSheet.Animation[animation.CurrentAnimation][animation.CurrentFrame].Height))
                    {
                        pos += sprite.SpriteSheet.Offset[animation.CurrentAnimation];
                    }
                    
                    
                }


               RenderInstance instance = new RenderInstance();     
               if (sprite.SpriteSheet != null)
                        instance.DrawCall = new BatchDrawSprite(sprite.SpriteSheet.SpriteSheetTexture, pos, sprite.SrcRect, Color.White, spatial.Rotation + sprite.Angle, sprite.Scale, sprite.origin * size);//new BatchDrawSprite(sprite.SpriteSheet.SpriteSheetTexture, spatial.Position, sprite.SrcRect, Color.White);
               else if (sprite.Sprite != null)
                        instance.DrawCall = new BatchDrawSprite(sprite.Sprite, pos, sprite.SrcRect, Color.White, spatial.Rotation + sprite.Angle, sprite.Scale, sprite.origin * size);
                    instance.SortKey.SetRenderLayer(sprite.RenderLayer);

               

                renderGroup.AddInstance(instance);
            }

            renderer.Submit(renderGroup);
        }
    }
}
