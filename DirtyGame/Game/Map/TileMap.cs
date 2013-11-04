using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Map
{
    class TileMap
    {
        public int Rows
        {
            get;
            private set;
        }

        public int Cols
        {
            get;
            private set;
        }

        public Rectangle DstRect
        {
            get;
            private set;
        }

        public int TileWidth
        {
            get;
            private set;
        }

        public int TileHeight
        {
            get;
            private set;
        }

        public TileMap(int rows, int cols, int tileWidth, int tileHeight, Point topLeft)
        {
            Rows = rows;
            Cols = cols;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            DstRect = new Rectangle(topLeft.X, topLeft.Y, cols * tileWidth, rows * tileHeight);
        }

        public TileMap(int rows, int cols, int tileWidth, int tileHeight, Rectangle dstRect)
        {
            Rows = rows;
            Cols = cols;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            DstRect = dstRect;
        }
    }
}
