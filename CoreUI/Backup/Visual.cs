using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CoreUI;
using CoreUI.Brushes;

namespace CoreUI
{

    public enum Visibility { Visible, Hidden };
    public enum SizeMode { Fill, Normal };

    public abstract class Visual
    {
        private Visual mParent;
        public delegate void VisualEventHandler(object sender);
        public event VisualEventHandler SizeChanged;
        public event VisualEventHandler PositionChanged;
        private SizeMode mSizeMode = SizeMode.Normal;
        private Visibility mVisibility = Visibility.Visible;
        private bool mInvalidated = true;
        protected Rectangle mBounds;
        protected Rectangle mRealBounds;
        private Object mTag;
        private int mForeground;
        private int mBackground;
        private int mMiddleground;

        public Visual()
        {
            Bounds = new Rectangle(0, 0, 0, 0);
            Background = Color.LightBlue.ToArgb();
            Foreground = Color.Black.ToArgb();
            Middleground = Color.White.ToArgb();
        }
        public Object Tag
        {
            get
            {
                return mTag;
            }
            set
            {
                mTag = value;
            }
        }
        public virtual int Middleground
        {
            get
            {
                return mMiddleground;
            }
            set
            {
                mMiddleground = value;
                InvalidateVisual();
            }
        }
        public virtual int Background
        {
            get
            {
                return mBackground;
            }
            set
            {
                mBackground = value;
                InvalidateVisual();
            }
        }
        protected internal Point ScreenCoords
        {
            get
            {
                Point coords = GetScreenCoords();
                coords.X += Bounds.X;
                coords.Y += Bounds.Y;
                return coords;
            }
        }
        protected internal virtual Point GetScreenCoords()
        {
            if (Parent != null)
                return Parent.GetScreenCoords();
            else
                return new Point();
        }
        public virtual int Foreground
        {
            get
            {
                return mForeground;
            }
            set
            {
                mForeground = value;
                InvalidateVisual();
            }
        }
        protected virtual Rectangle Bounds
        {
            get
            {
                return mRealBounds;
            }
            set
            {
                mBounds = value;
                CheckPositioning();
            }
        }
        public virtual Rectangle RequestedSize
        {
            get
            {
                return Bounds;
            }
        }
        public virtual Rectangle InnerBounds
        {
            get
            {
                return Bounds;
            }
        }
        protected internal virtual void Render()
        {
            mInvalidated = false;
        }
        public void InvalidateVisual()
        {
            mInvalidated = true;
            if (Parent != null)
                Parent.InvalidateVisual();
        }
        protected internal virtual void OnSizeChanged(object sender)
        {
            if (SizeChanged != null)
                SizeChanged(sender);
        }
        public virtual Visual Parent
        {
            get
            {
                return mParent;
            }
            set
            {
                if (Parent != null)
                    mParent.SizeChanged -= new VisualEventHandler(mParent_SizeChanged);
                mParent = value;
                mParent.SizeChanged += new VisualEventHandler(mParent_SizeChanged);
                CheckPositioning();
            }
        }
        void mParent_SizeChanged(object sender)
        {
            CheckPositioning();
        }
        protected internal virtual void CheckPositioning()
        {
            if (mParent != null && mSizeMode == SizeMode.Fill)
                mRealBounds = Parent.InnerBounds;
            else
                mRealBounds = mBounds;
            InvalidateVisual();
        }
        public virtual Visibility Visibility
        {
            get { return mVisibility; }
            set
            {
                mVisibility = value;
                InvalidateVisual();
            }
        }
        public virtual SizeMode SizeMode
        {
            get
            {
                return mSizeMode;
            }
            set
            {
                mSizeMode = value;
                CheckPositioning();
            }
        }
        protected internal virtual void OnPositionChanged(object sender)
        {
            if (PositionChanged != null)
                PositionChanged(sender);
        }
        public bool Invalidated
        {
            get
            {
                return mInvalidated;
            }
        }
        public Point Position
        {
            get
            {
                return new Point(Bounds.X, Bounds.Y);
            }
            set
            {
                Rectangle tmpBounds = Bounds;
                tmpBounds.X = value.X;
                tmpBounds.Y = value.Y;
                Bounds = tmpBounds;
                OnPositionChanged(this);
            }
        }
        public Point Size
        {
            get
            {
                return new Point(Bounds.Width, Bounds.Height);
            }
            set
            {
                Rectangle tmpBounds = Bounds;
                tmpBounds.Width = value.X;
                tmpBounds.Height = value.Y;
                Bounds = tmpBounds;
                OnSizeChanged(this);
            }
        }
    }
}
