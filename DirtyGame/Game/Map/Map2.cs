using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Map
{
    class Map2
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

        public int WidthInPixels
        {
            get;
            private set;
        }

        public int HeightInPixels
        {
            get;         
            private set;
        }

        public Tileset Tileset
        {
            get;
            private set;
        }

        private List<TileData> tiles;    

        public Map2(Tileset tileset, int rows, int cols, List<TileData> tileData)
        {
            Tileset = tileset;
            Rows = rows;
            Cols = cols;
            HeightInPixels = Tileset.TileHeight* Rows;
            WidthInPixels = Tileset.TileWidth * Cols;
            tiles = tileData;
        }

        public Map.TileData GetTile(float x, float y)
        {
            return null;
        }

        public Map.TileData GetTile(int row, int col)
        {
            return null;
        }

        public void GetRowCol(float x, float y, out int row, out int col)
        {
            row = 0;
            col = 0;
        }

        public void GetPosition(int row, int col, out Vector2 position)
        {
            position.X = 0;
            position.Y = 0;
        }

        public bool IsOnMap(int row, int col)
        {
            return row < Rows && col < Cols && row >= 0 && col >= 0;
        }

        public bool IsOnMap(float x, float y)
        {
            int halfWidth = WidthInPixels/2;
            int halfHeight = HeightInPixels/2;
            return x > -halfWidth && x < halfWidth && y > -halfHeight && y < halfHeight;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Begin();
            foreach (TileData data in tiles)
            {
                batch.Draw(Tileset.Texture, data.DstRect, data.SrcRect, Color.White);
            }
            batch.End();
        }
    }
}
