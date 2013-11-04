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
    }
}
