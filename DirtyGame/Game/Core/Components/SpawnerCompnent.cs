using DirtyGame.game.Core.Components.Render;
using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.Core.Components
{
    class SpawnerComponent : Component
    {
        public TimeSpan timePerSpawn;
        public int numMobs;
        public TimeSpan timeOfLastSpawn;
        public SpriteComponent sprite;
        public MonsterData data;
    }
}
