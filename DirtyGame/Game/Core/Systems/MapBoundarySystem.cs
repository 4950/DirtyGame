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

namespace DirtyGame.game.Core.Systems
{
    class MapBoundarySystem : EntitySystem
    {
        private Renderer renderer;

        public MapBoundarySystem(Renderer renderer)
            : base(SystemDescriptions.MapBoundrySystem.Aspect, SystemDescriptions.MapBoundrySystem.Priority)
        {
            this.renderer = renderer;
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            foreach (Entity e in entities)
            {
                Vector2 pos = e.GetComponent<Spatial>().Position;
                if (pos.X < 0) pos.X = 0;
                if (pos.Y < 0) pos.Y = 0;
                //Player width is a bit off and the player can't go all the way to the end of the map
                if (pos.X > renderer.ActiveMap.getPixelWidth() - e.GetComponent<Sprite>().SpriteSheet.SpriteSheetTexture.Width)
                    pos.X = renderer.ActiveMap.getPixelWidth() - e.GetComponent<Sprite>().SpriteSheet.SpriteSheetTexture.Width;
                if (pos.Y > renderer.ActiveMap.getPixelHeight() - e.GetComponent<Sprite>().SpriteSheet.SpriteSheetTexture.Height)
                    pos.Y = renderer.ActiveMap.getPixelHeight() - e.GetComponent<Sprite>().SpriteSheet.SpriteSheetTexture.Height;
                e.GetComponent<Spatial>().Position = pos;
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
    }
    
}
