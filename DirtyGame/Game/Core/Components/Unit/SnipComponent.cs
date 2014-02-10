using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Core.Components
{
    public class SnipComponent : Component
    {
        #region Variables
        private uint LaserId;
        private Boolean laserpres;
        private Boolean isRunning;
        private int fleeDistance;
        private double range;
        private Boolean locked;
        
        private Vector2 offset;
        #endregion

        #region Constructors
        public SnipComponent()
        {
            locked = false;
            laserpres = false;
            isRunning = false;
            fleeDistance = 200;
            range = 400;
            
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
