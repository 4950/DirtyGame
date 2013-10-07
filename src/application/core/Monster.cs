using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

            this.pos.xPosition = xPos;
            this.pos.yPosition = yPos;

            this.render = new RenderComponent() { rectangle = render.rectangle, texture = render.texture };
            this.render.rectangle.X = xPos;
            this.render.rectangle.Y = yPos;
            //Debug.WriteLine("MONSTER AT " + xPos + " " + yPos);
            this.render.timeOfLastDraw = new TimeSpan(0, 0, 0, 0, 0);

            this.AddComponent(this.render);
            this.AddComponent(pos);
        }
    }
}
