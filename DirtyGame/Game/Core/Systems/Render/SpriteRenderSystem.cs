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
using Microsoft.Xna.Framework;


namespace DirtyGame.game.Systems
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
                Spatial spatial = e.GetComponent<Spatial>();
                Sprite sprite = e.GetComponent<Sprite>();
                
                //DELETE
               // Animation animation = e.GetComponent<Animation>();
                //DELETE

                // create RenderInstance
                RenderInstance instance = new RenderInstance();
          //      instance.DrawCall = new BatchDrawSprite(sprite.Texture, spatial.Position, sprite.SrcRect, Color.White);
                instance.DrawCall = new BatchDrawSprite(sprite.Sprite_Sheet.Sprite_Sheet, 
                                                        spatial.Position, 
                                                        //Maybe put the currentAnimation string in SpriteSheet class
                                                        sprite.Sprite_Sheet.Animation[sprite.Sprite_Sheet.CurrentAnimation][sprite.Sprite_Sheet.CurrentFrame],
                                                        Color.White);
          //      sprite.NextFrame(dt);
                instance.SortKey.SetRenderLayer(sprite.RenderLayer);

                renderGroup.AddInstance(instance);
            }

            renderer.Submit(renderGroup);
        }
    }
}
