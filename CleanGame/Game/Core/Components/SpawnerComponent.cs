using DirtyGame.game.Core.Components.Render;
using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DirtyGame.game.Core.Components
{
    public class SpawnerComponent : Component
    {
        public TimeSpan timePerSpawn;
        public int numMobs;
        public TimeSpan timeOfLastSpawn;
        [XmlIgnoreAttribute]
        public SpriteComponent sprite;
        public MonsterData data;
    }
}
