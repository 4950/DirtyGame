using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.SGraphics;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Map
{
    public class Tileset
    {
        public int TileWidth
        {
            get;
            set;
        }

        public int TileHeight
        {
            get;
            set;
        }

        public Texture2D Texture
        {
            get;
            private set;
        }

        public Tileset(Texture2D texture, int tileWidth, int tileHeight)
        {
            Texture = texture;
            TileHeight = tileHeight;
            TileWidth = tileWidth;
        }
    }
}
