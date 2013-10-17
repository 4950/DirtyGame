using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShittyPrototype.src.graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShittyPrototype.src.application.core;
using ShittyPrototype.src.application;

namespace ShittyPrototype.src.core
{
    class Spawner : Entity
    {
        public MonsterComponent monster = new MonsterComponent();
        public SpawnerComponent spawner = new SpawnerComponent();
        public PositionComponent pos = new PositionComponent();
        public RenderComponent render = new RenderComponent();
        
        public Spawner() { }

        
        public Spawner(int spawnTimeMilliseconds, int xLocation, int yLocation, int numMobs, Texture2D textures) //Need to pass in a series of items including Content
        {
            //this.monster.monster = monster;

            this.spawner.timePerSpawn = new TimeSpan(0,0,0,0,spawnTimeMilliseconds);
            this.spawner.numMobs = numMobs;
            this.spawner.timeOfLastSpawn = new TimeSpan(0, 0, 0, 0, 0);

            //Location of the spawner
            this.pos.x = xLocation;
            this.pos.y = yLocation;
            
            //Rendering information for the monsters
            this.render.texture = textures;
            this.render.rectangle = new Rectangle(pos.x, pos.y, 46, 46);
/*            //Need to put in
            //  the number of frames
            //  texture name to load in a texture2D with content
            //  
            //Setting the first frame of the animation
            this.render.currentFrame = 0;                   //Needs to be passed in value
            //Number of frames in the animation
            this.render.numberOfFrames = 12;                //Needs to be passed in value
            //Frame Width
            this.render.frameWidth = 50;                    //Needs to be passed in value
            //Frame Height
            this.render.frameHeight = 50;                   //Needs to be passed in value


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
*/

            this.AddComponent(render);
            this.AddComponent(pos);
            this.AddComponent(spawner);
            
        }
    }
}
