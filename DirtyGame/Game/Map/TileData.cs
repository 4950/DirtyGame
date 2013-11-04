using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Map
{
    public class TileData
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

        public bool Visible
        {
            get;
            set;
        }

        public TileData(Rectangle dstRect, Rectangle srcRect)
        {
            Visible = true;
            Passable = false;
            DstRect = dstRect;
            SrcRect = srcRect;
        }
    }
}
