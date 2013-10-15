using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUI.Brushes
{
    public class SolidColorBrush : Brush
    {
        private IUIColor mColor;

        public SolidColorBrush(IUIColor Color)
        {
            mColor = Color;
        }
        protected internal override void Draw()
        {
            CoreUIEngine.mDrawEngine.Draw_FilledBox(Parent.InnerBounds.Left, Parent.InnerBounds.Top, Parent.InnerBounds.Right, Parent.InnerBounds.Bottom, mColor);
        }
    }
}
