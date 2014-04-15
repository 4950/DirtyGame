using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanGame.Game.Core.Components
{
    class ArcherComponent : Component
    {
        public TimeSpan timeOfWeaponCheck;
        public TimeSpan timeToNextCheck;
        public float timeElapsed;
    }
}
