using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Render;
using DirtyGame.game.SGraphics;
using EntityFramework;
using EntityFramework.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Core
{
    class EntityFactory
    {
        private EntityManager entityMgr;
        private ResourceManager resourceMgr;

        public EntityFactory(EntityManager em, ResourceManager resourceMgr)
        {
            entityMgr = em;
            this.resourceMgr = resourceMgr;
        }

        public Entity CreateTestEntity()
        {
            Entity e = entityMgr.CreateEntity();
            Spatial spatial = new Spatial();
            spatial.MoveTo(0, 0);

            Sprite sprite = new Sprite();
            sprite.RenderLayer = RenderLayer.BACKGROUND;
            sprite.Texture = resourceMgr.GetResource<Texture2D>("Player");
            sprite.SrcRect = new Rectangle(0, 0, 100, 100);

            e.AddComponent(spatial);
            e.AddComponent(sprite);
            return e;
        }
    }
}
