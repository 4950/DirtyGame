using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ShittyPrototype.src.graphics;

namespace ShittyPrototype.src.Map
{
    class Map
    {
        public class TileData
        {
            public Rectangle coords; //Coords of tile in tileset
            public bool passable; //is this tile type passable
        }
        public class TileSet
        {
            public int width, height; //width and height(in tiles) of tileset
            public int tilewidth, tileheight; //tile width and height
            public String name; //name of tileset
            public Dictionary<int, TileData> tiles; //maps GIDs to tiledata
            public Texture2D tex;

            public TileSet()
            {
                tiles = new Dictionary<int, TileData>();
            }
            public void LoadSet(XmlElement setXML, GraphicsDevice dev, Map map, ContentManager Content)
            {
                int firstGID = int.Parse(setXML.GetAttribute("firstgid"));
                tilewidth = int.Parse(setXML.GetAttribute("tilewidth"));
                tileheight = int.Parse(setXML.GetAttribute("tileheight"));
                name = setXML.GetAttribute("name");

                XmlElement img = (XmlElement)setXML.FirstChild;
                int iwidth = int.Parse(img.GetAttribute("width"));
                int iheight = int.Parse(img.GetAttribute("height"));
                String imgFilename = img.GetAttribute("source");
                tex = Content.Load<Texture2D>(imgFilename);


                width = iwidth / tilewidth;
                height = iheight / tileheight;

                int tileID = firstGID;
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {

                        TileData t = new TileData();
                        t.coords = new Rectangle(tilewidth * col, tileheight * row, tilewidth, tileheight);
                        t.passable = true;
                        tiles.Add(tileID, t);
                        map.tileSetsByTileGID.Add(tileID, this);
                        tileID++;
                    }
                }

            }

        }
        struct LayerTile
        {
            public TileData tile; //source tile
            public Rectangle dest; //Destination rect
        }
        class MapLayer
        {
            //int zIndex; //zindex of this layer
            String name; //name of layer
            int width, height; //width and height of layer in tiles
            Dictionary<TileSet, List<LayerTile>> tileRectsBySet;

            public MapLayer()
            {
                tileRectsBySet = new Dictionary<TileSet, List<LayerTile>>();
            }
            public void LoadLayer(XmlElement layerXML, Map map)
            {
                name = layerXML.GetAttribute("name");
                width = int.Parse(layerXML.GetAttribute("width"));
                height = int.Parse(layerXML.GetAttribute("height"));

                int tileX = 0, tileY = 0;
                //Stupid attempt at base64 decode and zlib inflate
                /*
                XmlElement tiles = (XmlElement)layerXML.FirstChild;
                if (tiles.GetAttribute("encoding") == "base64")
                {
                    String cleantxt = tiles.InnerText.Replace('\n', ' ').Replace('\r', ' ').Replace(" ", string.Empty);
                    byte[] data = Convert.FromBase64String(cleantxt);
                    byte[] newdata = new byte[data.Length - 6];
                    Buffer.BlockCopy(data, 2, newdata, 0, data.Length - 6);
                    Stream r = new MemoryStream(newdata);
                    String decode = Encoding.UTF8.GetString(Convert.FromBase64String(cleantxt));
                    //decode.
                    //r.ReadByte();
                    //r.ReadByte();
                    
                    if (tiles.GetAttribute("compression") == "zlib")
                    {
                        r = new System.IO.Compression.DeflateStream(r, System.IO.Compression.CompressionMode.Decompress);
                    }
                    String tileXML = new StreamReader(r, System.Text.Encoding.UTF8).ReadToEnd();

                }*/
                foreach (XmlElement tile in ((XmlElement)layerXML.FirstChild).ChildNodes)
                {
                    int GID = int.Parse(tile.GetAttribute("gid"));
                    TileSet set = map.tileSetsByTileGID[GID];
                    LayerTile lt = new LayerTile();
                    lt.dest = new Rectangle(tileX * map.tileWidth, tileY * map.tileHeight, map.tileWidth, map.tileHeight);
                    lt.tile = set.tiles[GID];

                    if (!tileRectsBySet.ContainsKey(set))
                        tileRectsBySet.Add(set, new List<LayerTile>());
                    tileRectsBySet[set].Add(lt);

                    tileX++;
                    if (tileX == width)
                    {
                        tileY++;
                        tileX = 0;
                    }
                    if (tileY == height)
                        break;
                }
            }
            public void draw(SpriteBatch batch)
            {
                foreach (var entry in tileRectsBySet)
                {
                    foreach (LayerTile t in entry.Value)
                    {
                        batch.Draw(entry.Key.tex, t.dest, t.tile.coords, Color.White);
                    }
                }
            }
        }

        int tileWidth, tileHeight; //Width and height of tiles
        int width, height; //width and height of map
        Dictionary<String, TileSet> tileSets;
        Dictionary<int, TileSet> tileSetsByTileGID;
        List<MapLayer> layers;
        SpriteBatch batch;

        public int getPixelHeight()
        {
            return height * tileHeight;
        }

        public int getPixelWidth()
        {
            return width * tileWidth;
        }

        public Map(GraphicsDevice dev)
        {
            tileSets = new Dictionary<string, TileSet>();
            tileSetsByTileGID = new Dictionary<int, TileSet>();
            layers = new List<MapLayer>();
            batch = new SpriteBatch(dev);
        }
        public bool LoadMap(String filename, GraphicsDevice dev, ContentManager Content)
        {
            filename = Program.Path + filename;
            if (!File.Exists(filename))
                return false;
            try
            {
                XmlDocument mapDoc = new XmlDocument();
                mapDoc.Load(filename);

                XmlElement mapXML = (XmlElement)mapDoc.GetElementsByTagName("map")[0];
                tileWidth = int.Parse(mapXML.GetAttribute("tilewidth"));
                tileHeight = int.Parse(mapXML.GetAttribute("tileheight"));
                width = int.Parse(mapXML.GetAttribute("width"));
                height = int.Parse(mapXML.GetAttribute("height"));

                XmlNodeList tileSetsXML = mapXML.GetElementsByTagName("tileset");
                foreach (XmlElement tileSetXML in tileSetsXML)
                {
                    TileSet T = new TileSet();
                    T.LoadSet(tileSetXML, dev, this, Content);
                    tileSets.Add(T.name, T);
                }

                XmlNodeList layersXML = mapXML.GetElementsByTagName("layer");
                foreach (XmlElement layerXML in layersXML)
                {
                    MapLayer l = new MapLayer();
                    l.LoadLayer(layerXML, this);
                    layers.Add(l);
                }

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public void draw(Camera cam)
        {
            batch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, cam.TransformMatrix);
            foreach (MapLayer l in layers)
            {
                l.draw(batch);
            }
            batch.End();
        }
    }
}
