using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework;

namespace DirtyGame.game.Core.Components
{
    class SnipComponent : Component
    {
        private uint LaserId;
        private Boolean laserpres;
        private Boolean isRunning;
        private Boolean lockedOn;
        private int fleeDistance;
        private double range;
        private float timeDelay;

        public SnipComponent()
        {
            laserpres = false;
            isRunning = false;
            lockedOn = false;
            fleeDistance = 200;
            range = 400;
            timeDelay = 1;
        }


        //Delay in seconds
        public float TimeDelay
        {
            get
            {
                return timeDelay;
            }
            set
            {
                timeDelay = value;
            }
        }

        public double Range
        {
            get
            {
                return range;
            }
            set
            {
                range = value;
            }
        }

        public uint Laser
        {
            get
            {
                return LaserId;
            }
            set
            {
                LaserId = value;
            }
        }

        public Boolean LaserPres
        {
            get
            {
                return laserpres;
            }
            set
            {
                laserpres = value;
            }
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

        public Boolean IsRunning
        {
            get
            {
                return isRunning;
            }
            set
            {
                isRunning = value;
            }
        }

        public int FleeDistance
        {
            get
            {
                return fleeDistance;
            }
            set
            {
                fleeDistance = value;
            }
        }


    }
}
