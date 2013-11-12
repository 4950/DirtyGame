using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.SGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Core.Components.Render
{
    public class SpriteComponent : RenderComponent
    {
        #region Variables
        private Rectangle srcRect;        
        //Stores the sprite's sprite sheet and animations
        private SpriteSheet spriteSheet;
        
        #endregion

        #region Constructors
        public SpriteComponent()
        {
            
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
