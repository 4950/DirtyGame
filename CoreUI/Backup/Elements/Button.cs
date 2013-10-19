using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CoreUI.Elements
{
    public class Button : Element
    {
        public delegate void ClickEventHandler(object sender);
        public event ClickEventHandler Click;
        private Visuals.ButtonChrome mButtonChrome;
        private String mText = "";
        private String DispText = "";
        private Point TextPos = new Point();

        private bool mIsPressed = false;

        public Button()
        {
            Middleground = Color.Navy.ToArgb();
            mButtonChrome = new Visuals.ButtonChrome();
            mButtonChrome.Parent = this;
        }
        protected internal override void Render()
        {
            mButtonChrome.Render();
            base.Render();
            CoreUIEngine.mScreen2D.Action_End2D();
            CoreUIEngine.mText.Action_BeginText();
            CoreUIEngine.mText.NormalFont_DrawText(DispText, TextPos.X, TextPos.Y, Foreground, mFontInt);
            CoreUIEngine.mText.Action_EndText();
            CoreUIEngine.mScreen2D.Action_Begin2D();
        }
        public String Text
        {
            get
            {
                return mText;
            }
            set
            {
                mText = value;
                CalculateText();
                InvalidateVisual();
            }
        }
        private void CalculateText()
        {
            DispText = mText;
            float w = 0, h = 0;
            CoreUIEngine.mText.NormalFont_GetTextSize(DispText, mFontInt, ref w, ref h);
            if (w > Bounds.Width)
            {
                StringBuilder sb = new StringBuilder(DispText);
                while (w > Bounds.Width)
                {
                    sb.Remove(0, 1);
                    sb.Remove(sb.Length - 1, 1);
                    CoreUIEngine.mText.NormalFont_GetTextSize(sb.ToString(), mFontInt, ref w, ref h);
                }
                DispText = sb.ToString();
            }
            TextPos = new Point((int)(Bounds.Left + (Bounds.Width / 2) - (w / 2)), (int)(Bounds.Top + (Bounds.Height / 2) - (h / 2)));
        }
        protected internal override void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mIsPressed = true;
                mButtonChrome.Indent = true;
                InvalidateVisual();
            }
            base.OnMouseDown(sender, e);
        }
        public Visuals.ButtonChrome ButtonChrome
        {
            set
            {
                mButtonChrome = value;
                mButtonChrome.Parent = this;
            }
        }
        protected internal override void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mIsPressed = false;
                mButtonChrome.Indent = false;
                InvalidateVisual();
            }
            base.OnMouseUp(sender, e);
            if (Click != null && e.Clicks > 0)
                Click(sender);
        }
        public bool IsPressed
        {
            get
            {
                return mIsPressed;
            }
        }
    }
}
