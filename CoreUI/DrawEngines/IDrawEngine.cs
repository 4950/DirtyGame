using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUI
{
    public interface IUITexture
    {
        void Delete();
    }
    public interface IUIFont
    {
    }
    public interface IUIRenderSurface
    {
        void resize(int width, int height);
    }
    public interface IUIColor
    {
    }
    public interface IDrawEngine
    {
        IUIRenderSurface CreateRenderSurface(int width, int height);
        IUIColor CreateColor(float r, float g, float b, float a);
        IUIColor CreateColor(float r, float g, float b);
        IUIColor CreateColor(int argb);
        IUIColor CreateColor(System.Drawing.Color color);
        IUITexture CreateTexture(byte[] data);
        IUITexture CreateTexture(String filename);
        void BeginDraw(IUIRenderSurface ren);
        void BeginDraw();
        void EndDraw();
        void setSize(int width, int height);
        void Draw_RS(IUIRenderSurface tex, int left, int top, int right, int bottom);
        void Draw_Texture(IUITexture tex, int left, int top, int right, int bottom);
        void Draw_FilledBox(int left, int top, int right, int bottom, IUIColor color);
        void Draw_FilledBox(int left, int top, int right, int bottom, IUIColor color1, IUIColor color2, IUIColor color3, IUIColor color4);
        void Draw_Box(int left, int top, int right, int bottom, IUIColor color);
        void Draw_Line(int left, int top, int right, int bottom, IUIColor color);
        void Draw_Line(int left, int top, int right, int bottom, IUIColor color1, IUIColor color2);
        void Draw_Default_Text(String text, int left, int top, IUIColor color);
        void Draw_Default_Text(String text, int left, int top, IUIColor color, IUIFont font);
        System.Drawing.PointF getTextSize(String text, IUIFont font);
    }
}
