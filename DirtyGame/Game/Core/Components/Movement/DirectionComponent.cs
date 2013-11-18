using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;

namespace DirtyGame.game.Core.Components
{
    class DirectionComponent : Component
    {
        #region Variables
        //Sprite direction
        private string heading = "Down";
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
