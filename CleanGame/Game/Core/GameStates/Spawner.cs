using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CleanGame.Game.Core.GameStates
{
    public class Spawner
    {
        //Data about the spawner
        //Location
        private int xPosition;
        private int yPosition;
        //? ? ? ? ?
        private Rectangle spawnerRectangle;
        //Monster Type
        private string monsterType;
        //Monster Weapon
        private string monsterWeapon;
        //Number of Monsters
        private int numberOfMonsters;
        //TimeSpan for Monsters to Spawn
        private TimeSpan timePerSpawn;
        //Health Up Modifier
        private float healthUpModifier = 1.0f;
        //Damage Up Modifier
        private float damageUpModifier = 1.0f;

        #region Properties
        public int XPosition
        {
            get
            {
                return xPosition;
            }
            set
            {
                xPosition = value;
            }
        }
        public int YPosition
        {
            get
            {
                return yPosition;
            }
            set
            {
                yPosition = value;
            }
        }
        public Rectangle SpawnerRectangle
        {
            get
            {
                return spawnerRectangle;
            }
            set
            {
                spawnerRectangle = value;
            }
        }
        public string MonsterType
        {
            get
            {
                return monsterType;
            }
            set
            {
                monsterType = value;
            }
        }
        public string MonsterWeapon
        {
            get
            {
                return monsterWeapon;
            }
            set
            {
                monsterWeapon = value;
            }
        }
        public int NumberOfMonsters
        {
            get
            {
                return numberOfMonsters;
            }
            set
            {
                numberOfMonsters = value;
            }
        }
        public TimeSpan TimePerSpawn
        {
            get
            {
                return timePerSpawn;
            }
            set
            {
                timePerSpawn = value;
            }
        }
        public float HealthUpModifier
        {
            get
            {
                return healthUpModifier;
            }
            set
            {
                healthUpModifier = value;
            }
        }
        public float DamageUpModifier
        {
            get
            {
                return damageUpModifier;
            }
            set
            {
                damageUpModifier = value;
            }
        }
        #endregion

        public Spawner()
        {
        }
        public Spawner(int xPos, int yPos, Rectangle spawnerRec, string mType, string mWeapon, int numMonsters, TimeSpan tPerSpawn, float healthMod, float damageMod)
        {
            this.xPosition = xPos;
            this.yPosition = yPos;
            this.spawnerRectangle = spawnerRec;
            this.monsterType = mType;
            this.monsterWeapon = mWeapon;
            this.numberOfMonsters = numMonsters;
            this.timePerSpawn = tPerSpawn;
            this.healthUpModifier = healthMod;
            this.damageUpModifier = damageMod;
        }
    }
}
