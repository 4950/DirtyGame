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
        //Monster Type
        private string monsterType;
        //Monster Weapon
        private string monsterWeapon;
        //Number of Monsters
        private int numberOfMonsters;
        //TimeSpan for Monsters to Spawn
        private TimeSpan timePerSpawn;
        //Modifier
        private float damageupmodifier = 1.0f;
        private float healthupmodifier = 1.0f;
        

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
        public float DamageUpModifier
        {
            get
            {
                return damageupmodifier;
            }
            set
            {
                damageupmodifier = value;
            }
        }

        
        public float HealthUpModifier
        {
            get
            {
                return healthupmodifier;
            }
            set
            {
                healthupmodifier = value;
            }
        }

        #endregion

        public Spawner()
        {
        }
        public Spawner(int xPos, int yPos, string mType, string mWeapon, int numMonsters, TimeSpan tPerSpawn, float healthUpModifier, float damageUpModifier)
        {
            this.xPosition = xPos;
            this.yPosition = yPos;
            this.monsterType = mType;
            this.monsterWeapon = mWeapon;
            this.numberOfMonsters = numMonsters;
            this.timePerSpawn = tPerSpawn;
            this.healthupmodifier = healthUpModifier;
            this.damageupmodifier = damageUpModifier;
        }
    }
}
