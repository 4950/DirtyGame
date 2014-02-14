using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CleanGame.Game.Core.Components
{
    public class AOEComponent : Component
    {
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
        [XmlIgnoreAttribute]
        public EntityRef Owner;
        [XmlIgnoreAttribute]
        public EntityRef Weapon;
        /// <summary>
        /// This bool is true only for frames in which damage will be applied
        /// </summary>
        public List<Entity> HitList = new List<Entity>();
    }
}
