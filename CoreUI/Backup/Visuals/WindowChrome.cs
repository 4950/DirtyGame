using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CoreUI.Elements;
using MTV3D65;

namespace CoreUI.Visuals
{
    public class WindowChrome : Visual
    {
        private Button mClose;
        private Button mMaximize;
        private int ChromeColorNorm = new TV_COLOR(.227f, .586f, .945f, .5f).GetIntColor();
        private int ChromeColorFlash = new TV_COLOR(1, 0, 0, .5f).GetIntColor();
        private int GrayColor = Color.Gray.ToArgb();
        private int BlackColor = Color.Black.ToArgb();
        public bool Flash;

        public WindowChrome(Window w)
        {
            Parent = w;
            this.Size = w.Size;
            this.Position = w.Position;

            mClose = new Button();
            mClose.Size = new Point(22, 22);
            mClose.Parent = this;
            mClose.Click += new Button.ClickEventHandler(mClose_Click);
            Visuals.XVisual xv = new CoreUI.Visuals.XVisual();
            xv.SizeMode = SizeMode.Fill;
            mClose.BackgroundVisual = xv;

            mMaximize = new Button();
            mMaximize.Size = new Point(22, 22);
            mMaximize.Parent = this;
            mMaximize.Click += new Button.ClickEventHandler(mMaximize_Click);
            Visuals.MinVisual mv = new CoreUI.Visuals.MinVisual();
            mv.SizeMode = SizeMode.Fill;
            mMaximize.BackgroundVisual = mv;
        }
        private int ChromeColor
        {
            get
            {
                if (Flash)
                    return ChromeColorFlash;
                else
                    return ChromeColorNorm;
            }
        }
        void mMaximize_Click(object sender)
        {
            ((Window)Parent).Maximize();
        }
        void mClose_Click(object sender)
        {
            ((Window)Parent).Close();
        }
        public bool TitleBarHitTest(int X, int Y)
        {
            if (X > Bounds.Left - 8 && X < Bounds.Right + 8 && Y > Bounds.Top - 27 && Y < Bounds.Top)
                return true;
            return false;
        }
        protected internal void HitTest(ref List<Element> retlist, float X, float Y)
        {
            mClose.HitTest(ref retlist, X, Y);
            mMaximize.HitTest(ref retlist, X, Y);
            if (X > Bounds.Left - 8 && X < Bounds.Right + 8 && Y > Bounds.Top - 27 && Y < Bounds.Bottom + 8)
            {
                if (!retlist.Contains((Window)Parent))
                    retlist.Add((Window)Parent);
            }
        }
        protected internal Element HitTest(float X, float Y)
        {
            Element e = mClose.HitTest(X, Y);
            if (e != null)
                return e;
            e = mMaximize.HitTest(X, Y);
            if (e != null)
                return e;
            if (X > Bounds.Left - 8 && X < Bounds.Right + 8 && Y > Bounds.Top - 27 && Y < Bounds.Bottom + 8)
                return (Window)Parent;
            return null;
        }
        public void Resize()
        {
            Size = Parent.Size;
            Position = Parent.Position;
            mClose.Position = new Point(Bounds.Right - 24, Bounds.Top - 25);
            mMaximize.Position = new Point(Bounds.Right - 49, Bounds.Top - 25);
        }
        protected internal override void Render()
        {
            base.Render();
            CoreUIEngine.mScreen2D.Draw_FilledBox(Bounds.Left - 8, Bounds.Top - 27, Bounds.Right + 8, Bounds.Top - 1, ChromeColor);//Top
            CoreUIEngine.mScreen2D.Draw_FilledBox(Bounds.Left - 8, Bounds.Bottom + 1, Bounds.Right + 8, Bounds.Bottom + 8, ChromeColor);//Bottom
            CoreUIEngine.mScreen2D.Draw_FilledBox(Bounds.Left - 8, Bounds.Top, Bounds.Left - 1, Bounds.Bottom, ChromeColor);//Left
            CoreUIEngine.mScreen2D.Draw_FilledBox(Bounds.Right + 1, Bounds.Top, Bounds.Right + 8, Bounds.Bottom, ChromeColor);//Right
            CoreUIEngine.mScreen2D.Draw_Box(Bounds.Left - 1, Bounds.Top - 1, Bounds.Right + 1, Bounds.Bottom + 1, GrayColor);
            CoreUIEngine.mScreen2D.Action_End2D();
            CoreUIEngine.mText.Action_BeginText();
            CoreUIEngine.mText.NormalFont_DrawText(((Window)Parent).Title, Bounds.Left + 2, Bounds.Top - 22, Parent.Foreground);
            CoreUIEngine.mText.Action_EndText();
            CoreUIEngine.mScreen2D.Action_Begin2D();
            mClose.Render();
            mMaximize.Render();
        }
    }
}
