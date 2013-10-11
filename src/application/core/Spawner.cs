using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShittyPrototype.src.graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShittyPrototype.src.application.core;
using ShittyPrototype.src.application;

namespace ShittyPrototype.src.core
{
    class Spawner : Entity
    {
        public MonsterComponent monster = new MonsterComponent();
        public SpawnerComponent spawner = new SpawnerComponent();
        public PositionComponent pos = new PositionComponent();
        public RenderComponent render = new RenderComponent();
        
        public Spawner() { }

        
        public Spawner(int spawnTimeMilliseconds, int xLocation, int yLocation, int numMobs, Texture2D textures)
        {
            //this.monster.monster = monster;

            this.spawner.timePerSpawn = new TimeSpan(0,0,0,0,spawnTimeMilliseconds);
            this.spawner.numMobs = numMobs;
            this.spawner.timeOfLastSpawn = new TimeSpan(0, 0, 0, 0, 0);

            this.pos.x = xLocation;
            this.pos.y = yLocation;
            
            this.render.texture = textures;
            this.render.rectangle = new Rectangle(pos.x, pos.y, 46, 46);

            this.AddComponent(render);
            this.AddComponent(pos);
            this.AddComponent(spawner);
            
        }
    }
}
