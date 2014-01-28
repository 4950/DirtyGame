﻿using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace DirtyGame.game.Core.Components
{
    public class StatsComponent : Component
    {
        //Base Stats
        public int BaseMoveSpeed { get; set; }
        public int BaseDamage { get; set; }

        //Calculated Stats
        public int MoveSpeed
        {
            get
            {
                return BaseMoveSpeed;
            }
        }
        public int Damage
        {
            get
            {
                return BaseDamage;
            }
        }
    }
}
