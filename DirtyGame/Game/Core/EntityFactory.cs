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
            return CreateTestEntity(new Vector2(0.0f, 0.0f), "Down");
        }

        public Entity CreateTestEntity(Vector2 entityPosition, string animationName)
        {
            Entity e = entityMgr.CreateEntity();
            Spatial spatial = new Spatial();
            //spatial.MoveTo(0, 0);
            spatial.MoveTo(entityPosition);

            Sprite sprite = new Sprite();
            sprite.RenderLayer = RenderLayer.BACKGROUND;
      //      sprite.Texture = resourceMgr.GetResource<Texture2D>("playerSheet");
      //      sprite.SrcRect = new Rectangle(0, 0, 50, 50);
            sprite.SpriteSheet = new SpriteSheet(resourceMgr.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");
//JARED     sprite.Sprite_Sheet.CurrentAnimation = animationName;

            //Creating an Animation component
            Animation animation = new Animation();
            //Changing the animation with the string property
            animation.CurrentAnimation = animationName;

            e.AddComponent(spatial);
            e.AddComponent(sprite);
            e.AddComponent(animation);
            return e;
        }
    }
}
