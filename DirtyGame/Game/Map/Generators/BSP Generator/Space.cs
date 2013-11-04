using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Map.Generators.BSP_Generator
{
    class Space
    {     
        enum SplitDirection
        {
            Vertically = 0,
            Horizontally = 1
        }


        public Rectangle dstRect;
        public int cols;
        public int rows;
        public Space left;
        public Space right;

        public Room Room
        {
            get;
            set;
        }

        public Space(Rectangle dstRect, int rows, int cols)
        {
            this.dstRect = dstRect;
            this.cols = cols;
            this.rows = rows;
            left = null;
            right = null;
            Room = null;
        }

       

    }
}
