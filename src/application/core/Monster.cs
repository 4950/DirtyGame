using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ShittyPrototype.src.core;
using ShittyPrototype.src.graphics;

namespace ShittyPrototype.src.application.core
{
    class Monster : Entity
    {
        public RenderComponent render;
        public PositionComponent pos;

        public Monster(int xPos, int yPos, RenderComponent render)
        {
            pos = new PositionComponent();

            this.pos.x = xPos;
            this.pos.y = yPos;

            this.render = new RenderComponent() { rectangle = render.rectangle, texture = render.texture };
            this.render.rectangle.X = xPos;
            this.render.rectangle.Y = yPos;
            //Debug.WriteLine("MONSTER AT " + xPos + " " + yPos);
            this.render.timeOfLastDraw = new TimeSpan(0, 0, 0, 0, 0);

            //Setting the first frame of the animation
            this.render.currentFrame = 0;                   //Needs to be passed in value
            //Number of frames in the animation
            this.render.numberOfFrames = 12;                //Needs to be passed in value
            //Frame Width
            this.render.frameWidth = 50;                    //Needs to be passed in value
            //Frame Height
            this.render.frameHeight = 50;                   //Needs to be passed in value
            //
            this.render.timeBetweenFrames = 1f / this.render.numberOfFrames;

            //Rectangles of the sprite textures on the sprite strip
            this.render.spriteRectangles = new Rectangle[this.render.numberOfFrames];
            //Setting each frame in a rectangle
            for (int i = 0; i < this.render.numberOfFrames; i++)
            {
                //TODO need to change this to allow sprite sheet not just sprite strip
                this.render.spriteRectangles[i] = new Rectangle((i + this.render.startFrame) * this.render.frameWidth,
                                                                0, //TODO change to allow sprite sheets
                                                                this.render.frameWidth,
                                                                this.render.frameHeight);
            }

            this.AddComponent(this.render);
            this.AddComponent(pos);
        }
    }
}
