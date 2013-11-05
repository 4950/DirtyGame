using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Util
{
    public static class Utillity
    {
        public static Point GetRandomPointInside(Rectangle rect)
        {
            return new Point(Rand.RandInt(rect.X, rect.X + rect.Width),
                Rand.RandInt(rect.Y, rect.Y + rect.Height));
        }

        public static Vector2 GetRandomPositionInside(Rectangle rect)
        {
            return new Vector2(Rand.RandInt(rect.X, rect.X + rect.Width),
                Rand.RandInt(rect.Y, rect.Y + rect.Height));
        }

        public static int GetManhattanDistance(RowCol rc1, RowCol rc2)
        {
            return Math.Abs(rc1.Row - rc2.Row) + Math.Abs(rc1.Col - rc2.Col);
        }
}
}
