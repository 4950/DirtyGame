using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace CleanGame.Game.Core.Systems.Render
{
    class CameraUpdateSystem : EntitySystem
    {
        private Renderer renderer;

        public CameraUpdateSystem(Renderer renderer)
            : base(SystemDescriptions.CameraUpdateSystem.Aspect, SystemDescriptions.CameraUpdateSystem.Priority)
        {
            this.renderer = renderer;
        }

        Vector2 CheckMapBounds(Vector2 pos)
        {
            Vector2 newPos = pos;
            if (pos.X < 0) newPos.X = 0;
            if (pos.Y < 0) newPos.Y = 0;
            if (pos.X > renderer.ActiveMap.getPixelWidth() - renderer.GetViewportWidth())
                newPos.X = renderer.ActiveMap.getPixelWidth() - renderer.GetViewportWidth();
            if (pos.Y > renderer.ActiveMap.getPixelHeight() - renderer.GetViewportHeight())
                newPos.Y = renderer.ActiveMap.getPixelHeight() - renderer.GetViewportHeight();
            return newPos;
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            foreach (Entity e in entities)
            {
                if (!e.HasComponent<PlayerComponent>()) continue;
                Vector2 newPos = e.GetComponent<SpatialComponent>().Position;
                newPos.X -= renderer.GetViewportWidth() / 2;
                newPos.Y -= renderer.GetViewportHeight() / 2;

                //Just so the player is really really really in the center, 
                //but the texture width seems to be much longer than the picture suggests (w=600, h=21)             
                /*
                newPos.X += e.GetComponent<Sprite>().Texture.Width / 2;
                newPos.Y += e.GetComponent<Sprite>().Texture.Height / 2;
                */

                newPos = CheckMapBounds(newPos);
                renderer.ActiveCamera.MoveTo(newPos);
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
