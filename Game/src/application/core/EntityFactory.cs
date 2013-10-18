using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShittyPrototype.src.graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ShittyPrototype.src.application;
using Microsoft.Xna.Framework.Content;

namespace ShittyPrototype.src.core
{
    class EntityFactory
    {
        private GraphicsDevice _graphicsDevice;

        public EntityFactory(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }
        public Entity CreateTestEntity()
        {
            Entity entity = new Entity();
            RenderComponent renderComp = new RenderComponent();

            renderComp.texture = new Texture2D(_graphicsDevice, 1, 1);
            renderComp.texture.SetData(new Color[] { Color.AliceBlue });

            renderComp.rectangle = new Rectangle(0, 0, 50, 50);
            entity.AddComponent(renderComp);

            PositionComponent posComp = new PositionComponent(0, 0);
            entity.AddComponent(posComp);

            return entity;
        }

        public Entity createPlayerEntity(String spriteTexture, int spriteWidth, int spriteHeight, int numFrames, ContentManager Content)
        {
            PositionComponent posComp = new PositionComponent(400, 200);

            RenderComponent renderComp = new RenderComponent();
            //renderComp.texture = new Texture2D(_graphicsDevice, 1, 1);
            //renderComp.texture.SetData(new Color[] { Color.Fuchsia });
            //renderComp.rectangle = new Rectangle(posComp.x, posComp.y, 50, 50);

            //Adding player sprite
            renderComp.texture = Content.Load<Texture2D>(spriteTexture);
            //Setting the bounds of the sprite texture
            renderComp.rectangle = new Rectangle(posComp.x, posComp.y, spriteWidth, spriteWidth);
            //Setting the number of frames
            renderComp.numberOfFrames = numFrames;
            //
            renderComp.startFrame = 0;
            //
            renderComp.numberOfFrames = 12;
            //
            //Frame Width
            renderComp.frameWidth = 50;                    //Needs to be passed in value
            //Frame Height
            renderComp.frameHeight = 50;                   //Needs to be passed in value
            //
            renderComp.timeBetweenFrames = 1f / renderComp.numberOfFrames;

            //Rectangles of the sprite textures on the sprite strip
            renderComp.spriteRectangles = new Rectangle[renderComp.numberOfFrames];
            //Setting each frame in a rectangle
            for (int i = 0; i < renderComp.numberOfFrames; i++)
            {
                //TODO need to change this to allow sprite sheet not just sprite strip
                renderComp.spriteRectangles[i] = new Rectangle((i + renderComp.startFrame) * renderComp.frameWidth,
                                                                0, //TODO change to allow sprite sheets
                                                                renderComp.frameWidth,
                                                                renderComp.frameHeight);
            }



            InputComponent inputComponent = new InputComponent();

            Entity player = new Entity();
            player.AddComponent(posComp);
            player.AddComponent(renderComp);
            player.AddComponent(inputComponent);
            return player;

        }
    }
}
