using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CoreUI.DrawEngines
{
    public class MonoGameTexture : IUITexture
    {
        public Texture2D texture;
        public MonoGameTexture(Texture2D tex)
        {
            texture = tex;
        }
        public void Delete()
        {
            texture.Dispose();
        }
    }
    public class MonoGameFont : IUIFont
    {
        public SpriteFont font;
        public MonoGameFont(SpriteFont font)
        {
            this.font = font;
        }
    }
    public class MonoGameColor : IUIColor
    {
        public Color color;
        public MonoGameColor(Color c)
        {
            color = c;
        }
    }
    public class MonoGameRenderSurface : IUIRenderSurface
    {
        public RenderTarget2D target;
        public MonoGameRenderSurface(RenderTarget2D tar)
        {
            target = tar;
        }
        public void resize(int width, int height)
        {
            RenderTarget2D n = new RenderTarget2D(target.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            target.Dispose();
            target = n;
        }
    }

    public class MonoGameDrawEngine : IDrawEngine
    {
        SpriteBatch batch;
        GraphicsDevice device;
        Matrix? transform = null;
        ContentManager content;
        IUIRenderSurface cur;
        BasicEffect eff;
        SpriteFont defaultFont;
        Texture2D pixel;
        Texture2D gradQuad;
        Texture2D gradPixel;
        SamplerState sampler;

        public MonoGameDrawEngine(GraphicsDevice dev, ContentManager cont)
        {
            content = cont;
            device = dev;

            SamplerState sampler = new SamplerState();
            sampler.Filter = TextureFilter.Anisotropic;
            sampler.MaxAnisotropy = 16;

            batch = new SpriteBatch(device);

            pixel = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
            Color[] rectData = new Color[1];
            rectData[0] = Color.White;
            pixel.SetData(rectData);

            gradPixel = new Texture2D(device, 2, 1, false, SurfaceFormat.Color);
            rectData = new Color[2];
            rectData[0] = Color.Transparent;
            rectData[1] = Color.White;
            gradPixel.SetData(rectData);

            gradQuad = new Texture2D(device, 2, 2, false, SurfaceFormat.Color);
            rectData = new Color[4];
            rectData[0] = Color.Transparent;
            rectData[1] = Color.White;
            rectData[2] = Color.Transparent;
            rectData[3] = Color.Transparent;
            gradQuad.SetData(rectData);
        }
        public void SetDevice(GraphicsDevice dev)
        {
            this.device = dev;
        }
        public void setDefaultFont(SpriteFont font)
        {
            defaultFont = font;
        }
        public IUIRenderSurface CreateRenderSurface(int width, int height)
        {
            RenderTarget2D tar = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            return new MonoGameRenderSurface(tar);
        }
        public void BeginDraw(IUIRenderSurface ren)
        {
            cur = ren;
            MonoGameRenderSurface r = ren as MonoGameRenderSurface;
            if (r.target.GraphicsDevice.IsDisposed)
                r.target = new RenderTarget2D(device, r.target.Width, r.target.Height, false, SurfaceFormat.Color, DepthFormat.None);
            device.SetRenderTarget(r.target);
            device.Clear(Color.Transparent);
            BeginDraw();
        }
        public void setSize(int width, int height)
        {
            eff = new BasicEffect(device);
            eff.VertexColorEnabled = true;
            eff.Projection = Matrix.CreateOrthographicOffCenter
            (0, (float)device.Viewport.Width,     // left, right
            (float)device.Viewport.Width, 0,    // bottom, top
            1, 1000);
        }
        public IUIColor CreateColor(float r, float g, float b, float a)
        {
            Color c = new Color(r, g, b, a);
            return new MonoGameColor(c);
        }
        public IUIColor CreateColor(float r, float g, float b)
        {
            Color c = new Color(r, g, b);
            return new MonoGameColor(c);
        }
        public IUIColor CreateColor(int argb)
        {
            return CreateColor(System.Drawing.Color.FromArgb(argb));
        }
        public IUIColor CreateColor(System.Drawing.Color color)
        {
            Color c = new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            return new MonoGameColor(c);
        }
        public void Draw_RS(IUIRenderSurface tex, int left, int top, int right, int bottom)
        {
            MonoGameRenderSurface r = tex as MonoGameRenderSurface;
            batch.Draw(r.target, toRect(left, top, right, bottom), Color.White);
        }
        public IUITexture CreateTexture(byte[] data)
        {
            Texture2D tex = //new Texture2D(device, 0, 0);
            Texture2D.FromStream(device, new System.IO.MemoryStream(data));

            //tex.SetData(data);
            return new MonoGameTexture(tex);
        }
        public IUITexture CreateTexture(String filename)
        {
            Texture2D tex = content.Load<Texture2D>(filename);
            return new MonoGameTexture(tex);
        }
        public void setViewMatrix(Matrix? view)
        {
            transform = view;
        }
        public void BeginDraw()
        {
            if (batch.GraphicsDevice.IsDisposed)
                batch = new SpriteBatch(device);


            if (transform != null)
            {

                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, sampler, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, (Matrix)transform);
            }
            else
            {
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, sampler, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null);
            }

            //device.SamplerStates[0].MaxAnisotropy = 16;
        }
        public void EndDraw()
        {
            batch.End();
            if (cur != null)
            {
                device.SetRenderTarget(null);
                cur = null;
            }
        }
        private Rectangle toRect(int left, int top, int right, int bottom)
        {
            return new Rectangle(left, top, right - left, bottom - top);
        }
        public void Draw_Texture(IUITexture tex, int left, int top, int right, int bottom)
        {
            batch.Draw((tex as MonoGameTexture).texture, toRect(left, top, right, bottom), Color.White);
        }

        public void Draw_FilledBox(int left, int top, int right, int bottom, IUIColor color)
        {
            batch.Draw(pixel, toRect(left, top, right, bottom), (color as MonoGameColor).color);
        }

        public void Draw_FilledBox(int left, int top, int right, int bottom, IUIColor color1, IUIColor color2, IUIColor color3, IUIColor color4)
        {

            //device.SamplerStates[0].Filter = TextureFilter.Anisotropic;

            batch.Draw(pixel, toRect(left, top, right, bottom), (color1 as MonoGameColor).color);
            batch.Draw(gradQuad, toRect(left, top, right, bottom), (color2 as MonoGameColor).color);
            batch.Draw(gradQuad, toRect(left, top, right, bottom), null, (color3 as MonoGameColor).color, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            batch.Draw(gradQuad, toRect(left, top, right, bottom), null, (color4 as MonoGameColor).color, 0, Vector2.Zero, SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0);

            //device.SamplerStates[0].Filter = TextureFilter.Point;

        }

        public void Draw_Box(int left, int top, int right, int bottom, IUIColor color)
        {
            Draw_Line(left, top, right, top, color);
            Draw_Line(right, top, right, bottom, color);
            Draw_Line(left, bottom, right, bottom, color);
            Draw_Line(left, top, left, bottom, color);
        }

        public void Draw_Line(int left, int top, int right, int bottom, IUIColor color)
        {
            int width = 1;
            Vector2 begin = new Vector2(left, top);
            Vector2 end = new Vector2(right, bottom);

            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;

            //device.SamplerStates[0].Filter = TextureFilter.Anisotropic;

            batch.Draw(pixel, r, null, (color as MonoGameColor).color, angle, Vector2.Zero, SpriteEffects.None, 0);

            //device.SamplerStates[0].Filter = TextureFilter.Point;
        }

        public void Draw_Line(int left, int top, int right, int bottom, IUIColor color1, IUIColor color2)
        {

            int width = 1;
            Vector2 begin = new Vector2(left, top);
            Vector2 end = new Vector2(right, bottom);


            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;

            //device.SamplerStates[0].Filter = TextureFilter.Anisotropic;

            batch.Draw(pixel, r, null, (color1 as MonoGameColor).color, angle, Vector2.Zero, SpriteEffects.None, 0);
            batch.Draw(gradPixel, r, null, (color2 as MonoGameColor).color, angle, Vector2.Zero, SpriteEffects.None, 0);

            //device.SamplerStates[0].Filter = TextureFilter.Point;
        }

        public void Draw_Default_Text(string text, int left, int top, IUIColor color)
        {
            if (defaultFont != null && text != null && text != "")
                batch.DrawString(defaultFont, text, new Vector2(left, top), (color as MonoGameColor).color);

        }
        public System.Drawing.PointF getTextSize(String text, IUIFont font)
        {
            SpriteFont f = null;
            if (font != null)
                f = (font as MonoGameFont).font;
            Vector2 s = f == null ? defaultFont.MeasureString(text) : f.MeasureString(text);
            return new System.Drawing.PointF(s.X, s.Y);
        }
        public void Draw_Default_Text(string text, int left, int top, IUIColor color, IUIFont font)
        {
            SpriteFont f = null;
            if (font != null)
                f = (font as MonoGameFont).font;
            if (defaultFont != null && text != "")
                batch.DrawString(f == null ? defaultFont : f, text, new Vector2(left, top), (color as MonoGameColor).color);

        }
    }
}
