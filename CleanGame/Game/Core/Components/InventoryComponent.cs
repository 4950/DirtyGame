using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CleanGame.Game.Core.Components
{
    public class InventoryComponent : Component
    {
        [XmlIgnoreAttribute]
        private List<Entity> weapons;
        [XmlIgnoreAttribute]
        private Entity currentWeapon;

        public InventoryComponent()
        {
            weapons = new List<Entity>();
        }
        [XmlIgnoreAttribute]
        public List<Entity> WeaponList { get { return weapons; } }
        public void addWeapon(Entity weapon, Entity owner)
        {
            if (weapon.HasComponent<WeaponComponent>())
            {
                weapon.GetComponent<WeaponComponent>().Owner = owner;
                weapons.Add(weapon);
                if (weapons.Count == 1)
                    currentWeapon = weapon;
            }
        }
        public void setCurrentWeapon(Entity weapon)
        {
            if (weapons.Contains(weapon))
                currentWeapon = weapon;
        }
        [XmlIgnoreAttribute]
        public Entity CurrentWeapon { get { return currentWeapon; } }
        public void removeWeapon(Entity weapon)
        {
            weapons.Remove(weapon);
            if (currentWeapon == weapon)
            {
                if (weapons.Count > 0)
                    currentWeapon = weapons[0];
                else
                    currentWeapon = null;
            }
        }
    }
}
