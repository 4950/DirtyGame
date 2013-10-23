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
            sprite.Sprite_Sheet = new SpriteSheet(resourceMgr.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");
            sprite.Sprite_Sheet.CurrentAnimation = animationName;

            Animation animation = new Animation();
      //      animation.CurrentAnimation = "AttackDown";

  /*          //Adding all the animations to the player sprite
            sprite.AddAnimation(12, 0, 0, "Down", 50, 50, new Vector2(0, 0));
            sprite.AddAnimation(1, 0, 0, "IdleDown", 50, 50, new Vector2(0, 0));
            sprite.AddAnimation(12, 50, 0, "Up", 50, 50, new Vector2(0, 0));
            sprite.AddAnimation(1, 50, 0, "IdleUp", 50, 50, new Vector2(0, 0));
            sprite.AddAnimation(8, 100, 0, "Left", 50, 50, new Vector2(0, 0));
            sprite.AddAnimation(1, 100, 0, "IdleLeft", 50, 50, new Vector2(0, 0));
            sprite.AddAnimation(8, 100, 8, "Right", 50, 50, new Vector2(0, 0));
            sprite.AddAnimation(1, 100, 8, "IdleRight", 50, 50, new Vector2(0, 0));
            sprite.AddAnimation(9, 150, 0, "AttackDown", 70, 80, new Vector2(0, 0));
            sprite.AddAnimation(9, 230, 0, "AttackUp", 70, 80, new Vector2(-13, -27));
            sprite.AddAnimation(9, 310, 0, "AttackLeft", 70, 70, new Vector2(-30, -5));
            sprite.AddAnimation(9, 380, 0, "AttackRight", 70, 70, new Vector2(+15, -5));

            //Setting current animation
            sprite.CurrentAnimation = "AttackRight";
*/   

            e.AddComponent(spatial);
            e.AddComponent(sprite);
            e.AddComponent(animation);
            return e;
        }
    }
}
