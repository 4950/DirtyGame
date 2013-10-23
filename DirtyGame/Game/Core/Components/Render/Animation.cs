using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.SGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Core.Components.Render
{
    class Animation : Renderable
    {
        #region Variables
        //Current animation of the sprite
        private string currentAnimation;     

//JARED private Rectangle srcRect; 
//JARED //Stores the sprite sheet of the sprite
//JARED private Texture2D texture;
//JARED private SpriteSheet spriteSheet;
//JARED //Stores the different animations of the sprite with a string tag
//JARED private Dictionary<string, Rectangle[]> sAnimations = new Dictionary<string, Rectangle[]>();
//JARED //Stores the number of frames in a given animation
//JARED private Dictionary<string, int> sFrames = new Dictionary<string, int>();
//JARED //Stores the sprite offsets for each of the animations
//JARED private Dictionary<string, Vector2> sOffsets = new Dictionary<string, Vector2>();
//JARED 
//JARED //Current frame of the animation
//JARED private int currentFrame;
//JARED //Time between each frame of the sprite
//JARED //private double timeBetweenFrames;
//JARED //Time elapsed since last draw
//JARED private double timeElapsed;
       
        #endregion

        #region Constructors
        public Animation()
        {
            
        }
        #endregion

        #region Properties
        public string CurrentAnimation
        {
            get
            {
                return currentAnimation;
            }
            set
            {
                currentAnimation = value;
            }
        }
        #endregion

        #region Helper Methods
        
        #endregion
    }
}
