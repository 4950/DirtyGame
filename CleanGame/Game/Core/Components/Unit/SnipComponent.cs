using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework;
using Microsoft.Xna.Framework;

namespace CleanGame.Game.Core.Components
{
    public class SnipComponent : Component
    {
        #region Variables
        private uint LaserId;
        private Boolean laserpres;
        private Boolean isRunning;
        private int fleeDistance;
        private int laserOffset; //Radians offset after sniper fires
        private double range;
        private Boolean locked;
        private float defaultRotation;
        private float timeDelay; //seconds
        private Vector2 offset; //Vector, offset when player enters range
        #endregion

        #region Constructors
        public SnipComponent()
        {
            locked = false;
            laserpres = false;
            isRunning = false;
            fleeDistance = 200;
            defaultRotation = 2f;
            timeDelay = 1;
            range = 400;
            laserOffset = 5;
            offset = new Vector2(75, 75);
        }
        #endregion


        #region Properties
        //Delay in seconds


        public Vector2 Offset
        {
            get
            {
                return offset;
            }

            set
            {
                offset = value;
            }
        }

        public int LaserOffset
        {
            get
            {
                return laserOffset;
            }

            set
            {
                laserOffset = value;
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

        public float DefaultRotation
        {
            get
            {
                return defaultRotation;
            }
            set
            {
                defaultRotation = value;
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

        public Boolean Locked
        {
            get
            {
                return locked;
            }
            set
            {
                locked = value;
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
        #endregion


    }
}

