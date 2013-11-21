using DirtyGame.game.Core.Components.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EntityFramework
{
    public struct MonsterData
    {
        public int Health;
        public Vector2 Size;
         
        public MonsterData(int health, Vector2 size)
        {
            this.Health = health;
            this.Size = size;
        }

        public static MonsterData BasicMonster
        {
            get
            {
                MonsterData m = new MonsterData(200, new Vector2(46, 46));
                return m;
            }
        }

        public static MonsterData RangedMonster
        {
            get
            {
                MonsterData m = new MonsterData(100, new Vector2(46, 46));
                return m;
            }
        }
    }
    class MonsterComponent : UnitComponent
    {
        public MonsterData data { get; set; }
        //public string monsterType;
    }
}
