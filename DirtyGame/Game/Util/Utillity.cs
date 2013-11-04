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
            Random random = RandomNumberGenerator.Rand;
            return new Point(random.Next(rect.Width - rect.X) + rect.X, random.Next(rect.Height - rect.Y) + rect.Y);
        }
    }
}
