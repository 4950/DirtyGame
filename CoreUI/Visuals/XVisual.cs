using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace CoreUI.Visuals
{
    public class XVisual : Visual
    {
        private IUIColor red = CoreUIEngine.mDrawEngine.CreateColor(Color.Red);
        protected internal override void Render()
        {
            base.Render();
            CoreUIEngine.mDrawEngine.Draw_Line(Bounds.Left + 1, Bounds.Top + 1, Bounds.Right - 1, Bounds.Bottom - 1, red);
            CoreUIEngine.mDrawEngine.Draw_Line(Bounds.Right - 1, Bounds.Top + 1, Bounds.Left + 1, Bounds.Bottom - 1, red);
        }
    }
}
