using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CoreUI.Visuals
{
    public class ButtonChrome : Visual
    {
        protected bool mIndent = false;

        protected internal override void Render()
        {
            base.Render();
            if (Parent != null)
            {
                if (mIndent)
                {
                    CoreUIEngine.mDrawEngine.Draw_FilledBox(Parent.InnerBounds.Left, Parent.InnerBounds.Top, Parent.InnerBounds.Right, Parent.InnerBounds.Bottom, Parent.Middleground);
                }
                else
                {
                    //CoreUIEngine.mDrawEngine.Draw_FilledBox(Parent.InnerBounds.Left, Parent.InnerBounds.Top, Parent.InnerBounds.Right, Parent.InnerBounds.Bottom, CoreUIEngine.mDrawEngine.CreateColor(Color.LightBlue), CoreUIEngine.mDrawEngine.CreateColor(Color.Tomato), CoreUIEngine.mDrawEngine.CreateColor(Color.Wheat), CoreUIEngine.mDrawEngine.CreateColor(Color.Blue));
                    CoreUIEngine.mDrawEngine.Draw_Box(Parent.InnerBounds.Left, Parent.InnerBounds.Top, Parent.InnerBounds.Right, Parent.InnerBounds.Bottom, CoreUIEngine.mDrawEngine.CreateColor(Color.LightBlue));
                    //CoreUI.mScreen2D.Draw_Box(Parent.Position.X, Parent.Position.Y, Parent.Position.X + Parent.Size.X, Parent.Position.Y + Parent.Size.Y, Color.LightBlue.ToArgb(), Color.LightBlue.ToArgb(), Color.Blue.ToArgb(), Color.Blue.ToArgb());
                }
            }
        }
        public bool Indent
        {
            get
            {
                return mIndent;
            }
            set
            {
                mIndent = value;
                InvalidateVisual();
            }
        }
    }
}
