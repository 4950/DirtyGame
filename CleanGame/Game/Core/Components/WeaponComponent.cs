using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CleanGame.Game.SGraphics;
using System.Xml.Serialization;

namespace CleanGame.Game.Core.Components
{
    public class WeaponComponent : Component
    {
        public enum WeaponType
        {
            Ranged,
            Melee,
            AOE,
            Landmine
        }
        #region Constructors
        public WeaponComponent()
        {
            
        }
        #endregion

        #region Properties
        public String WeaponName { get; set; }
        public String Portrait { get; set; }
        public float BaseDamage { get; set; }
        public float Range { get; set; }
        public WeaponType Type { get; set; }
        public String ProjectileSprite { get; set; }
        public String SpriteXml { get; set; }
        public float ProjectileSpeed { get; set; }
        public float Cooldown { get; set; }
        public float LastFire { get; set; }
        public float Price { get; set; }
        public float AmmoPrice { get; set; }
        public Entity Owner { get; set; }
        public String FireSound { get; set; }
        /// <summary>
        /// Max Ammo level. -1 is infinite
        /// </summary>
        public int MaxAmmo { get; set; }
        public int Ammo { get; set; }
        #endregion

        #region Functions
        
        #endregion
    }
}
