using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Core.Components.Render
{
    class Sprite : Renderable
    {
        #region Variables
        private Rectangle srcRect;
        //Stores the sprite sheet of the sprite
        private Texture2D texture;
        //Stores the different animations of the sprite with a string tag
        private Dictionary<string, Rectangle[]> sAnimations = new Dictionary<string, Rectangle[]>();
        //Stores the number of frames in a given animation
        private Dictionary<string, int> sFrames = new Dictionary<string, int>();
        //Stores the sprite offsets for each of the animations
        private Dictionary<string, Vector2> sOffsets = new Dictionary<string, Vector2>();
        //Current animation of the sprite
        private string currentAnimation;
        //Current frame of the animation
        private int currentFrame;
        //Time between each frame of the sprite
        //private double timeBetweenFrames;
        //Time elapsed since last draw
        private double timeElapsed;
        
        #endregion

        #region Constructors
        public Sprite()
        {
            
        }
        #endregion

        #region Properties
        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
            }
        }    

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

        public Dictionary<string, Rectangle[]> Animation
        {
            get
            {
                return sAnimations;
            }
            set
            {
                sAnimations = value;
            }
        }

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

        public double TimeBetweenFrames
        {
            get
            {
                return 1f / sFrames[currentAnimation];
            }
        }

        #endregion

        #region Helper Methods
        //Adding a specified animation to the sprite component
        public void AddAnimation(int numFrames,
                                 int yPosition,
                                 int xStartFrame,
                                 string animationName,
                                 int frameWidth,
                                 int frameHeight,
                                 Vector2 frameOffset)
        {
            //Stores the rectangles for the individual frames of an animation
            Rectangle[] tempRectangles = new Rectangle[numFrames];
            //Looping through all the frames of the animation
            for (int i = 0; i < numFrames; i++)
            {
                tempRectangles[i] = new Rectangle((i + xStartFrame) * frameWidth,
                                                  yPosition,
                                                  frameWidth,
                                                  frameHeight);
            }
            //Saving the animation to the animation Dictionary
            sAnimations.Add(animationName, tempRectangles);
            //Saving the animation's offset to the offset Dictionary
            sOffsets.Add(animationName, frameOffset);
            //Saving the animation's number of frames
            sFrames.Add(animationName, numFrames);
        }

        //Move the sprite to the next frame
        public void NextFrame(float deltaTime)//GameTime gameTime
        {
            //Adding to the time since last draw
            //timeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            timeElapsed += deltaTime;
            if (timeElapsed > TimeBetweenFrames)
            {
                timeElapsed -= TimeBetweenFrames;
                //Checking to make sure we are not going over the number of frames
                if (currentFrame < (sFrames[currentAnimation]- 1))
                {
                    currentFrame++;
                }
                //Starting back at frame 0
                else
                {
                    currentFrame = 0;
                }
            }
        }
        #endregion
    }
}
