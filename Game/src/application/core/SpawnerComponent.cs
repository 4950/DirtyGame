using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShittyPrototype.src.core;

namespace ShittyPrototype.src.application.core
{
    class SpawnerComponent : IComponent
    {
        public TimeSpan timePerSpawn;
        public int numMobs;
        public TimeSpan timeOfLastSpawn;
        
    }
}
