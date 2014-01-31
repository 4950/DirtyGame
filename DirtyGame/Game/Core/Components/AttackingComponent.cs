using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;

namespace DirtyGame.game.Core.Components
{
    public class AttackingComponent : Component
    {
        #region Variables
        //If entity is attacking
        private bool attacking = false;
        #endregion

        #region Properties
        public bool isAttacking
        {
            get
            {
                return attacking;
            }
            set
            {
                attacking = value;
            }
        }
        #endregion
    }
}
