using CleanGame.Game.Core.Components.Render;
using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CleanGame.Game.Core.Components
{
    public class SpawnerComponent : Component
    {
        public TimeSpan timePerSpawn;
        public int numMobs;
        public TimeSpan timeOfLastSpawn;
        public String MonsterType;
        public String MonsterWeapon;
        public float HealthUpModifier;
        public float DamageUpModifier;

    }
}
