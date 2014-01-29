using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.Core.Components
{
    public class AOEComponent : Component
    {
        /// <summary>
        /// Damage is applied to enemies in range each tick
        /// </summary>
        public float Damage;
        /// <summary>
        /// Interval between ticks in seconds
        /// </summary>
        public float TickInterval;
        /// <summary>
        /// Total number of ticks
        /// </summary>
        public float Ticks;
        /// <summary>
        /// Time elapse since last tick(counts up)
        /// </summary>
        public float Timer;
        public EntityRef Owner;
        /// <summary>
        /// This bool is true only for frames in which damage will be applied
        /// </summary>
        public List<Entity> HitList = new List<Entity>();
    }
}
