using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DirtyGame.game.SGraphics;
using System.Xml.Serialization;

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
        public float Cooldown { get; set; }
        public float LastFire { get; set; }
        public float Price { get; set; }
        public float AmmoPrice { get; set; }
        /// <summary>
        /// Max Ammo level. -1 is infinite
        /// </summary>
        public int MaxAmmo { get; set; }
        public int Ammo { get; set; }
        [XmlIgnoreAttribute]
        public SpriteSheet MeleeSheet;
        #endregion

        #region Functions
        
        #endregion
    }
}
