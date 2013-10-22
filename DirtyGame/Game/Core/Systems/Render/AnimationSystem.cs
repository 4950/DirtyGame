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
                Animation animation = e.GetComponent<Animation>();
                Sprite sprite = e.GetComponent<Sprite>();

                sprite.Sprite_Sheet.NextFrame(dt);
            }
    //        RenderGroup renderGroup = new RenderGroup();
    //        renderGroup.AddCommand(new BeginBatchDraw(renderer.ActiveCamera.Transform));
    //        foreach (Entity e in entities)
    //        {
    //            Spatial spatial = e.GetComponent<Spatial>();
    //            Sprite sprite = e.GetComponent<Sprite>();
                
    //            // create RenderInstance
    //            RenderInstance instance = new RenderInstance();
    //            instance.DrawCall = new BatchDrawSprite(sprite.Texture, spatial.Position, sprite.SrcRect, Color.White);
    ////            instance.DrawCall = new BatchDrawSprite(sprite.Texture, 
    ////                                                    spatial.Position, 
    ////                                                    sprite.Animation[sprite.CurrentAnimation][sprite.CurrentFrame],
    ////                                                    Color.White);
    ////            sprite.NextFrame(dt);
    //            instance.SortKey.SetRenderLayer(sprite.RenderLayer);

    //            renderGroup.AddInstance(instance);
    //        }

    //        renderer.Submit(renderGroup);
        }
    }
}
