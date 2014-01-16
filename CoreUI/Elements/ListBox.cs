using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace CoreUI.Elements
{
    public class Listbox : Element
    {
        public event ElementEventHandler SelectedIndexChanged;
        private List<Object> ListBoxObjects = new List<object>();
        private List<String> DisplayTexts = new List<string>();
        private List<Label> DisplayLabels = new List<Label>();
        private int DisplayPosition = 0;
        private int DisplayNum;
        private int DisplayHeight;
        private Scrollbar mScrollBar;
        private bool ShowScrollBar = false;
        private int mSelectedIndex = -1;
        private static IUIColor BorderColor = CoreUIEngine.mDrawEngine.CreateColor(Color.Black);

        public Listbox()
        {
            mScrollBar = new Scrollbar();
            mScrollBar.Parent = this;
            mScrollBar.Value = 0;
            mScrollBar.Scroll += new ElementEventHandler(mScrollBar_Scroll);
            Background = CoreUIEngine.mDrawEngine.CreateColor(Color.White);
            Middleground = CoreUIEngine.mDrawEngine.CreateColor(Color.LightCyan);
        }

        void mScrollBar_Scroll(object sender)
        {
            if (ShowScrollBar)
            {
                DisplayPosition = (int)mScrollBar.Value;
                CalculateLabels();
                InvalidateVisual();
            }
        }
        protected internal override Element HitTest(float X, float Y)
        {
            Element e = mScrollBar.HitTest(X, Y);
            if (e != null)
                return e;
            foreach (Label l in DisplayLabels)
            {
                e = l.HitTest(X, Y);
                if (e != null)
                    return e;
            }
            return base.HitTest(X, Y);
        }
        protected internal override void Render()
        {
            base.Render();
            CoreUIEngine.mDrawEngine.Draw_FilledBox(Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Bottom, Background);
            CoreUIEngine.mDrawEngine.Draw_Box(Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Bottom, BorderColor);
            foreach (Label l in DisplayLabels)
                l.Render();
            if (ShowScrollBar)
                mScrollBar.Render();
        }
        protected internal override void OnSizeChanged(object sender)
        {
            CalculateText();
            base.OnSizeChanged(sender);
        }
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                CalculateText();
            }
        }
        public int SelectedIndex
        {
            get
            {
                return mSelectedIndex;
            }
            set
            {
                mSelectedIndex = value;
                if (mSelectedIndex > ListBoxObjects.Count - 1 || mSelectedIndex < 0 || ListBoxObjects.Count == 0)
                    mSelectedIndex = -1;
                foreach (Label l in DisplayLabels)
                    l.Background = CoreUIEngine.mDrawEngine.CreateColor(0, 0, 0, 0);
                if (mSelectedIndex != -1 && mSelectedIndex >= DisplayPosition && mSelectedIndex <= DisplayPosition + DisplayNum)
                    DisplayLabels[mSelectedIndex].Background = Middleground;
                InvalidateVisual();
                if (SelectedIndexChanged != null)
                    SelectedIndexChanged(this);
            }
        }
        public Object SelectedItem
        {
            get
            {
                return ListBoxObjects[mSelectedIndex];
            }
        }
        public void AddItem(Object o)
        {
            ListBoxObjects.Add(o);
            DisplayTexts.Add(o.ToString());
            CalculateScrollbar();
        }
        public void RemoveItem(Object o)
        {
            if (ListBoxObjects.Contains(o))
            {
                DisplayTexts.RemoveAt(ListBoxObjects.IndexOf(o));
                ListBoxObjects.Remove(o);
            }
            CalculatePosition();
        }
        public void RemoveAllItems()
        {
            ListBoxObjects.Clear();
            DisplayTexts.Clear();
            CalculatePosition();
        }
        private void CalculatePosition()
        {
            if (ListBoxObjects.Count < DisplayNum)
                DisplayPosition = 0;
            else if (0 < (ListBoxObjects.Count - (DisplayPosition + DisplayNum)))
                DisplayPosition = ListBoxObjects.Count - DisplayNum;

            CalculateScrollbar();
        }
        private void CalculateText()
        {

            PointF size = CoreUIEngine.mDrawEngine.getTextSize("AL/|^$", mFontInt);
            DisplayHeight = (int)Math.Ceiling(size.Y);
            DisplayNum = (int)Math.Floor((Bounds.Height / (double)DisplayHeight));

            CalculatePosition();
        }
        private int CalcMaxHeight()
        {
            CalculateText();
            return DisplayHeight * ListBoxObjects.Count + 1;
        }
        public override Rectangle RequestedSize
        {
            get
            {
                Rectangle r = Bounds;
                r.Height = CalcMaxHeight();
                return r;
            }
        }
        private void CalculateScrollbar()
        {
            if (DisplayTexts.Count - DisplayNum > 0)
            {
                ShowScrollBar = true;
                mScrollBar.Position = new Point(Bounds.Right - mScrollBar.Size.X - 1, Bounds.Top + 1);
                mScrollBar.Size = new Point(18, Bounds.Height - 2);
                mScrollBar.Min = 0;
                mScrollBar.Max = DisplayTexts.Count;
                mScrollBar.BarSize = DisplayNum;
            }
            else
                ShowScrollBar = false;

            CalculateLabels();
        }
        public override IUIColor Foreground
        {
            get
            {
                return base.Foreground;
            }
            set
            {
                base.Foreground = value;
                foreach (Label l in DisplayLabels)
                    l.Foreground = Foreground;
            }
        }
        private void CalculateLabels()
        {
            if (DisplayLabels.Count > DisplayNum)
            {
                int c = DisplayLabels.Count - DisplayNum;
                DisplayLabels.RemoveRange(0, c);
            }
            if (DisplayLabels.Count < DisplayNum)
            {
                int c = DisplayNum - DisplayLabels.Count;
                for (int i = 0; i < c; i++)
                {
                    Label l = new Label();
                    l.Background = CoreUIEngine.mDrawEngine.CreateColor(0, 0, 0, 0);
                    l.Foreground = Foreground;
                    l.MouseDown += new System.Windows.Forms.MouseEventHandler(l_MouseDown);
                    l.Size = new Point(Bounds.Width - 4, DisplayHeight - 4);
                    l.Parent = this;
                    DisplayLabels.Add(l);
                }
            }
            for (int i = 0; i < DisplayNum; i++)
            {
                if ((DisplayPosition + i + 1) <= DisplayTexts.Count)
                {
                    DisplayLabels[i].Position = new Point(Bounds.Left + 2, Bounds.Top + 2 + (DisplayHeight * i));
                    DisplayLabels[i].Text = DisplayTexts[DisplayPosition + i];
                    DisplayLabels[i].Tag = DisplayPosition + i;
                }
            }
        }

        void l_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            SelectedIndex = ((int)((Label)sender).Tag);
        }
    }
}
