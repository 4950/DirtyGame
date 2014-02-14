using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CleanGame.Game.Core.GameStates
{
    public class Scenario
    {
        //Name of Scenario
        private string name;
        //Difficulty Score
        private float difficultyScore;
        //Map Name
        private string mapName;
        //Player Spawn Location
        private Vector2 playerSpawn;
        //List of Spawners
        private List<Spawner> spawners;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public float DifficultyScore
        {
            get
            {
                return difficultyScore;
            }
            set
            {
                difficultyScore = value;
            }
        }
        public string MapName
        {
            get
            {
                return mapName;
            }
            set
            {
                mapName = value;
            }
        }
        public Vector2 PlayerSpawn
        {
            get
            {
                return playerSpawn;
            }
            set
            {
                playerSpawn = value;
            }
        }
        public List<Spawner> Spawners
        {
            get
            {
                return spawners;
            }
            set
            {
                spawners = value;
            }
        }

        public Scenario()
        {
        }
        public Scenario(string name, float difficulty, string map, List<Spawner> listSpawners, Vector2 pSpawn)
        {
            this.name = name;
            this.difficultyScore = difficulty;
            this.mapName = map;
            this.spawners = listSpawners;
            this.playerSpawn = pSpawn;
        }
    }
}
