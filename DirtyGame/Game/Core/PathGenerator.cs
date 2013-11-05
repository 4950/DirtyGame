using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Map;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Core
{
    class PathGenerator
    {
        public Path GeneratePathFromTiles(List<TileData> tiles)
        {
            Path path = new Path();
            if (tiles.Count() == 0) return path;
            
            Point lastPoint = tiles[0].DstRect.Center;            
            path.Points.Add(lastPoint);
            bool h = false;
            bool v = false;
            bool changedDir = false;
            for (int i = 1; i < tiles.Count(); ++i)
            {
                Point p = tiles[i].DstRect.Center;
                if (p.X == lastPoint.X)
                {
                    if (h) changedDir = true;
                    v = true;
                    h = false;
                }
                if (p.Y == lastPoint.Y)
                {
                    if (v) changedDir = true;
                    v = false;
                    h = true;
                }


                if (changedDir)
                {
                    changedDir = false;
                    path.Points.Add(lastPoint);                                       
                }
                lastPoint = p;                
            }
            path.Points.Add(tiles[tiles.Count - 1].DstRect.Center);
            return path;
        }
    }
}
