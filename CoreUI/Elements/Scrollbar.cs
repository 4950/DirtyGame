using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CoreUI.Elements
{
    public class Scrollbar : Element
    {
        public event ElementEventHandler Scroll;

        private float mBarPercent;
        private float mBarSize = 50;
        private float mMin = 0;
        private float mMax = 100;
        private float mValue = 0;
        private Button mBar = new Button();
        private bool Scrolling = false;

        public Scrollbar()
        {
            mBar.Parent = this;
            mBar.ButtonChrome = new Visuals.ScrollbarVisual();
            mBar.MouseDown += new System.Windows.Forms.MouseEventHandler(mBar_MouseDown);
            mBar.MouseUp += new System.Windows.Forms.MouseEventHandler(mBar_MouseUp);
            mBar.MouseMove += new System.Windows.Forms.MouseEventHandler(mBar_MouseMove);
            CalculateBarSize();
        }

        void mBar_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Scrolling)
                Scrolling = false;
        }
        void mBar_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Scrolling)
                DoScroll(e.Y);
        }
        void mBar_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Scrolling = true;
        }
        protected internal override void Render()
        {
            base.Render();
            CoreUIEngine.mDrawEngine.Draw_FilledBox(Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Bottom, CoreUIEngine.mDrawEngine.CreateColor(Color.LightGray));
            mBar.Render();
        }
        private void DoScroll(int Y)
        {
            int posY = mBar.Position.Y + Y;
            if (posY + mBar.Size.Y > Bounds.Bottom)
                posY = Bounds.Bottom - mBar.Size.Y;
            if (posY < Bounds.Top)
                posY = Bounds.Top;
            mBar.Position = new Point(mBar.Position.X, posY);
            CalculateValue();
            if (Scroll != null)
                Scroll(this);
        }
        protected internal override Element HitTest(float X, float Y)
        {
            Element e = mBar.HitTest(X, Y);
            if (e != null)
                return e;
            return base.HitTest(X, Y);
        }
        protected override Rectangle Bounds
        {
            get
            {
                return base.Bounds;
            }
            set
            {
                value.Width = 18;
                base.Bounds = value;
                CalculateBarSize();
            }
        }
        public float Max
        {
            get
            {
                return mMax;
            }
            set
            {
                mMax = value;
                if (mMax <= mMin)
                    mMax = mMin + 1;
                CalculateBarSize();
            }
        }
        public float Min
        {
            get
            {
                return mMin;
            }
            set
            {
                mMin = value;
                if (mMin >= mMax)
                    mMin = mMax - 1;
                CalculateBarSize();
            }
        }
        public float Value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
                if (mValue > mMax)
                    mValue = mMax;
                if (mValue < mMin)
                    mValue = mMin;
                CalculateBarPosition();
            }
        }
        private void CalculateValue()
        {
            float totalspace = Bounds.Height - mBar.Size.Y;
            float space = mBar.Position.Y - Bounds.Top;
            float valpercent;
            if (totalspace > 0)
                valpercent = space / totalspace;
            else
                valpercent = 0;
            mValue = valpercent * (mMax - mMin - BarSize);
        }
        public float BarSize
        {
            get
            {
                return mBarSize;
            }
            set
            {
                mBarSize = value;
                if (mBarSize > mMax - mMin)
                    mBarSize = mMax - mMin;
                CalculateBarSize();
            }
        }
        private void CalculateBarSize()
        {
            mBarPercent = mBarSize / (mMax - mMin);
            mBar.Size = new Point(18, (int)(Bounds.Height * mBarPercent));
            CalculateBarPosition();
        }
        private void CalculateBarPosition()
        {
            float valpercent = Value / (mMax - mMin);
            float availspace = Bounds.Height - mBar.Size.Y;
            int pos = (int)(availspace * valpercent);
            mBar.Position = new Point(Bounds.Left, Bounds.Top + pos);
        }
    }
}
