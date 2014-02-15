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
            Background = CoreUIEngine.mDrawEngine.CreateColor(0, 0, 0, 0);
        }
        protected internal override void Render()
        {
            base.Render();
            CoreUIEngine.mDrawEngine.Draw_FilledBox(Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Bottom, Background);
            //CoreUIEngine.mDrawEngine.Action_End2D();
            //CoreUIEngine.mText.Action_BeginText();
            CoreUIEngine.mDrawEngine.Draw_Default_Text(DispText, TextPos.X, TextPos.Y, Foreground, mFontInt);
            //CoreUIEngine.mText.Action_EndText();
            //CoreUIEngine.mDrawEngine.Action_Begin2D();
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
        protected internal override void OnPositionChanged(object sender)
        {
            base.OnPositionChanged(sender);
            CalculateText();
        }
        protected internal override void OnSizeChanged(object sender)
        {
            base.OnSizeChanged(sender);
            CalculateText();
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
            PointF size = CoreUIEngine.mDrawEngine.getTextSize(DispText, mFontInt);

            if (mTextMode == LabelTextMode.Truncate)
            {
                if (size.X > Bounds.Width)
                {
                    StringBuilder sb = new StringBuilder(DispText);
                    while (size.X > Bounds.Width)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        size = CoreUIEngine.mDrawEngine.getTextSize(sb.ToString(), mFontInt);
                    }
                    DispText = sb.ToString();
                    TextPos.X = Bounds.Left;
                }
                else
                {
                    switch (mTextPosition)
                    {
                        case TextPosition.Center:
                            TextPos.X = (int)(Bounds.Left + (Bounds.Width / 2) - (size.X / 2));
                            break;
                        case TextPosition.Left:
                            TextPos.X = Bounds.Left;
                            break;
                        case TextPosition.Right:
                            TextPos.X = (int)(Bounds.Left + (Bounds.Width - size.X));
                            break;
                    }
                }
            }
            else if (mTextMode == LabelTextMode.SizeToContent)
            {
                mBounds.Width = (int)size.X;
                mBounds.Height = (int)size.Y;
                TextPos.X = Bounds.Left;
            }
            TextPos.Y = (int)(Bounds.Top + (Bounds.Height / 2) - (size.Y / 2));
             
        }
    }
}
