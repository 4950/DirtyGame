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

            //Finished reading the file
          //  bool finished = false;

            animationReader.ReadToFollowing("root");
           // animationReader.ReadToFollowing("animationDefault") || animati

            //Parse the XML for animations
            while (animationReader.Read())
            {
                //Temporary Variables
                string animationName;
                int xPosition;
                int yPosition;
                int xStartFrame;
                int xOffset;
                int yOffset;
                int numberOfFrames;
                int width;
                int height;
                int frameCount = 0;

                //Switching between the type of animation XML definitions
                switch (animationReader.Name)
                {
                    case "animationDefault":
                        //animationName
                        animationName = animationReader.GetAttribute("name");
                        //yPosition
                        yPosition = Convert.ToInt32(animationReader.GetAttribute("yPosition"));
                        //xStartFrame
                        xStartFrame = Convert.ToInt32(animationReader.GetAttribute("xStartFrame"));
                        //xOffset
                        xOffset = Convert.ToInt32(animationReader.GetAttribute("xOffset"));
                        //yOffset
                        yOffset = Convert.ToInt32(animationReader.GetAttribute("yOffset"));
                        //numberOfFrames
                        numberOfFrames = Convert.ToInt32(animationReader.GetAttribute("numberOfFrames"));
                        //width
                        width = Convert.ToInt32(animationReader.GetAttribute("width"));
                        //height
                        height = Convert.ToInt32(animationReader.GetAttribute("height"));

                        //Adding the animation to the dictionaries
                        AddAnimationDefault(numberOfFrames, yPosition, xStartFrame, animationName, width, height, new Vector2(xOffset, yOffset));

                        break;

                    case "animationByFrame":
                        //animationName
                        animationName = animationReader.GetAttribute("name");
                        //xOffset
                        xOffset = Convert.ToInt32(animationReader.GetAttribute("xOffset"));
                        //yOffset
                        yOffset = Convert.ToInt32(animationReader.GetAttribute("yOffset"));
                        //numberOfFrames
                        numberOfFrames = Convert.ToInt32(animationReader.GetAttribute("numberOfFrames"));

                        //Temporary Rectangle array
                        Rectangle[] tempRectangles = new Rectangle[numberOfFrames];

                        //Looping through each frame
                        while (animationReader.ReadToFollowing("frame"))
                        {
                            //xPosition
                            xPosition = Convert.ToInt32(animationReader.GetAttribute("xPosition"));
                            //yPosition
                            yPosition = Convert.ToInt32(animationReader.GetAttribute("yPosition"));
                            //width
                            width = Convert.ToInt32(animationReader.GetAttribute("width"));
                            //height
                            height = Convert.ToInt32(animationReader.GetAttribute("height"));

                            //Adding the animation to the dictionaries
                            tempRectangles[frameCount] = new Rectangle(xPosition, yPosition, width, height);
                            
                            //Incrementing the frame count
                            frameCount++;
                        }

                        //Reseting the frame count
                        frameCount = 0;

                        //Saving the animation to the animation Dictionary
                        sAnimations.Add(animationName, tempRectangles);
                        //Saving the animation's offset to the offset Dictionary
                        sOffsets.Add(animationName, new Vector2(xOffset, yOffset));

                        break;
                }

                animationReader.Read(); //Why do I need two XMLReader.Read() methods? If not there it will have null values for all the attributes.
            }
        }
        #endregion

        #region Methods
        //Adding a specified animation to the sprite component, default
        private void AddAnimationDefault(int numFrames, int yPosition, int xStartFrame, string animationName, int frameWidth, int frameHeight, Vector2 frameOffset)
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
