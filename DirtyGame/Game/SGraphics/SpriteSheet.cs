using System;
using System.Collections.Generic;
using System.Linq;
using DirtyGame.game.SGraphics.Commands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace DirtyGame.game.SGraphics
{
    class SpriteSheet
    {
        #region Variables
        //The sprite sheet of the sprite
        private Texture2D spriteSheetTexture;
        //The location of the xml file that contains the animation information
        private string xmlFileLocation;
        //Stores the different animations of the sprite with a string tag
        private Dictionary<string, Rectangle[]> sAnimations = new Dictionary<string, Rectangle[]>();
        //Stores the sprite offsets for each of the animations
        private Dictionary<string, Vector2> sOffsets = new Dictionary<string, Vector2>();
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
        }
        #endregion
    }
}
