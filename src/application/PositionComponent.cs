using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShittyPrototype.src.core;

namespace ShittyPrototype.src.application
{
    class PositionComponent : IComponent
    {
        public int x;
        public int y;

        public PositionComponent()
        {
            x = 0;
            y = 0;
        }
        public PositionComponent(int a, int b)
        {
            x = a;
            y = b;
        }

        public void incrementX(int inc)
        {
            x += inc;
        }
        public void incrementY(int inc)
        {
            y += inc;
        }
    }
}
