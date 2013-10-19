using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace CoreUI.Visuals
{
    public class ScrollbarVisual : ButtonChrome
    {
        private static IUIColor nccolL = CoreUIEngine.mDrawEngine.CreateColor(245 / 255.0f, 245 / 255.0f, 245 / 255.0f, 1);
        private static IUIColor nccolMc = CoreUIEngine.mDrawEngine.CreateColor(120 / 255.0f, 119 / 255.0f, 119 / 255.0f, 1);
        private static IUIColor nccolMf = CoreUIEngine.mDrawEngine.CreateColor(177 / 255.0f, 177 / 255.0f, 177 / 255.0f, 1);
        private static IUIColor nccolR = CoreUIEngine.mDrawEngine.CreateColor(192 / 255.0f, 192 / 255.0f, 196 / 255.0f, 1);

        private static IUIColor yccolL = CoreUIEngine.mDrawEngine.CreateColor(233 / 255.0f, 247 / 255.0f, 253 / 255.0f, 1);
        private static IUIColor yccolMc = CoreUIEngine.mDrawEngine.CreateColor(26 / 255.0f, 64 / 255.0f, 85 / 255.0f, 1);
        private static IUIColor yccolMf = CoreUIEngine.mDrawEngine.CreateColor(91 / 255.0f, 151 / 255.0f, 183 / 255.0f, 1);
        private static IUIColor yccolR = CoreUIEngine.mDrawEngine.CreateColor(102 / 255.0f, 186 / 255.0f, 221 / 255.0f, 1);

        protected internal override void Render()
        {
            base.Render();
            if (Parent != null)
            {
                if (mIndent)
                {
                    CoreUIEngine.mDrawEngine.Draw_FilledBox(Parent.InnerBounds.Left, Parent.InnerBounds.Top, Parent.InnerBounds.Right, Parent.InnerBounds.Bottom, yccolL, yccolR, yccolR, yccolL);
                    if (Parent.InnerBounds.Height > 10 && Parent.InnerBounds.Width > 5)
                    {
                        CoreUIEngine.mDrawEngine.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), yccolMc);
                        CoreUIEngine.mDrawEngine.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), yccolMf);

                        CoreUIEngine.mDrawEngine.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top - 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top - 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), yccolMc);
                        CoreUIEngine.mDrawEngine.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top - 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top - 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), yccolMf);

                        CoreUIEngine.mDrawEngine.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), yccolMc);
                        CoreUIEngine.mDrawEngine.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), yccolMf);
                    }
                }
                else
                {
                    CoreUIEngine.mDrawEngine.Draw_FilledBox(Parent.InnerBounds.Left, Parent.InnerBounds.Top, Parent.InnerBounds.Right, Parent.InnerBounds.Bottom, nccolL, nccolR, nccolR, nccolL);
                    if (Parent.InnerBounds.Height > 10 && Parent.InnerBounds.Width > 5)
                    {
                        CoreUIEngine.mDrawEngine.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), nccolMc);
                        CoreUIEngine.mDrawEngine.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), nccolMf);

                        CoreUIEngine.mDrawEngine.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top - 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top - 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), nccolMc);
                        CoreUIEngine.mDrawEngine.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top - 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top - 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), nccolMf);

                        CoreUIEngine.mDrawEngine.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + 3 + (int)Math.Ceiling(Parent.InnerBounds.Height / 2.0d + .01f), nccolMc);
                        CoreUIEngine.mDrawEngine.Draw_Line(Parent.InnerBounds.Left + 2, Parent.InnerBounds.Top + 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), Parent.InnerBounds.Right - 2, Parent.InnerBounds.Top + 3 + (int)Math.Floor(Parent.InnerBounds.Height / 2.0d + .01f), nccolMf);
                    }
                }
            }
        }
    }
}