using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Util;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Map
{
    public class TileData
    {
        public uint Id
        {
            get;
            private set;
        }

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

        public int Row
        {
            get;
            private set;
        }

        public int Col
        {
            get;
            private set;
        }

        public TileData(Rectangle dstRect, Rectangle srcRect, int row, int col)
        {
            Id = (uint)Guid.NewGuid().GetHashCode();
            Row = row;
            Col = col;
            Visible = true;
            Passable = false;
            DstRect = dstRect;
            SrcRect = srcRect;
        }
    }
}
