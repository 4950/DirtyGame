using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace CoreUI.Visuals
{
    public class MinVisual : Visual
    {
        private IUIColor red = CoreUIEngine.mDrawEngine.CreateColor(Color.Red);
        protected internal override void Render()
        {
            base.Render();
            CoreUIEngine.mDrawEngine.Draw_Line(Bounds.Right - 2, Bounds.Top + 2, Bounds.Right - 2, Bounds.Bottom - 2, red);
            CoreUIEngine.mDrawEngine.Draw_Line(Bounds.Left + 2, Bounds.Top + 2, Bounds.Left + 2, Bounds.Bottom - 2, red);
            CoreUIEngine.mDrawEngine.Draw_Line(Bounds.Left + 2, Bounds.Top + 2, Bounds.Right - 2, Bounds.Top + 2, red);
            CoreUIEngine.mDrawEngine.Draw_Line(Bounds.Left + 2, Bounds.Bottom - 2, Bounds.Right - 2, Bounds.Bottom - 2, red);
        }
    }
}
