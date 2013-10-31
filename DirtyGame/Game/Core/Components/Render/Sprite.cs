using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.SGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Core.Components.Render
{
    public class Sprite : Renderable
    {
        #region Variables
        private Rectangle srcRect;

//JARED //Stores the sprite sheet of the sprite
//JARED private Texture2D texture;
        
        //Stores the sprite's sprite sheet and animations
        private SpriteSheet spriteSheet;
        
        #endregion

        #region Constructors
        public Sprite()
        {
            
        }
        #endregion

        #region Properties
//JARED public Texture2D Texture
//JARED {
//JARED     get
//JARED     {
//JARED         return texture;
//JARED     }
//JARED     set
//JARED     {
//JARED         texture = value;
//JARED     }
//JARED }    

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
