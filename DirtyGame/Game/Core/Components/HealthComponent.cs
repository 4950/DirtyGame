using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
namespace DirtyGame.game.Core.Components
{
    class HealthComponent : Component
    {
        #region Constructors
        public HealthComponent()
        {
            CurrentHealth = 100;
        }
        #endregion

        #region Properties
        public int MaxHealth
        {
            get;
            set;
        }

        public int CurrentHealth
        {
            get;
            set;
        }
        
        #endregion

        #region Functions

        public void LoseHealth(int numHealthLost)
        {
            if (CurrentHealth >= numHealthLost)
            {
                CurrentHealth = CurrentHealth - numHealthLost;
            }

            else
            {
                CurrentHealth = 0;
            }
        }

        #endregion
    }
}
