using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.SGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Core.Components.Render
{
    class Sprite : Renderable
    {
        #region Variables
  //      private Rectangle srcRect;
        
        //Stores the sprite sheet of the sprite
  //      private Texture2D texture;
        
        //Stores the sprite's sprite sheet and animations
        private SpriteSheet spriteSheet;
        
        #endregion

        #region Constructors
        public Sprite()
        {
            
        }
        #endregion

        #region Properties
 //       public Texture2D Texture
 //       {
 //           get
 //           {
 //               return texture;
 //           }
 //           set
 //           {
 //               texture = value;
 //           }
 //       }    

  //      public Rectangle SrcRect
  //      {
  //         get
  //          {
  //              return srcRect;
  //          }
  //          set
  //          {
  //              srcRect = value;
  //          }
  //      }

        public SpriteSheet Sprite_Sheet
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
