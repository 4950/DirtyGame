using System;
using System.Collections.Generic;
using System.Linq;
using DirtyGame.game.SGraphics.Commands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace DirtyGame.game.SGraphics
{
    public class SpriteSheet
    {
        #region Variables
        //The sprite sheet of the sprite
        private Texture2D spriteSheetTexture;
        //The location of the xml file that contains the animation information
        private string xmlFileLocation;
        //Stores the different animations of the sprite with a string tag
        private Dictionary<string, Rectangle[]> sAnimations = new Dictionary<string, Rectangle[]>();
//JARED //Stores the number of frames in a given animation
//JARED private Dictionary<string, int> sFrames = new Dictionary<string, int>();
        //Stores the sprite offsets for each of the animations
        private Dictionary<string, Vector2> sOffsets = new Dictionary<string, Vector2>();
//JARED //Current animation of the sprite
//JARED private string currentAnimation;
        //Current frame of the animation
        private int currentFrame;
        //Time between each frame of the sprite
        //private double timeBetweenFrames;
        //Time elapsed since last draw
        private double timeElapsed;
        #endregion

        #region Properties
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

//JARED public string CurrentAnimation
//JARED {
//JARED     get
//JARED     {
//JARED         return currentAnimation;
//JARED     }
//JARED     set
//JARED     {
//JARED         currentAnimation = value;
//JARED     }
//JARED }

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

//JARED public double TimeBetweenFrames
//JARED {
//JARED     get
//JARED     {
//JARED         //return 1f / 12.0;
//JARED         return 1f / sAnimations[currentAnimation].Length;   //   NEED TO CHANGE IN THE FUTURE
//JARED     }
//JARED }

        public Texture2D SpriteSheetTexture
        {
            get
            {
                return spriteSheetTexture;
            }
        }
        #endregion

        #region Constructors
        public SpriteSheet(Texture2D texture, string xmlFile)
        {
            //Saving pointer to the sprite sheet texture
            spriteSheetTexture = texture;
            //Saving the XML file location if needed later
            xmlFileLocation = xmlFile;
            if (xmlFile == "") return;

            //Load in the XML file of animations
            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            xmlSettings.IgnoreWhitespace = true;
            xmlSettings.IgnoreComments = true;
            XmlReader animationReader = XmlReader.Create(xmlFile, xmlSettings);

            //Parse the XML for animations
            while (animationReader.ReadToFollowing("animation"))
            {
                //animationName
                animationReader.MoveToNextAttribute();
                string animationName = animationReader.Value;
                //yPosition
                animationReader.MoveToNextAttribute();
                int yPosition = Convert.ToInt32(animationReader.Value);
                //xStartFrame
                animationReader.MoveToNextAttribute();
                int xStartFrame = Convert.ToInt32(animationReader.Value);
                //xOffset
                animationReader.MoveToNextAttribute();
                int xOffset = Convert.ToInt32(animationReader.Value);
                //yOffset
                animationReader.MoveToNextAttribute();
                int yOffset = Convert.ToInt32(animationReader.Value);
                //numberOfFrames
                animationReader.MoveToNextAttribute();
                int numberOfFrames = Convert.ToInt32(animationReader.Value);
                //width
                animationReader.MoveToNextAttribute();
                int width = Convert.ToInt32(animationReader.Value);
                //height
                animationReader.MoveToNextAttribute();
                int height = Convert.ToInt32(animationReader.Value);

                //Adding in the animation to the dictionaries
                AddAnimation(numberOfFrames, yPosition, xStartFrame, animationName, width, height, new Vector2(xOffset, yOffset));
            }
        }
        #endregion

        #region Methods
        //Adding a specified animation to the sprite component
        public void AddAnimation(int numFrames, int yPosition, int xStartFrame, string animationName, int frameWidth, int frameHeight, Vector2 frameOffset)
        {
            //Stores the rectangles for the individual frames of an animation
            Rectangle[] tempRectangles = new Rectangle[numFrames];
            //Looping through all the frames of the animation
            for (int i = 0; i < numFrames; i++)
            {
                tempRectangles[i] = new Rectangle((i + xStartFrame) * frameWidth, yPosition, frameWidth, frameHeight);
            }
            //Saving the animation to the animation Dictionary
            sAnimations.Add(animationName, tempRectangles);
            //Saving the animation's offset to the offset Dictionary
            sOffsets.Add(animationName, frameOffset);
//JARED     //Saving the animation's number of frames
//JARED     sFrames.Add(animationName, numFrames);
        }

        //Move the sprite to the next frame
        public void NextFrame(string animationName, float deltaTime)//GameTime gameTime
        {
            //Adding to the time since last draw
            timeElapsed += deltaTime;
            //Saving the time between frames
            double time_between_frames = TimeBetweenFrames(animationName);

            if (timeElapsed > time_between_frames)
            {
                timeElapsed -= time_between_frames;
                //Checking to make sure we are not going over the number of frames
                if (currentFrame < (sAnimations[animationName].Length - 1))
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

        //Gives the time between frames for a given animation
        public double TimeBetweenFrames(string animationName)
        {
            return 1.0f / sAnimations[animationName].Length;
        }
        #endregion
    }
}
