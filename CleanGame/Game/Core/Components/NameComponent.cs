using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;

namespace CleanGame.Game.Core.Components
{
    public class NameComponent : Component
    {
        #region Variables
        //Sprite direction
        private string name = "DEFAULT";
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        #endregion
    }
}
