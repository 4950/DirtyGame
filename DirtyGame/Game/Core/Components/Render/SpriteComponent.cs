using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.SGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

namespace DirtyGame.game.Core.Components.Render
{
    public class SpriteComponent : RenderComponent
    {
        #region Variables
        private Rectangle srcRect;        
        //Stores the sprite's sprite sheet and animations
        [XmlIgnoreAttribute]
        private SpriteSheet spriteSheet;
        [XmlIgnoreAttribute]
        public Texture2D sprite;
        
        #endregion

        #region Constructors
        public SpriteComponent()
        {
            Angle = 0;
            Scale = 1;
        }
        #endregion

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
        public float Angle { get; set; }

        /// <summary>
        /// Draw origin of the sprite. (0, 0) is top left corner, (1, 1) is bottom right.
        /// </summary>
        public Vector2 origin { get; set; }

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
