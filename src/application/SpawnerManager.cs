using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ShittyPrototype.src.core;
using ShittyPrototype.src.graphics;
using System.Diagnostics;
using ShittyPrototype.src.application.core;


namespace ShittyPrototype.src.application
{
    class SpawnerManager
    {
        public Spawner[] spawners;
        Random r = new Random();
        public SpawnerManager()
        { 
        
        }

        public SpawnerManager(Spawner[] spawnerList) 
        {
            this.spawners = spawnerList;
        }

        public List<Monster> Update(GameTime gameTime)
        {
            List<Monster> monstersToSpawn = new List<Monster>();
            foreach(Spawner s in spawners)
            {

                if ((s.spawner.numMobs != 0) && (s.spawner.timePerSpawn + s.spawner.timeOfLastSpawn <= gameTime.TotalGameTime))
                {
                    int xPos = s.pos.xPosition+r.Next(-75, 76);
                    int yPos = s.pos.yPosition+r.Next(-75, 76);

                    Monster monster = new Monster(xPos, yPos, s.render);
                    
                    s.spawner.numMobs--;
                    s.spawner.timeOfLastSpawn = gameTime.TotalGameTime;

                    monstersToSpawn.Add(monster);
                }
            }   
            return monstersToSpawn;
        }
    }
}
