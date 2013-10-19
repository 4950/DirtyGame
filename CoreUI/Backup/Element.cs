using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CoreUI
{
    public abstract class Element : Visual
    {
        public delegate void ElementEventHandler(object sender);

        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseEnter;
        public event MouseEventHandler MouseCaptureChanged;
        public event MouseEventHandler MouseLeave;
        public event MouseEventHandler MouseScroll;
        public event MouseEventHandler MouseMove;

        public event ElementEventHandler GotFocus;
        public event ElementEventHandler LostFocus;

        private bool mIsEnabled = true;
        private bool mIsMouseOver = false;
        private bool mIsFocused = false;

        protected int mFontInt = 0;
        private Font mFont;

        protected Visual mBackgroundVisual;

        public Visual BackgroundVisual
        {
            get
            {
                return mBackgroundVisual;
            }
            set
            {
                mBackgroundVisual = value;
                if (mBackgroundVisual != null)
                    mBackgroundVisual.Parent = this;
                InvalidateVisual();
            }
        }
        public bool IsEnabled
        {
            get { return mIsEnabled; }
            set { mIsEnabled = value; InvalidateVisual(); }
        }
        public bool IsFocused
        {
            get { return mIsFocused; }
        }
        public bool IsMouseOver
        {
            get { return mIsMouseOver; }
            set { mIsMouseOver = value; InvalidateVisual(); }
        }
        public virtual Font Font
        {
            get
            {
                return mFont;
            }
            set
            {
                mFont = value;
                if (mFont != null)
                    mFontInt = FontManager.GetFontInt(value);
                else
                    mFontInt = 0;
                InvalidateVisual();
            }
        }
        protected internal override void Render()
        {
            if (mBackgroundVisual != null && BackgroundVisual.Visibility == Visibility.Visible)
                mBackgroundVisual.Render();
            base.Render();
        }
        protected internal virtual Element HitTest(float X, float Y)
        {
            if (IsEnabled)
                if (X > Bounds.Left && X < Bounds.Right && Y > Bounds.Top && Y < Bounds.Bottom)
                    return this;
            return null;
        }
        protected internal virtual void HitTest(ref List<Element> retlist, float X, float Y)
        {
            if (HitTest(X, Y) != null)
                if (!retlist.Contains(this))
                    retlist.Add(this);
        }
        protected internal override void OnPositionChanged(object sender)
        {
            base.OnPositionChanged(sender);
            if (mBackgroundVisual != null)
                mBackgroundVisual.CheckPositioning();
        }
        protected internal override void OnSizeChanged(object sender)
        {
            base.OnSizeChanged(sender);
            if (mBackgroundVisual != null)
                mBackgroundVisual.CheckPositioning();
        }
        protected internal virtual void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (MouseDown != null)
                MouseDown(sender, e);
        }
        protected internal virtual void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (MouseMove != null)
                MouseMove(sender, e);
        }
        protected internal virtual void OnMouseEnter(object sender, MouseEventArgs e)
        {
            IsMouseOver = true;
            if (MouseEnter != null)
                MouseEnter(sender, e);
        }
        protected internal virtual void OnMouseLeave(object sender, MouseEventArgs e)
        {
            IsMouseOver = false;
            if (MouseLeave != null)
                MouseLeave(sender, e);
        }
        protected internal virtual void OnLostFocus(object sender)
        {
            if (LostFocus != null)
                LostFocus(sender);
            if (Parent != null && Parent.GetType().IsSubclassOf(typeof(Element)))
                ((Element)Parent).OnLostFocus(sender);
            mIsFocused = false;
        }
        protected internal virtual void OnGotFocus(object sender)
        {
            if (GotFocus != null)
                GotFocus(sender);
            if (Parent != null && Parent.GetType().IsInstanceOfType(typeof(Element)))
                ((Element)Parent).OnGotFocus(sender);
            mIsFocused = true;
        }
        protected internal virtual void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (MouseUp != null)
                MouseUp(sender, e);
        }
    }
}
