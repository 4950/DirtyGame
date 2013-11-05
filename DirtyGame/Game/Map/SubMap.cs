using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Util;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Map
{
    public class SubMap : TileMap
    {
        public GridMap ParentMap
        {
            get;
            private set;
        }

        public static int count = 2;
        public SubMap(GridMap parentMap, int rows, int cols, int rowOffset, int colOffset, Point position)
            : base(rows, cols, rowOffset, colOffset, parentMap.Tileset, position)
        {            
            ParentMap = parentMap;
            foreach (TileData tile in Tiles)
            {
                tile.SrcRect = new Rectangle(count, 0, 0, 0);
            }
            count++;
        }

        public override IEnumerable<TileData> Tiles
        {
            get
            {
                return ParentMap.GetTiles(Bounds);
            }
        }

        public override TileData GetTile(RowCol rc)
        {
            if (WithinBounds(rc))
            {
                return ParentMap.GetTile(rc);
            }
            else
            {
                return null;
            }
        }

        public override TileData GetTile(Point p)
        {
            if (WithinBounds(p))
            {
                return ParentMap.GetTile(p);
            }
            else
            {
                return null;
            }
        }

        public override SubMap GetSubMap(Rectangle bounds)
        {
            if (Bounds.Contains(bounds))
            {
                return ParentMap.GetSubMap(bounds);
            }
            else
            {
                return null;
            }
        }

        public override IEnumerable<TileData> GetTiles(Rectangle bounds)
        {            
            if (Bounds.Contains(bounds))
            {
                return ParentMap.GetTiles(bounds);
            }
            else
            {
                return null;
            }
        }

        public override IEnumerable<TileData> GetNeighbors(RowCol rc)
        {
            if (WithinBounds(rc))
            {
                return ParentMap.GetNeighbors(rc);
            }
            else
            {
                return null;
            }
        }

        public override TileData GetTile(uint id)
        {
            TileData tile = ParentMap.GetTile(id);
            if (WithinBounds(new RowCol(tile.Row, tile.Col)))
            {
                return tile;
            }
            else
            {
                return null;
            }
        }

        public override IEnumerable<TileData> GetNeighbors(TileData tile)
        {
            return GetNeighbors(new RowCol(tile.Row, tile.Col));
        }
    }
}
