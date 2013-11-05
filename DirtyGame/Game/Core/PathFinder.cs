using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Map;
using DirtyGame.game.Util;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Core
{
    class PathFinder
    {
        class SearchNode : IComparable<SearchNode>
        {
            public int F
            {
                get
                {
                    return G + H;
                }
            }

            public int G
            {
                get;
                set;
            }

            public int H
            {
                get;
                set;
            }

            public uint Id
            {
                get
                {
                    return Tile.Id;
                }                
            }

            public TileData Tile
            {
                get;
                set;
            }


            private SearchNode parent;
            public SearchNode Parent
            {
                get
                {
                    return parent;
                }
                set
                {
                    parent = value;
                    SearchNode node = parent;
                    G = 0;
                    while (node != null)
                    {
                        G++;
                        node = node.Parent;
                    }
                }
            }

            public SearchNode(int h, TileData tile, SearchNode parent)
            {
                G = 0;
                H = h;
                Tile = tile;
                Parent = parent;             
            }

            public int CompareTo(SearchNode other)
            {
                return F - (other.F);
            }
        }
                

        public Path GetPath2(TileMap map, Point start, Point end)
        {
            Dictionary<uint, SearchNode> openAccessor = new Dictionary<uint, SearchNode>();
            MinHeap<SearchNode> open = new MinHeap<SearchNode>();
            Dictionary<uint, SearchNode> closed = new Dictionary<uint,SearchNode>();

            TileData origin = map.GetTile(start);
            TileData dest = map.GetTile(end);

            SearchNode node = new SearchNode(0, origin, null);          
            open.Insert(node);
            openAccessor.Add(origin.Id, node);

            IEnumerable<TileData> neighbors;
            SearchNode winner = null;
            while (open.Count > 0)
            {
                node = open.ExtractMin();
                if (node.Tile == dest)
                {
                    winner = node;
                    break;
                }
                closed.Add(node.Id, node);
                neighbors = map.GetNeighbors(node.Tile);
                foreach (TileData tile in neighbors)
                {
                    if (tile.Passable == false) continue;
                    if (closed.ContainsKey(tile.Id)) continue;
                    if (openAccessor.ContainsKey(tile.Id))
                    {
                        SearchNode next = openAccessor[tile.Id];
                        //check if cost is better
                        SearchNode newNode = new SearchNode(Utillity.GetManhattanDistance(tile.RowCol, dest.RowCol), tile, node);
                        if (newNode.F < next.F)
                        {
                            next.Parent = node;
                            next.H = newNode.H;
                            //update in key in open list
                            open.UpdateKey(next);
                        }
                    }
                    else
                    {
                        SearchNode next = new SearchNode(Utillity.GetManhattanDistance(tile.RowCol, dest.RowCol), tile, node);
                        openAccessor.Add(next.Id, next);
                        open.Insert(next);
                    }
                }                
            }

            if (winner == null)
            {
                return new Path();
            }
            else
            {
                node = winner;
                List<TileData> tiles = new List<TileData>();
                while (node != null)
                {                    
                    node.Tile.SrcRect = new Rectangle(60, 0, 0, 0);
                    tiles.Add(node.Tile);
                    node = node.Parent;
                    
                }      
                Path path = new PathGenerator().GeneratePathFromTiles(tiles);
                return path;
            }
            
            
        }        
    }
}
