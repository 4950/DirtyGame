using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Util;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Map
{
    public abstract class TileMap
    {
        public abstract IEnumerable<TileData> Tiles
        {
            get;
        }

        public abstract TileData GetTile(RowCol rc);
        public abstract TileData GetTile(Point p);
        public abstract SubMap GetSubMap(Rectangle bounds);
        public abstract IEnumerable<TileData> GetTiles(Rectangle bounds);

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

        public Rectangle Bounds
        {
            get;
            private set;
        }

        public int TileWidth
        {
            get
            {
                return Tileset.TileWidth;
            }        
        }

        public int TileHeight
        {
            get
            {
                return Tileset.TileHeight;
            }     
        }

        public Tileset Tileset
        {
            get;
            private set;
        }

        public int RowOffset
        {
            get;
            private set;
        }

        public int ColOffset
        {
            get;
            private set;
        }

        public TileMap(int rows, int cols, Tileset tileset, Point topLeft)
        {
            Rows = rows;
            Cols = cols;
            Tileset = tileset;
            RowOffset = 0;
            ColOffset = 0;
            Bounds = new Rectangle(topLeft.X, topLeft.Y, Cols * TileWidth, Rows * TileHeight);           
        }

        public TileMap(int rows, int cols, int rowOffset, int colOffset, Tileset tileset, Point topLeft)
        {
            Rows = rows;
            Cols = cols;
            Tileset = tileset;
            RowOffset = rowOffset;
            ColOffset = colOffset;
            Bounds = new Rectangle(topLeft.X, topLeft.Y, Cols * TileWidth, Rows * TileHeight);
        }       

        public bool WithinBounds(RowCol rc)
        {
            return rc.Row >= RowOffset && rc.Row < Rows + RowOffset && rc.Col >= ColOffset && rc.Col < Cols + ColOffset;
        }

        public bool WithinBounds(Point p)
        {
            return Bounds.Contains(p);
        }

        public int GetPixelWidth()
        {
            return Bounds.Width;
        }

        public int GetPixelHeight()
        {
            return Bounds.Height;
        }
        
        public RowCol GetRowCol(Point p)
        {         
            if (p.X < Bounds.X || p.X > Bounds.X + Bounds.Width)
            {
                throw new Exception();
            }
            if (p.Y < Bounds.Y || p.Y > Bounds.Y + Bounds.Height)
            {
                throw new Exception();
            }
            int row = p.Y/TileHeight;
            int col = p.X/TileWidth;
            return new RowCol(p.Y/TileHeight, p.X/TileWidth);
        }

        public Point GetPoint(RowCol rc)
        {
            rc.Row -= RowOffset;
            rc.Col -= ColOffset;
            if (rc.Row < 0 || rc.Row > Rows)
            {
                throw new Exception();
            }
            if (rc.Col < 0 || rc.Col > Cols)
            {
                throw new Exception();
            }
            return new Point(rc.Col * (TileHeight), rc.Row * (TileWidth));
        }
    }
}
