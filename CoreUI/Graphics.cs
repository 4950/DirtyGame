using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUI
{
    public static class Graphics
    {
        public enum TwoColorDirection { Right, Down };
        public enum ThreeColorDirection { BottomRight, BottomLeft };
        public static void Draw_Box(int x1, int y1, int x2, int y2, IUIColor color)
        {
            CoreUIEngine.mDrawEngine.Draw_Box(x1, y1, x2, y2, color);
        }
        public static void Draw_Box(int x1, int y1, int x2, int y2, IUIColor color1, IUIColor color2, TwoColorDirection dir)
        {
            if (dir == TwoColorDirection.Down)
            {
                CoreUIEngine.mDrawEngine.Draw_Line(x1, y1, x2 + 1, y1, color1);
                CoreUIEngine.mDrawEngine.Draw_Line(x1, y1, x1, y2 + 1, color1, color2);
                CoreUIEngine.mDrawEngine.Draw_Line(x2, y1, x2, y2 + 1, color1, color2);
                CoreUIEngine.mDrawEngine.Draw_Line(x1, y2, x2 + 1, y2, color2);
            }
            else if (dir == TwoColorDirection.Right)
            {
                CoreUIEngine.mDrawEngine.Draw_Line(x1, y1, x2 + 1, y1, color1, color2);
                CoreUIEngine.mDrawEngine.Draw_Line(x1, y1, x1, y2 + 1, color1);
                CoreUIEngine.mDrawEngine.Draw_Line(x2, y1, x2, y2 + 1, color2);
                CoreUIEngine.mDrawEngine.Draw_Line(x1, y2, x2 + 1, y2, color1, color2);
            }
        }
        public static void Draw_Box(int x1, int y1, int x2, int y2, IUIColor color1, IUIColor color2, IUIColor color3, ThreeColorDirection dir)
        {
            if (dir == ThreeColorDirection.BottomRight)
            {
                CoreUIEngine.mDrawEngine.Draw_Line(x1, y1, x2 + 1, y1, color1, color2);
                CoreUIEngine.mDrawEngine.Draw_Line(x1, y1, x1, y2 + 1, color1, color2);
                CoreUIEngine.mDrawEngine.Draw_Line(x2, y1, x2, y2 + 1, color2, color3);
                CoreUIEngine.mDrawEngine.Draw_Line(x1, y2, x2 + 1, y2, color2, color3);
            }
            else if (dir == ThreeColorDirection.BottomLeft)
            {
                CoreUIEngine.mDrawEngine.Draw_Line(x1, y1, x2 + 1, y1, color2, color1);
                CoreUIEngine.mDrawEngine.Draw_Line(x1, y1, x1, y2 + 1, color2, color3);
                CoreUIEngine.mDrawEngine.Draw_Line(x2, y1, x2, y2 + 1, color1, color2);
                CoreUIEngine.mDrawEngine.Draw_Line(x1, y2, x2 + 1, y2, color2, color3);
            }
        }
    }
}
