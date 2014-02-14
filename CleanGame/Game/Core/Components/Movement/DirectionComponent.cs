using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;

namespace CleanGame.Game.Core.Components
{
    public class DirectionComponent : Component
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
