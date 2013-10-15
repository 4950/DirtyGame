using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUI.Brushes
{
    public class SolidColorBrush : Brush
    {
        private int mColor;

        public SolidColorBrush(int Color)
        {
            mColor = Color;
        }
        protected internal override void Draw()
        {
            CoreUIEngine.mScreen2D.Draw_FilledBox(Parent.InnerBounds.Left, Parent.InnerBounds.Top, Parent.InnerBounds.Right, Parent.InnerBounds.Bottom, mColor);
        }
    }
}
