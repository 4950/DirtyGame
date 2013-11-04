using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using DirtyGame.game.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Map.Generators.BSP
{
    class BSPMapGenerator : IMapGenerator
    {
        enum SplitDirection
        {
            Vertically = 0,
            Horizontally = 1
        }

        private Tileset tileset;
        private int rows;
        private int columns;
        private int maxLeafSize;
        private int minLeafSize;
        private int minRoomSize;

        public BSPMapGenerator(Tileset tileset, int rows, int columns, int maxLeafSize, int minLeafSize)
        {
            this.tileset = tileset;
            this.rows = rows;
            this.columns = columns;
            this.maxLeafSize = maxLeafSize;
            this.minLeafSize = minLeafSize;
            minRoomSize = minLeafSize / 2;
            
        }

        public Map3 GenerateMap3(Point p)
        {
            Map3 map = new Map3(tileset, rows, columns, p);
            BSPNode<TileMap> root = new BSPNode<TileMap>(map);
            Queue<BSPNode<TileMap>> queue = new Queue<BSPNode<TileMap>>();            
            queue.Enqueue(root);
          
            while (queue.Count > 0)
            {
                BSPNode<TileMap> node = queue.Dequeue();
                TileMap data = node.Data;
                if (data.Rows > maxLeafSize || data.Cols > maxLeafSize || Rand.PercentChanceSuccess(75))
                {
                    Partition(node);                    
                }
                if(node.Left != null) queue.Enqueue(node.Left);
                if(node.Right != null) queue.Enqueue(node.Right);                
            }

            CreateRooms(root);         
                    
            return map;
        }

        private BSPNode<TileMap> GetRandomLeaf(BSPNode<TileMap> node)
        {
            if (node == null) return null;
            if (node.IsLeaf) return node;

            BSPNode<TileMap> left = null;
            BSPNode<TileMap> right = null;
            BSPNode<TileMap> ret = null;
            if (node.Left != null) left = GetRandomLeaf(node.Left);
            if (node.Right != null) right = GetRandomLeaf(node.Right);

            if (right != null && left != null)
            {
                if (Rand.RandBool())
                {
                    ret = right;
                  
                }
                else
                {
                    ret = left;
                  
                }
            }
            else if (right == null)
            {
                ret = left;
            }
            else if (left == null)
            {
                ret = right;
            }

            return ret;
        }


        public void CreateHall(BSPNode<TileMap> node)
        {
            BSPNode<TileMap> left = GetRandomLeaf(node.Left);
            BSPNode<TileMap> right = GetRandomLeaf(node.Right);      
            if (left == null || right == null) return;

            Point a = Utillity.GetRandomPointInside(left.Data.Bounds);
            Point b = Utillity.GetRandomPointInside(right.Data.Bounds);
        
            RowCol start = node.Data.GetRowCol(a);
            RowCol end = node.Data.GetRowCol(b);
            RowCol loc = new RowCol(start);
            
            List<RowCol> hall = new List<RowCol>();
            int hallWidthOrigin = Rand.RandInt(2, 4);
            while (!(loc.Row == end.Row && loc.Col == end.Col))
            {
                //int hallWidth = Rand.RandInt(hallWidthOrigin - 1, hallWidthOrigin + 2);
                int hallWidth = hallWidthOrigin;
                if (loc.Row < end.Row && Rand.PercentChanceSuccess(25))
                {
                    loc.Row++;                    
                    hall.Add(loc);
                    for (int i = -hallWidth/2; i < hallWidth/2; ++i)
                    {
                        RowCol rc = new RowCol(loc.Row, loc.Col + i);
                        if (node.Data.WithinBounds(rc))
                        {
                            hall.Add(rc);
                        } 
                    }
                }
                else if (loc.Row > end.Row && Rand.PercentChanceSuccess(25))
                {
                    loc.Row--;
                    hall.Add(loc);
                    for (int i = -hallWidth / 2; i < hallWidth / 2; ++i)
                    {
                        RowCol rc = new RowCol(loc.Row, loc.Col + i);
                        if (node.Data.WithinBounds(rc))
                        {
                            hall.Add(rc);
                        }
                    }
                }
                else if (loc.Col < end.Col && Rand.PercentChanceSuccess(25))
                {
                    loc.Col++;
                    hall.Add(loc);
                    for (int i = -hallWidth / 2; i < hallWidth / 2; ++i)
                    {
                        RowCol rc = new RowCol(loc.Row + i, loc.Col);
                        if (node.Data.WithinBounds(rc))
                        {
                            hall.Add(rc);
                        }
                    }
                }
                else if (loc.Col > end.Col && Rand.PercentChanceSuccess(25))
                {
                    loc.Col--;
                    hall.Add(loc);
                    for (int i = -hallWidth / 2; i < hallWidth / 2; ++i)
                    {
                        RowCol rc = new RowCol(loc.Row + i, loc.Col);
                        if (node.Data.WithinBounds(rc))
                        {
                            hall.Add(rc);
                        }
                    }
                }

                foreach (RowCol rc in hall)
                {
                    node.Data.GetTile(rc).Passable = true;                                    
                }
                hall.Clear();
                
            }
        }

        public void CreateRoom(BSPNode<TileMap> node)
        {
            TileMap map = node.Data;       
            int roomRows = Rand.RandInt(minRoomSize, map.Rows);
            int roomCols = Rand.RandInt(minRoomSize, map.Cols);
            int roomWidth = roomCols * tileset.TileWidth;
            int roomHeight = roomRows * tileset.TileHeight;

            int roomCenterRow = Rand.RandInt(0, map.Rows - roomRows);
            int roomCenterCol = Rand.RandInt(0, map.Cols - roomCols);
            int roomCenterX = roomCenterCol * tileset.TileWidth;
            int roomCenterY = roomCenterRow * tileset.TileHeight;

            Rectangle dstRect = new Rectangle(map.Bounds.X + roomCenterX, map.Bounds.Y + roomCenterY, roomWidth, roomHeight);
            TileMap subMap = map.GetSubMap(dstRect);
            node.Left = new BSPNode<TileMap>(subMap);
            foreach (TileData tile in map.GetTiles(dstRect))
            {
                tile.Passable = true;
            }
        }

        public void CreateRooms(BSPNode<TileMap> node)
        {
            if (node.Left != null || node.Right != null)
            {
                if (node.Left != null) CreateRooms(node.Left);
                if (node.Right != null) CreateRooms(node.Right);
                if (node.IsLeaf == false) CreateHall(node);           
            }
            else
            {
                // create the room
                CreateRoom(node);
            }
        }

        public void Partition(BSPNode<TileMap> node)
        {
            if (node.Left != null || node.Right != null)
            {
                return;
            }

            TileMap map = node.Data;

            //which direction are we going to split
            SplitDirection direction;
            int max = 0;
            if (map.Rows > map.Cols && (double)map.Cols / (double)map.Rows >= 0.15)
            {
                direction = SplitDirection.Horizontally;
                max = map.Rows;
            }
            else if (map.Cols > map.Rows && (double)map.Rows / (double)map.Cols >= 0.15)
            {
                direction = SplitDirection.Vertically;
                max = map.Cols;
            }
            else
            {
                // get random direction
                direction = Rand.RandEnum<SplitDirection>();
                if (direction == SplitDirection.Horizontally)
                {
                    max = map.Rows;
                }
                else
                {
                    max = map.Cols;
                }
            }

            max -= minLeafSize * 2;
            if (max <= minLeafSize) return;
            
            // choose split point
            int split = Rand.RandInt(minLeafSize, max);
            switch (direction)
            {
                case SplitDirection.Horizontally:
                    {
                        int splitDist = map.TileHeight * split;
                        int halfHeight = map.Bounds.Height / 2;
                        Rectangle topRect = new Rectangle(map.Bounds.X, map.Bounds.Y, map.Bounds.Width, splitDist);
                        Rectangle botRect = new Rectangle(map.Bounds.X, map.Bounds.Y + topRect.Height, map.Bounds.Width, map.Bounds.Height - splitDist);
                        node.Left = new BSPNode<TileMap>(map.GetSubMap(topRect));
                        node.Right = new BSPNode<TileMap>(map.GetSubMap(botRect));
                        break;
                    }
                case SplitDirection.Vertically:
                    {
                        int splitDist = map.TileWidth * split;
                        int halfWidth = map.Bounds.Width / 2;
                        Rectangle leftRect = new Rectangle(map.Bounds.X, map.Bounds.Y, splitDist, map.Bounds.Height);
                        Rectangle rightRect = new Rectangle(map.Bounds.X + leftRect.Width, map.Bounds.Y, map.Bounds.Width - splitDist, map.Bounds.Height);
                        node.Left = new BSPNode<TileMap>(map.GetSubMap(leftRect));
                        node.Right = new BSPNode<TileMap>(map.GetSubMap(rightRect));
                        break;
                    }
            }           
        }     
    }
}
