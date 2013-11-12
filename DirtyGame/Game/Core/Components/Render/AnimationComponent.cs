using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.SGraphics;
using EntityFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Core.Components
{
    class AnimationComponent : Component
    {
        #region Variables
        //Current animation of the sprite
        private string currentAnimation = "";
        //Time elapsed since last draw
        private double timeElapsed;
        //Current frame of the animation
        private int currentFrame;
        #endregion

        #region Constructors
        public AnimationComponent()
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
                if (!currentAnimation.Equals(value))
                {
                    currentFrame = 0;
                }
                currentAnimation = value;
            }
        }
        public double TimeElapsed
        {
            get
            {
                return timeElapsed;
            }
            set
            {
                timeElapsed = value;
            }
        }
        public int CurrentFrame
        {
            get
            {
                return currentFrame;
            }
            set
            {
                currentFrame = value;
            }
        }
        #endregion

        #region Helper Methods
        
        #endregion
    }
}
