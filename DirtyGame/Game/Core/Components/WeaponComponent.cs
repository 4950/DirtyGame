using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtyGame.game.Core.Components
{
    public class WeaponComponent : Component
    {
        public enum WeaponType
        {
            Ranged,
            Melee,
            AOE
        }
        #region Constructors
        public WeaponComponent()
        {
            
        }
        #endregion

        #region Properties
        public String Name { get; set; }
        public String Portrait { get; set; }
        public float BaseDamage { get; set; }
        public float Range { get; set; }
        public WeaponType Type { get; set; }
        public String ProjectileSprite { get; set; }
        public float ProjectileSpeed { get; set; }
        #endregion

        #region Functions
        
        #endregion
    }
}
