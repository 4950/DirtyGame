using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace CoreUI.Visuals
{
    public class XVisual : Visual
    {
        protected internal override void Render()
        {
            base.Render();
            CoreUIEngine.mScreen2D.Draw_Line(Bounds.Left + 1, Bounds.Top + 1, Bounds.Right - 1, Bounds.Bottom - 1, Color.Red.ToArgb());
            CoreUIEngine.mScreen2D.Draw_Line(Bounds.Right - 1, Bounds.Top + 1, Bounds.Left + 1, Bounds.Bottom - 1, Color.Red.ToArgb());
        }
    }
}
