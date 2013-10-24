using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityFramework
{
    class TimeComponent : Component
    {
        public TimeSpan timeOfLastDraw;
        public TimeSpan timeBetweenDraw;
    }
}
