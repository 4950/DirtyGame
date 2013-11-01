using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;

namespace DirtyGame.game.Core.Components
{
    class DirectionComponent : Component
    {
        #region
        //Sprite direction
        private string heading;
        #endregion

        #region Properties
        public string Heading
        {
            get
            {
                return heading;
            }
            set
            {
                heading = value;
            }
        }
        #endregion
    }
}
