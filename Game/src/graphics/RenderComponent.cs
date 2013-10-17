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
        public Texture2D texture;
        public Rectangle rectangle;


        public TimeSpan timeOfLastDraw;
        public TimeSpan timeBetweenDraw;
    }
}
