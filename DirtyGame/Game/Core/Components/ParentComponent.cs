using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;

namespace DirtyGame.game.Core.Components
{
    class ParentComponent : Component
    {
        
         #region Constructors
        public ParentComponent(uint parent)
        {
            ParentId = parent;
        }
        #endregion

      

        #region Properties
        public uint ParentId
        {
            get;
            set;
        }
        #endregion
    }
}
