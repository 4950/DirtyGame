using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.SGraphics;
using EntityFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CleanGame.Game.Core.Components
{
    public class AnimationComponent : Component
    {
        #region Variables
        //Current animation of the sprite
        private string currentAnimation = "";
        //Time elapsed since last draw
        private double timeElapsed;
        //Current frame of the animation
        private int currentFrame;
        //Used to figure out if the current finite animation is over
        private bool startedFiniteAnimation = false;
        private bool finishedFiniteAnimation = true;
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
                currentFrame = 0;
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
        public bool StartedFiniteAnimation
        {
            get
            {
                return startedFiniteAnimation;
            }
            set
            {
                startedFiniteAnimation = value;
            }
        }
        public bool FinishedFiniteAnimation
        {
            get
            {
                return finishedFiniteAnimation;
            }
            set
            {
                finishedFiniteAnimation = value;
            }
        }
        #endregion

        #region Helper Methods
        
        #endregion
    }
}
