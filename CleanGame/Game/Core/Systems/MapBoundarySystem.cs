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

namespace CleanGame.Game.Core.Systems
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
                Vector2 pos = e.GetComponent<SpatialComponent>().Position;
                if (pos.X < 0) pos.X = 0;
                if (pos.Y < 0) pos.Y = 0;
                //Player width is a bit off and the player can't go all the way to the end of the map
                if (pos.X > renderer.ActiveMap.getPixelWidth() - e.GetComponent<SpriteComponent>().SrcRect.Width)
                    pos.X = renderer.ActiveMap.getPixelWidth() - e.GetComponent<SpriteComponent>().SrcRect.Width;
                if (pos.Y > renderer.ActiveMap.getPixelHeight() - e.GetComponent<SpriteComponent>().SrcRect.Height)
                    pos.Y = renderer.ActiveMap.getPixelHeight() - e.GetComponent<SpriteComponent>().SrcRect.Height;
                e.GetComponent<SpatialComponent>().Position = pos;
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
