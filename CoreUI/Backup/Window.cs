using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MTV3D65;
using CoreUI.Elements;

namespace CoreUI
{
    public class Window : ContentElement
    {
        public event ElementEventHandler Loaded;
        public event ElementEventHandler Closed;
        public event ElementEventHandler Closing;
        public enum WindowState { Maximized, Minimized, Normal };
        public enum WindowStyle { Normal, None };
        public bool HitTestUnFocused = false;
        private Visuals.WindowChrome mWindowChrome;
        private WindowStyle mStyle = WindowStyle.Normal;
        private WindowState mState = WindowState.Normal;

        //Flashing
        private bool Flashing = false;
        private float FlashTick = 0;
        private int FlashNum = 0;

        private TVRenderSurface rs;
        private String mTitle;
        private bool Moving = false;

        public Window()
        {
            Background = Color.White.ToArgb();
            mWindowChrome = new CoreUI.Visuals.WindowChrome(this);
            if (Loaded != null)
                Loaded(this);
        }
        protected internal override void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (CoreUIEngine.Windows[0] != this)
            {
                CoreUIEngine.Windows.Remove(this);
                CoreUIEngine.Windows.Insert(0, this);
                InvalidateVisual();
            }
            if (Style == WindowStyle.Normal)
                if (mWindowChrome.TitleBarHitTest(e.X, e.Y))
                    Move();
            base.OnMouseDown(sender, e);
        }
        public WindowStyle Style
        {
            get
            {
                return mStyle;
            }
            set
            {
                mStyle = value;
                InvalidateVisual();
            }
        }
        public void Center()
        {
            Position = new Point(((CoreUIEngine.mWidth / 2) - (Bounds.Width / 2)), ((CoreUIEngine.mHeight / 2) - (Bounds.Height / 2)));
        }
        protected internal override void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Moving)
                Moving = false;
            base.OnMouseUp(sender, e);
        }
        public void Move()
        {
            if (State == WindowState.Normal)
                Moving = true;
        }
        public void Flash()
        {
            Flashing = true;
            FlashNum = 0;
            FlashTick = 0;
            mWindowChrome.Flash = true;
        }
        protected internal override void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Moving)
                Reposition(e.X, e.Y);
            base.OnMouseMove(sender, e);
        }
        private void Reposition(int relX, int relY)
        {
            Position = new Point(Position.X + relX, Position.Y + relY);
        }
        public void Maximize()
        {
            if (State != WindowState.Maximized)
                State = WindowState.Maximized;
            else
                State = WindowState.Normal;
        }
        protected internal override void CheckPositioning()
        {
            CheckState();
        }
        public WindowState State
        {
            get
            {
                return mState;
            }
            set
            {
                mState = value;
                CheckState();
                OnSizeChanged(this);
            }
        }
        public override Rectangle InnerBounds
        {
            get
            {
                return new Rectangle(0, 0, Bounds.Width, Bounds.Height);
            }
        }
        private void CheckState()
        {
            switch (mState)
            {
                case WindowState.Normal:
                    mRealBounds = mBounds;
                    break;
                case WindowState.Maximized:
                    Point max = CoreUIEngine.mSize();
                    if (Style == WindowStyle.Normal)
                        mRealBounds = new Rectangle(8, 27, max.X - 16, max.Y - 35);
                    if (Style == WindowStyle.None)
                        mRealBounds = new Rectangle(0, 0, max.X, max.Y);
                    break;
                case WindowState.Minimized:
                    mRealBounds = mBounds;
                    break;
            }
            InvalidateVisual();
        }
        public override SizeMode SizeMode
        {
            get
            {
                return base.SizeMode;
            }
            set
            {
            }
        }
        public void Close()
        {
            if (Closing != null)
                Closing(this);
            Hide();
            if (Closed != null)
                Closed(this);
        }
        public String Title
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
                InvalidateVisual();
            }
        }
        public Point ConvertMouse(int X, int Y)
        {
            return new Point(X - Bounds.X, Y - Bounds.Y);
        }
        protected internal override void HitTest(ref List<Element> retlist, float X, float Y)
        {
            base.HitTest(ref retlist, X, Y);
            if (Style == WindowStyle.Normal)
                mWindowChrome.HitTest(X, Y);
        }
        protected internal override Element HitTest(float X, float Y)
        {
            Element e = base.HitTest(X, Y);
            if (Style == WindowStyle.Normal)
            {
                if (e != null)
                    return e;
                e = mWindowChrome.HitTest(X, Y);
                if (e != null)
                    return e;
            }
            return e;
        }
        protected internal override void OnPositionChanged(object sender)
        {
            base.OnPositionChanged(sender);
            mWindowChrome.Resize();
        }
        protected internal override void OnSizeChanged(object sender)
        {
            base.OnSizeChanged(sender);
            if (rs != null)
                rs.Destroy();
            rs = CoreUIEngine.mScene.CreateAlphaRenderSurface((int)Bounds.Width, (int)Bounds.Height);
            rs.SetBackgroundColor(new TV_COLOR(0, 0, 0, 0).GetIntColor());
            mWindowChrome.Resize();
        }
        public void Show()
        {
            CoreUIEngine.AddWindow(this);
            InvalidateVisual();
        }
        public void ShowDialog()
        {
            CoreUIEngine.AddModal(this);
            InvalidateVisual();
        }
        public void Hide()
        {
            CoreUIEngine.RemoveWindow(this);
            CoreUIEngine.ForceRedraw = true;
        }
        protected internal void TexRender()
        {
            if (State != WindowState.Minimized)
            {
                if (Flashing)
                {
                    FlashTick += CoreUIEngine.mTV.TimeElapsed();
                    if (FlashTick > 500)
                    {
                        FlashTick = 0;
                        mWindowChrome.Flash = !mWindowChrome.Flash;
                        FlashNum++;
                    }
                    if (FlashNum > 4)
                        StopFlash();
                }
                if (Style == WindowStyle.Normal)
                    mWindowChrome.Render();
                CoreUIEngine.mScreen2D.Draw_Texture(rs.GetTexture(), Bounds.Left, Bounds.Top, Bounds.Right - 1, Bounds.Bottom - 1);
            }
        }
        private void StopFlash()
        {
            Flashing = false;
            FlashNum = 0;
            FlashTick = 0;
            mWindowChrome.Flash = false;
        }
        protected internal override Point GetScreenCoords()
        {
            if (Parent != null)
                return Parent.GetScreenCoords();
            else
                return Position;
        }
        protected internal override void Render()
        {
            if (Invalidated)
            {
                rs.StartRender();
                CoreUIEngine.mScreen2D.Action_Begin2D();
                CoreUIEngine.mScreen2D.Draw_FilledBox(0, 0, Bounds.Width, Bounds.Height, Background);
                base.Render();
                CoreUIEngine.mScreen2D.Action_End2D();
                rs.EndRender();
            }
            if (Flashing)
                InvalidateVisual();
        }
    }
}
