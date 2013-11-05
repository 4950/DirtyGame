using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DirtyGame.game.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace DirtyGame.game.Map
{
    public class GridMap : TileMap
    {
        public override IEnumerable<TileData> Tiles
        {
            get
            {
                return GetTiles(Bounds);
            }
        } 

        private List<List<TileData>> tiles;
        private Dictionary<uint, TileData> tileDictionary; 
        private Tileset tileset;

        public GridMap(Tileset tileset, int rows, int cols, Point position)
            : base(rows, cols, tileset, position)
        {
            tiles = new List<List<TileData>>();
            tileDictionary = new Dictionary<uint, TileData>();
            Rectangle srcRect = new Rectangle(0, 0, TileWidth, TileHeight);
            for (int i = 0; i < rows; ++i)
            {
                tiles.Add(new List<TileData>());
                for (int j = 0; j < cols; ++j)
                {
                    Point p = GetPoint(new RowCol(i, j));
                    Rectangle dstRect = new Rectangle(p.X, p.Y, TileWidth, TileHeight);
                    TileData tile = new TileData(dstRect, srcRect, i, j);
                    tiles[i].Add(tile);
                    tileDictionary.Add(tile.Id, tile);
                }
            }
        }

        public override TileData GetTile(Point p)
        {
            return GetTile(GetRowCol(p));
        }

        public override TileData GetTile(RowCol rc)
        {
            if (WithinBounds(rc)) return tiles[rc.Row][rc.Col];
            else return null;
        }

        public override SubMap GetSubMap(Rectangle bounds)
        {          
            Point tl = new Point(bounds.Left, bounds.Top);
            Point br = new Point(bounds.Right, bounds.Bottom);
            RowCol tlRc = GetRowCol(tl);
            RowCol brRc = GetRowCol(br);
            int rows = brRc.Row - tlRc.Row;
            int cols = brRc.Col - tlRc.Col;

            return new SubMap(this, rows, cols, tlRc.Row, tlRc.Col, tl);                    
        }

        public override IEnumerable<TileData> GetTiles(Rectangle bounds)
        {
            List<TileData> targetTiles = new List<TileData>();
            Point tl = new Point(bounds.Left, bounds.Top);
            Point br = new Point(bounds.Right, bounds.Bottom);
            RowCol tlRc = GetRowCol(tl);
            RowCol brRc = GetRowCol(br);
            int rows = brRc.Row - tlRc.Row;
            int cols = brRc.Col - tlRc.Col;

            List<List<TileData>> tileRows = tiles.GetRange(tlRc.Row, rows);
            foreach (List<TileData> row in tileRows)
            {
                targetTiles.AddRange(row.GetRange(tlRc.Col, cols));
            }

            return targetTiles;
        }


        public override IEnumerable<TileData> GetNeighbors(RowCol rc)
        {
            List<TileData> neighbors = new List<TileData>();
            
            TileData tile = GetTile(new RowCol(rc.Row, rc.Col - 1));
            if(tile != null) neighbors.Add(tile);
            
            tile = GetTile(new RowCol(rc.Row, rc.Col + 1));
            if (tile != null) neighbors.Add(tile);
            
            tile = GetTile(new RowCol(rc.Row - 1, rc.Col));
            if (tile != null) neighbors.Add(tile);
            
            tile = GetTile(new RowCol(rc.Row + 1, rc.Col));
            if (tile != null) neighbors.Add(tile);
            
            return neighbors;
        }

        public override TileData GetTile(uint id)
        {
            if (tileDictionary.ContainsKey(id))
            {
                return tileDictionary[id];
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
