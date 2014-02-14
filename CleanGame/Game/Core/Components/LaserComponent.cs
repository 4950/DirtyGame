using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework;

namespace CleanGame.Game.Core.Components
{
    public class LaserComponent : Component
    {

        private Boolean lockedOn;
        private Boolean playerPres;
        public Boolean Reset;
        public int Offset;

        public LaserComponent()
        {
            lockedOn = false;
            playerPres = false;
            Reset = false;
        }

        public Boolean LockedOn
        {
            get
            {
                return lockedOn;
            }
            set
            {
                lockedOn = value;
            }
        }

        public Boolean PlayerPres
        {
            get
            {
                return playerPres;
            }
            set
            {
                playerPres = value;
            }
        }
        
    }
}
