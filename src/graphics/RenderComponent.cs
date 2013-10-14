using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShittyPrototype.src.core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShittyPrototype.src.graphics
{
    class RenderComponent : IComponent
    {
        //Sprite Texture
        public Texture2D texture;
        public Rectangle rectangle;

        //Number of Frames in the Sprite Strip
        public int numberOfFrames;
        //Frame Rectangles
        public Rectangle[] spriteRectangles; //This will have to change to a dictionary for all fo the sprites different animations
        //Start Frame for the animation
        public int startFrame;
        //Current Frame, default is the first frame
        public int currentFrame = 0;
        //Frame Width
        public int frameWidth;
        //Frame Height
        public int frameHeight;
        //Time Between Each Frame of the Sprite
        public double timeBetweenFrames;

        public TimeSpan timeOfLastDraw;
        public TimeSpan timeBetweenDraw;
        public double timeElapsed;

        //Methods
        //Move to next frame
        public void nextFrame(GameTime gameTime)         //TODO need to make it frame rate independent
        {
            //Adding to the time since last draw
            timeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeElapsed > timeBetweenFrames)
            {
                timeElapsed -= timeBetweenFrames;
                //Checking to make sure we are not going over the number of frames
                if (currentFrame < (numberOfFrames - 1))
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
    }
}
