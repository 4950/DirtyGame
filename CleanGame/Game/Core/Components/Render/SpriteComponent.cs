using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.SGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

namespace CleanGame.Game.Core.Components.Render
{
    public class SpriteComponent : RenderComponent
    {
        #region Variables
        private Rectangle srcRect;        
        //Stores the sprite's sprite sheet and animations
        [XmlIgnoreAttribute]
        private SpriteSheet spriteSheet;
        [XmlIgnoreAttribute]
        private Texture2D sprite;
        public string spriteName;
        public string xmlName;
        
        #endregion

        #region Constructors
        public SpriteComponent()
        {
            Angle = 0;
            Scale = 1;
        }
        #endregion
        public override void DidDeserialize()
        {
            if (xmlName != null && xmlName != "")
                setSpritesheet(spriteName, xmlName, ResourceManager.Instance);
            else
                setSprite(spriteName, ResourceManager.Instance);
        }
        public void setSprite(string spriteName, ResourceManager res)
        {
            this.spriteName = spriteName;
            sprite = res.GetResource<Texture2D>(spriteName);
            SrcRect = sprite.Bounds;
        }
        public void setSpritesheet(string spriteName, string xmlName, ResourceManager res)
        {
            this.spriteName = spriteName;
            this.xmlName = xmlName;
            spriteSheet = res.GetResource<SpriteSheet>(spriteName);
            if (spriteSheet == null)
            {
                spriteSheet = new SpriteSheet(res.GetResource<Texture2D>(spriteName), spriteName, xmlName);
                res.AddResource<SpriteSheet>(spriteSheet, spriteName);
            }
            if (spriteSheet.Animation.ContainsKey("IdleDown"))
                SrcRect = SpriteSheet.Animation["IdleDown"][0];
            else if (spriteSheet.Animation.Count > 0)
                srcRect = spriteSheet.Animation.Values.ElementAt(0)[0];
        }
        #region Properties
        public Rectangle SrcRect
        {
            get
            {
                return srcRect;
            }
            set
            {
                srcRect = value;
            }
        }

        public float Scale { get; set; }
        /// <summary>
        /// Angle in Radians. Sums with Spatial Rotation
        /// </summary>
        public float Angle { get; set; }
        /// <summary>
        /// Defines what part of the texture anchors to position. (0, 0) is top left corner, (1, 1) is bottom right.
        /// </summary>
        public Vector2 AnchorPoint { get; set; }


        /// <summary>
        /// Origin for rotation. (0, 0) is top left corner, (1, 1) is bottom right.
        /// </summary>
        public Vector2 origin { get; set; }

        [XmlIgnoreAttribute]
        public Texture2D Sprite
        {
            get
            {
                return sprite;
            }
        }
        [XmlIgnoreAttribute]
        public SpriteSheet SpriteSheet
        {
            get
            {
                return spriteSheet;
            }
            set
            {
                spriteSheet = value;
            }
        }
        #endregion
    }
}
