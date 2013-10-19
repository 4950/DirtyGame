using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CoreUI.Elements
{
    public class ComboBox : Element
    {
        private Button mButton;
        private Label mLabel;
        private Window mWindow;
        private Listbox mListBox;
        private List<Object> ComboBoxObjects;
        private List<String> DisplayTexts;
        private int mSelectedIndex = -1;
        private static IUIColor BlackColor = CoreUIEngine.mDrawEngine.CreateColor(Color.Black);

        public ComboBox()
        {
            Background = CoreUIEngine.mDrawEngine.CreateColor(Color.White);
            ComboBoxObjects = new List<object>();
            DisplayTexts = new List<string>();
            mButton = new Button();
            mButton.Parent = this;
            mButton.Click += new Button.ClickEventHandler(mButton_Click);
            mLabel = new Label();
            mLabel.Parent = this;
            mLabel.TextMode = LabelTextMode.Truncate;
            mLabel.Background = Background;
            mWindow = new Window();
            mWindow.Style = Window.WindowStyle.None;
            mWindow.LostFocus += new ElementEventHandler(mWindow_LostFocus);
            mListBox = new Listbox();
            mListBox.SelectedIndexChanged += new ElementEventHandler(mListBox_SelectedIndexChanged);
            mWindow.Content = mListBox;
            mListBox.Position = new Point(0, 0);
            mListBox.Size = new Point(100, 100);
        }
        public override IUIColor Background
        {
            get
            {
                return base.Background;
            }
            set
            {
                base.Background = value;
                if (mLabel != null)
                    mLabel.Background = base.Background;
            }
        }
        void mListBox_SelectedIndexChanged(object sender)
        {
            mWindow.Hide();
            if (mListBox.SelectedIndex != -1)
                SelectedIndex = mListBox.SelectedIndex;
        }
        void mWindow_LostFocus(object sender)
        {
            mWindow.Hide();
            if (mListBox.SelectedIndex != -1)
                SelectedIndex = mListBox.SelectedIndex;
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
                if (mSelectedIndex > ComboBoxObjects.Count - 1 || mSelectedIndex < 0 || ComboBoxObjects.Count == 0)
                    mSelectedIndex = -1;
                if (mSelectedIndex != -1)
                    mLabel.Text = DisplayTexts[mSelectedIndex];
                else
                    mLabel.Text = "";
            }
        }
        public Object SelectedItem
        {
            get
            {
                return ComboBoxObjects[mSelectedIndex];
            }
        }
        void mButton_Click(object sender)
        {
            mListBox.Size = new Point(Bounds.Width, mListBox.RequestedSize.Height);
            mWindow.Size = mListBox.Size;
            Point p = ScreenCoords;
            p.Y += Bounds.Height + 1;
            mWindow.Position = p;
            mWindow.Show();
        }
        protected internal override void Render()
        {
            base.Render();
            mButton.Render();
            mLabel.Render();
            CoreUIEngine.mDrawEngine.Draw_Box(Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Bottom, BlackColor);
        }
        public void AddItem(Object o)
        {
            ComboBoxObjects.Add(o);
            DisplayTexts.Add(o.ToString());
            mListBox.AddItem(o);
            if (ComboBoxObjects.Count == 1)
            {
                mLabel.Text = DisplayTexts[0];
                SelectedIndex = 0;
                mListBox.SelectedIndex = 0;
            }
        }
        public void RemoveItem(Object o)
        {
            if (ComboBoxObjects.Contains(o))
            {
                DisplayTexts.RemoveAt(ComboBoxObjects.IndexOf(o));
                ComboBoxObjects.Remove(o);
                mListBox.RemoveItem(o);
                if (ComboBoxObjects.Count == 0)
                {
                    mLabel.Text = "";
                    SelectedIndex = -1;
                    mListBox.SelectedIndex = -1;
                }
            }
        }
        protected internal override Element HitTest(float X, float Y)
        {
            Element e = mButton.HitTest(X, Y);
            if (e != null)
                return e;
            return base.HitTest(X, Y);
        }
        protected internal override void OnSizeChanged(object sender)
        {
            CalculateSize();
            base.OnSizeChanged(sender);
        }
        protected internal override void OnPositionChanged(object sender)
        {
            CalculatePosition();
            base.OnPositionChanged(sender);
        }
        private void CalculatePosition()
        {
            mLabel.Position = new Point(Bounds.Left + 2, Bounds.Top + 1);
            mButton.Position = new Point(Bounds.Right - mButton.Size.X - 1, Bounds.Top + 1);
        }
        private void CalculateSize()
        {

            PointF size = CoreUIEngine.mDrawEngine.getTextSize("AL/|^$", mFontInt);
            mBounds.Height = (int)(size.Y + 2);
            mLabel.Position = new Point(Bounds.Left + 2, Bounds.Top + 1);
            mLabel.Size = new Point((int)(Bounds.Width - size.Y - 1), (int)size.Y);
            mButton.Position = new Point(Bounds.Right - (int)size.Y - 1, Bounds.Top + 1);
            mButton.Size = new Point((int)size.Y, (int)size.Y);
        }
    }
}
