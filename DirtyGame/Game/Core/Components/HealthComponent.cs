using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.Core.Components
{
    class HealthComponent : Component
    {
        public int MaxHealth;
        public float CurrentHealth;
    }
}
