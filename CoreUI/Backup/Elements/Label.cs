using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CoreUI.Elements
{
    public enum LabelTextMode { Truncate, SizeToContent }
    public enum TextPosition { Left, Center, Right }
    public class Label : Element
    {
        
        private LabelTextMode mTextMode = LabelTextMode.Truncate;
        private TextPosition mTextPosition = TextPosition.Left;
        private String mText = "";
        private String DispText = "";
        private Point TextPos = new Point();

        public Label()
        {
            Background = Color.FromArgb(0, 0, 0, 0).ToArgb();
        }
        protected internal override void Render()
        {
            base.Render();
            CoreUIEngine.mScreen2D.Draw_FilledBox(Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Bottom, Background);
            CoreUIEngine.mScreen2D.Action_End2D();
            CoreUIEngine.mText.Action_BeginText();
            CoreUIEngine.mText.NormalFont_DrawText(DispText, TextPos.X, TextPos.Y, Foreground, mFontInt);
            CoreUIEngine.mText.Action_EndText();
            CoreUIEngine.mScreen2D.Action_Begin2D();
        }
        public LabelTextMode TextMode
        {
            get
            {
                return mTextMode;
            }
            set
            {
                mTextMode = value;
                CalculateText();
                InvalidateVisual();
            }
        }
        public TextPosition TextPosition
        {
            get
            {
                return mTextPosition;
            }
            set
            {
                mTextPosition = value;
                CalculateText();
                InvalidateVisual();
            }
        }
        protected internal override void OnSizeChanged(object sender)
        {
            CalculateText();
            base.OnSizeChanged(sender);
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

            if (mTextMode == LabelTextMode.Truncate)
            {
                if (w > Bounds.Width)
                {
                    StringBuilder sb = new StringBuilder(DispText);
                    while (w > Bounds.Width)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        CoreUIEngine.mText.NormalFont_GetTextSize(sb.ToString(), mFontInt, ref w, ref h);
                    }
                    DispText = sb.ToString();
                    TextPos.X = Bounds.Left;
                }
                else
                {
                    switch (mTextPosition)
                    {
                        case TextPosition.Center:
                            TextPos.X = (int)(Bounds.Left + (Bounds.Width / 2) - (w / 2));
                            break;
                        case TextPosition.Left:
                            TextPos.X = Bounds.Left;
                            break;
                        case TextPosition.Right:
                            TextPos.X = (int)(Bounds.Left + (Bounds.Width - w));
                            break;
                    }
                }
            }
            else if (mTextMode == LabelTextMode.SizeToContent)
            {
                mBounds.Width = (int)w;
                mBounds.Height = (int)h;
                TextPos.X = Bounds.Left;
            }
            TextPos.Y = (int)(Bounds.Top + (Bounds.Height / 2) - (h / 2));
        }
    }
}
