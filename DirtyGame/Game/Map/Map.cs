using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using DirtyGame;
using DirtyGame.game.SGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace DirtyGame.game.Map
{
    public class Map
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
        class MapLayerSet//class for one tileset of one layer
        {
            TileSet set;
            MapLayer layer;
            BasicEffect quadEffect;
            VertexBuffer verts;
            IndexBuffer inds;

            public MapLayerSet(TileSet set, MapLayer layer, GraphicsDevice dev)
            {
                this.set = set;
                this.layer = layer;

                Matrix View = Matrix.CreateLookAt(new Vector3(20, 1, 0), new Vector3(20, -1, 0), Vector3.Forward);
                Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4.0f / 3.0f, 1, 500);

                quadEffect = new BasicEffect(dev);
                quadEffect.LightingEnabled = false;
                quadEffect.World = Matrix.Identity;
                quadEffect.View = View;
                quadEffect.Projection = Projection;
                quadEffect.TextureEnabled = true;
                quadEffect.Texture = set.tex;

                List<LayerTile> tiles = layer.tileRectsBySet[set];
                int len = tiles.Count;

                verts = new VertexBuffer(dev, VertexPositionTexture.VertexDeclaration, 4*len, BufferUsage.WriteOnly);
                inds = new IndexBuffer(dev, typeof(int), 6*len, BufferUsage.WriteOnly);
                VertexPositionTexture[] v = new VertexPositionTexture[4 * len];
                int[] ins = new int[6*len];

                int i;
                for (i = 0; i < len; i++)
                {
                    ins[i * 6] = i * 4 + 0;
                    ins[i * 6 + 1] = i * 4 + 1;
                    ins[i * 6 + 2] = i * 4 + 2;
                    ins[i * 6 + 3] = i * 4 + 2;
                    ins[i * 6 + 4] = i * 4 + 1;
                    ins[i * 6 + 5] = i * 4 + 3;
                }

                Vector2 texSize = new Vector2(set.tex.Width, set.tex.Height);

                i = 0;
                foreach (LayerTile tile in tiles)
                {
                    Vector2[] texCoords = getTexCoords(tile.tile.coords, texSize);
                    Vector3[] quadCoords = getQuad(new Vector3(tile.dest.X-32, -500, tile.dest.Y-32), Vector3.Up, Vector3.Forward, tile.dest.Width, tile.dest.Height);
                    for (int j = 0; j < 4; j++)
                    {
                        //v[i * 4 + j].Normal = Vector3.Backward;
                        v[i * 4 + j].Position = quadCoords[j];
                        v[i * 4 + j].TextureCoordinate = texCoords[j];
                    }
                    i++;
                    if (i == len)
                        break;
                }

                verts.SetData<VertexPositionTexture>(v);
                inds.SetData<int>(ins);
            }
            public void render(GraphicsDevice dev, Matrix view)
            {
                Matrix v = Matrix.CreateLookAt(new Vector3(20, 1, 0), new Vector3(20, -1, 0), Vector3.Forward);
                v.Translation = view.Translation;
                quadEffect.View = v;
                foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    dev.Indices = inds;
                    dev.SetVertexBuffer(verts);
                    dev.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, verts.VertexCount, 0, inds.IndexCount / 3);//inds.IndexCount / 3);
                    //dev.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, verts, 0, 4, inds, 0, 2);
                }
            }
            private Vector3[] getQuad(Vector3 origin, Vector3 normal, Vector3 up, float width, float height)
            {
                Vector3[] v = new Vector3[4];
                Vector3 Left = Vector3.Cross(normal, up);
                Vector3 uppercenter = (up * height / 2) + origin;
                Vector3 UpperLeft = uppercenter + (Left * width / 2);
                Vector3 UpperRight = uppercenter - (Left * width / 2);
                Vector3 LowerLeft = UpperLeft - (up * height);
                Vector3 LowerRight = UpperRight - (up * height);

                v[0] = LowerLeft;
                v[1] = UpperLeft;
                v[2] = LowerRight;
                v[3] = UpperRight;

                return v;
            }
            private Vector2[] getTexCoords(Rectangle coords, Vector2 size)
            {
                float left = coords.Left / size.X;
                float top = coords.Top / size.Y;
                float bottom = coords.Bottom / size.Y;
                float right = coords.Right / size.X;
                Vector2[] v = new Vector2[4];
                v[0] = new Vector2(left, top);
                v[1] = new Vector2(right, top);
                v[2] = new Vector2(left, bottom);
                v[3] = new Vector2(right, bottom);
                return v;
            }
        }
        class MapLayer
        {
            //int zIndex; //zindex of this layer
            String name; //name of layer
            int width, height; //width and height of layer in tiles
            public Dictionary<TileSet, List<LayerTile>> tileRectsBySet;
            List<MapLayerSet> mapLayerSets;

            public MapLayer()
            {
                tileRectsBySet = new Dictionary<TileSet, List<LayerTile>>();
                mapLayerSets = new List<MapLayerSet>();
            }
            public void LoadLayer(XmlElement layerXML, Map map, GraphicsDevice dev)
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
                foreach (TileSet t in tileRectsBySet.Keys)
                {
                    mapLayerSets.Add(new MapLayerSet(t, this, dev));
                }
            }
            public void draw(SpriteBatch batch, Matrix view)
            {
                /*int i = 0;
                foreach (var entry in tileRectsBySet)
                {
                    foreach (LayerTile t in entry.Value)
                    {
                        i++;
                        batch.Draw(entry.Key.tex, t.dest, t.tile.coords, Color.White);
                    }
                }
                i = 0;*/
                foreach (MapLayerSet l in mapLayerSets)
                {
                    l.render(batch.GraphicsDevice, view);
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
            filename = App.Path + filename;
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
                    l.LoadLayer(layerXML, this, dev);
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
            batch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, cam.Transform);
            foreach (MapLayer l in layers)
            {
                l.draw(batch, cam.Transform);
            }
            batch.End();
        }
    }
}
