using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Map.Generators.BSP_Generator
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
            minRoomSize = 10;
            
        }

        public Map2 GenerateMap(int x, int y)
        {            
            Space root = new Space(new Rectangle(x, y, tileset.TileWidth * columns, tileset.TileHeight * rows), rows, columns);
            Queue<Space> queue = new Queue<Space>();
            List<Space> leaves = new List<Space>();                        
            queue.Enqueue(root);          

            Random random = RandomNumberGenerator.Rand;
            while (queue.Count > 0)
            {
                Space space = queue.Dequeue();
                if (space.rows > maxLeafSize || space.cols > maxLeafSize || random.Next(100) > 25)
                {
                    PartitionSpace(space);
                }
                if (space.left == null)
                {
                    leaves.Add(space);
                }
                else
                {
                    queue.Enqueue(space.left);
                    queue.Enqueue(space.right);
                }             
            }

            // convert to tiles
            List<TileData> tiles = new List<TileData>();
            int k = 1;
            foreach (Space space in leaves)
            {
                CreateRoom(space);
               
                for (int i = 0; i < space.rows; ++i)
                {
                    for (int j = 0; j < space.cols; ++j)
                    {
                        TileData data = new TileData();
                        data.DstRect = space.dstRect;
                        data.SrcRect = new Rectangle(k, 0, 0, 0);
                        data.Passable = false;
                        tiles.Add(data);
                    }
                }
                TileData data2 = new TileData();
                data2.DstRect = space.Room.DstRect;
                data2.SrcRect = new Rectangle(0, 0, 0, 0);
                data2.Passable = false;
                tiles.Add(data2);
                k += 1;
            }

            return new Map2(tileset, rows, columns, tiles);

        }

        private void CreateRoom(Space space)
        {
            Random rand = RandomNumberGenerator.Rand;
            int roomRows = rand.Next(space.rows - minRoomSize) + minRoomSize;            
            int roomCols = rand.Next(space.cols - minRoomSize) + minRoomSize;
            int roomWidth = roomCols * tileset.TileWidth;
            int roomHeight = roomRows*tileset.TileHeight;
         
            int roomCenterRow = rand.Next(space.rows - roomRows);
            int roomCenterCol = rand.Next(space.cols - roomCols);
            int roomCenterX = roomCenterCol*tileset.TileWidth;
            int roomCenterY = roomCenterRow*tileset.TileHeight;

            Rectangle dstRect = new Rectangle(space.dstRect.X + roomCenterX, space.dstRect.Y + roomCenterY, roomWidth, roomHeight);
            space.Room = new Room(dstRect, roomRows, roomCols);
        }

        private void CreateHall(Space fromSpace, Space toSpace)
        {
            Point a = Utillity.GetRandomPointInside(fromSpace.Room.DstRect);
            Point b = Utillity.GetRandomPointInside(toSpace.Room.DstRect);
        }

        private void PartitionSpace(Space space)
        {
            if (space.left != null || space.right != null)
            {
                return;
            }

            Random random = RandomNumberGenerator.Rand;

            //which direction are we going to split
            SplitDirection direction;
            int max = 0;
            if (space.rows > space.cols && (double)space.cols / (double)space.rows >= 0.15)
            {
                direction = SplitDirection.Horizontally;
                max = space.rows;              
            }
            else if (space.cols > space.rows && (double)space.rows / (double)space.cols >= 0.15)
            {
                direction = SplitDirection.Vertically;
                max = space.cols;
            }
            else
            {
                // get random direction
                Array values = Enum.GetValues(typeof(SplitDirection));
                direction = (SplitDirection)values.GetValue(random.Next(values.Length));
                if (direction == SplitDirection.Horizontally)
                {
                    max = space.rows;
                }
                else
                {
                    max = space.cols;
                }
                
            }

            max -= minLeafSize * 2;          
           
            if (max <= minLeafSize)
            {
                return;
            }
            
            // choose split point
            int split = random.Next(max) + minLeafSize;
            switch (direction)
            {
                case SplitDirection.Horizontally:
                {
                    int splitDist = tileset.TileHeight * split;
                    int halfHeight = space.dstRect.Height / 2;
                    Rectangle topRect = new Rectangle(space.dstRect.X, space.dstRect.Y, space.dstRect.Width, splitDist);
                    Rectangle botRect = new Rectangle(space.dstRect.X, space.dstRect.Y + topRect.Height, space.dstRect.Width, space.dstRect.Height - splitDist);                   
                    space.left = new Space(topRect, split, space.cols);
                    space.right = new Space(botRect, space.rows - split, space.cols);
                    break;
                }
                case SplitDirection.Vertically:
                {
                    int splitDist = tileset.TileWidth * split;
                    int halfWidth = space.dstRect.Width / 2;
                    Rectangle leftRect = new Rectangle(space.dstRect.X, space.dstRect.Y, splitDist, space.dstRect.Height);
                    Rectangle rightRect = new Rectangle(space.dstRect.X + leftRect.Width, space.dstRect.Y, space.dstRect.Width - splitDist, space.dstRect.Height);
                    space.left = new Space(leftRect, space.rows, split);
                    space.right = new Space(rightRect, space.rows, space.cols - split);
                    break;
                }
            }
            if (space.left.rows < 0 || space.left.cols < 0 || space.right.rows < 0 || space.right.cols < 0)
            {
                Console.WriteLine("err");
            }
        }    
 
    }
}
