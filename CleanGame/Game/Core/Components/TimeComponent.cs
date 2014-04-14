﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityFramework
{
    public class TimeComponent : Component
    {
        public TimeSpan timeOfLastDraw;
        public TimeSpan timeBetweenDraw;
        public float timeWander;
        public float timeofLock;
        public float timeDrawn;
        public TimeSpan timeOfWeaponCheck;
    }
}
