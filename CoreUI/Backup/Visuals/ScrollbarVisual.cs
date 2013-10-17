using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MTV3D65;

namespace CoreUI.Visuals
{
    public class ScrollbarVisual : ButtonChrome
    {
        private static int nccolL = new TV_COLOR(245 / 255.0f, 245 / 255.0f, 245 / 255.0f, 1).GetIntColor();
        private static int nccolMc = new TV_COLOR(120 / 255.0f, 119 / 255.0f, 119 / 255.0f, 1).GetIntColor();
        private static int nccolMf = new TV_COLOR(177 / 255.0f, 177 / 255.0f, 177 / 255.0f, 1).GetIntColor();
        private static int nccolR = new TV_COLOR(192 / 255.0f, 192 / 255.0f, 196 / 255.0f, 1).GetIntColor();

        private static int yccolL = new TV_COLOR(233 / 255.0f, 247 / 255.0f, 253 / 255.0f, 1).GetIntColor();
        private static int yccolMc = new TV_COLOR(26 / 255.0f, 64 / 255.0f, 85 / 255.0f, 1).GetIntColor();
        private static int yccolMf = new TV_COLOR(91 / 255.0f, 151 / 255.0f, 183 / 255.0f, 1).GetIntColor();
        private static int yccolR = new TV_COLOR(102 / 255.0f, 186 / 255.0f, 221 / 255.0f, 1).GetIntColor();

        protected internal override void Render()
        {
            base.Render();
            if (Parent != null)
            {
                if (mIndent)
                {
                    CoreUIEngine.mScreen2D.Draw_FilledBox(Parent.InnerBounds.Left, Parent.InnerBounds.Top, Parent.InnerBounds.Right, Parent.InnerBounds.Bottom, yccolL, yccolR, yccolR, yccolL);
                    if (Parent.InnerBounds.Height > 10 && Parent.InnerBounds.Width > 5)
                    {
                        CoreUIEngine.mScreen2D.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), yccolMc);
                        CoreUIEngine.mScreen2D.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), yccolMf);

                        CoreUIEngine.mScreen2D.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top - 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top - 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), yccolMc);
                        CoreUIEngine.mScreen2D.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top - 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top - 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), yccolMf);

                        CoreUIEngine.mScreen2D.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), yccolMc);
                        CoreUIEngine.mScreen2D.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), yccolMf);
                    }
                }
                else
                {
                    CoreUIEngine.mScreen2D.Draw_FilledBox(Parent.InnerBounds.Left, Parent.InnerBounds.Top, Parent.InnerBounds.Right, Parent.InnerBounds.Bottom, nccolL, nccolR, nccolR, nccolL);
                    if (Parent.InnerBounds.Height > 10 && Parent.InnerBounds.Width > 5)
                    {
                        CoreUIEngine.mScreen2D.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), nccolMc);
                        CoreUIEngine.mScreen2D.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), nccolMf);

                        CoreUIEngine.mScreen2D.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top - 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top - 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), nccolMc);
                        CoreUIEngine.mScreen2D.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top - 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top - 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), nccolMf);

                        CoreUIEngine.mScreen2D.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), nccolMc);
                        CoreUIEngine.mScreen2D.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), nccolMf);
                    }
                }
            }
        }
    }
}