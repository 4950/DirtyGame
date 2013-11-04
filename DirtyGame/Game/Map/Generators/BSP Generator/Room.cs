using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Map.Generators.BSP_Generator
{
    class Room
    {
        public Rectangle DstRect
        {
            get;
            private set;
        }
        public Room(Rectangle dstRect, int rows, int cols)
        {
            DstRect = dstRect;
        }
    }
}
