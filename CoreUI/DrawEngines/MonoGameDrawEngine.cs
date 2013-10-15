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
            RenderTarget2D n = new RenderTarget2D(target.GraphicsDevice, width, height);
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

        public MonoGameDrawEngine(GraphicsDevice dev, ContentManager cont)
        {
            content = cont;
            device = dev;
            batch = new SpriteBatch(device);
            
        }
        public IUIRenderSurface CreateRenderSurface(int width, int height)
        {
            RenderTarget2D tar = new RenderTarget2D(device, width, height);
            return new MonoGameRenderSurface(tar);
        }
        public void BeginDraw(IUIRenderSurface ren)
        {
            cur = ren;
            device.SetRenderTarget((ren as MonoGameRenderSurface).target);
            device.Clear(Color.Transparent);
            BeginDraw();
        }
        public void setSize(int width, int height)
        {
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
            Color c = new Color(color.R, color.G, color.B, color.A);
            return new MonoGameColor(c);
        }
        public void Draw_RS(IUIRenderSurface tex, int left, int top, int right, int bottom)
        {
            MonoGameRenderSurface r = tex as MonoGameRenderSurface;
            batch.Draw(r.target, toRect(left, top, right, bottom), Color.White);
        }
        public IUITexture CreateTexture(byte[] data)
        {
            Texture2D tex = new Texture2D(device, 0, 0);
            tex.SetData(data);
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
            if (transform != null)
            {
                batch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, (Matrix)transform);
            }
            else
            {
                batch.Begin();
            }
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
            Texture2D rect = new Texture2D(device, 2, 2, false, SurfaceFormat.Color);
            Color[] rectData = new Color[4];
            for (int i = 0; i < 4; i++)
                rectData[i] = (color as MonoGameColor).color;
            rect.SetData(rectData);
            batch.Draw(rect, toRect(left, top, right, bottom), Color.White);
        }

        public void Draw_FilledBox(int left, int top, int right, int bottom, IUIColor color1, IUIColor color2, IUIColor color3, IUIColor color4)
        {
            Texture2D rect = new Texture2D(device, 2, 2, false, SurfaceFormat.Color);
            Color[] rectData = new Color[4];
            rectData[0] = (color1 as MonoGameColor).color;
            rectData[1] = (color2 as MonoGameColor).color;
            rectData[2] = (color3 as MonoGameColor).color;
            rectData[3] = (color4 as MonoGameColor).color;
            rect.SetData(rectData);
            batch.Draw(rect, toRect(left, top, right, bottom), Color.White);
            //throw new NotImplementedException();
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
            Draw_Line(left, top, right, bottom, color, color);
        }

        public void Draw_Line(int left, int top, int right, int bottom, IUIColor color1, IUIColor color2)
        {
            Texture2D line = new Texture2D(device, 2, 1, false, SurfaceFormat.Color);
            Color[] lineData = new Color[2];
            lineData[0] = (color1 as MonoGameColor).color;
            lineData[1] = (color2 as MonoGameColor).color;
            line.SetData(lineData);

            int width = 1;
            Vector2 begin = new Vector2(left, top);
            Vector2 end = new Vector2(right, bottom);
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            batch.Draw(line, r, null, Color.White, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void Draw_Default_Text(string text, int left, int top, IUIColor color)
        {
            //throw new NotImplementedException();
        }

        public void Draw_Default_Text(string text, int left, int top, IUIColor color, IUIFont font)
        {
            //throw new NotImplementedException();
        }
    }
}
