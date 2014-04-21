using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace CleanGame.Game.Core.Components
{
    public class StatsComponent : Component
    {
        //Base Stats
        public int BaseMoveSpeed { get; set; }
        public int BaseDamage { get; set; }
        public int BaseHealth { get; set; }

        //Stat Scale
        public float HealthScale { get; set; }

        //Other stats
        public float CurrentHealth;
        [XmlArray]
        [XmlArrayItem(ElementName="List")]
        public List<string> ImmuneTo { get; set; }
        //Calculated Stats
        public int MaxHealth
        {
            get
            {
                return (int)(BaseHealth * HealthScale);
            }
        }
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

        public override object Clone()
        {
            StatsComponent clone = base.Clone() as StatsComponent;
            clone.ImmuneTo = new List<string>(ImmuneTo);
            return clone;
        }
        

    }
}
