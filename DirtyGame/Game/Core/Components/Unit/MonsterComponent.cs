using DirtyGame.game.Core.Components.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DirtyGame.game.Core.Components;
using System.Xml.Serialization;

namespace EntityFramework
{
    public struct MonsterData
    {
        public int Health;
        public float scale;
        //public WeaponComponent.WeaponType 
        [XmlIgnoreAttribute]
        public Entity weapon;
         
        public MonsterData(int health, float scale)
        {
            this.Health = health;
            this.scale = scale;
            this.weapon = null;
        }

        public static MonsterData BasicMonster
        {
            get
            {
                MonsterData m = new MonsterData(200, 1);
                return m;
            }
        }

        public static MonsterData RangedMonster
        {
            get
            {
                MonsterData m = new MonsterData(100, 1);
                return m;
            }
        }
    }
    public class MonsterComponent : UnitComponent
    {
        public MonsterData data { get; set; }
        //public string monsterType;
    }
}
