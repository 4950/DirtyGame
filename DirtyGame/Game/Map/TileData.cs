using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Map
{
    class TileData
    {
        public Rectangle DstRect
        {
            get;
            set;
        }

        public Rectangle SrcRect
        {
            get;
            set;
        }

        public bool Passable
        {
            get;
            set;
        }
    }
}
