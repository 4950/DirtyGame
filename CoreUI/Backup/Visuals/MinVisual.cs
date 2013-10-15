using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace CoreUI.Visuals
{
    public class MinVisual : Visual
    {
        protected internal override void Render()
        {
            base.Render();
            CoreUIEngine.mScreen2D.Draw_Line(Bounds.Right - 2, Bounds.Top + 2, Bounds.Right - 2, Bounds.Bottom - 2, Color.Red.ToArgb());
            CoreUIEngine.mScreen2D.Draw_Line(Bounds.Left + 2, Bounds.Top + 2, Bounds.Left + 2, Bounds.Bottom - 2, Color.Red.ToArgb());
            CoreUIEngine.mScreen2D.Draw_Line(Bounds.Left + 2, Bounds.Top + 2, Bounds.Right - 2, Bounds.Top + 2, Color.Red.ToArgb());
            CoreUIEngine.mScreen2D.Draw_Line(Bounds.Left + 2, Bounds.Bottom - 2, Bounds.Right - 2, Bounds.Bottom - 2, Color.Red.ToArgb());
        }
    }
}
