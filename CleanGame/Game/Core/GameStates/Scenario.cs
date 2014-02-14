using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGame.game.Core.GameStates
{
    public class Scenario
    {
        //Name of Scenario
        private string name;
        //Difficulty Score
        private float difficultyScore;
        //Map Name
        private string mapName;
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
        public Scenario(string name, float difficulty, string map, List<Spawner> listSpawners)
        {
            this.name = name;
            this.difficultyScore = difficulty;
            this.mapName = map;
            this.spawners = listSpawners;
        }
    }
}
