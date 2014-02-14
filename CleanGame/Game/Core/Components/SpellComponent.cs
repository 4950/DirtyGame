using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CleanGame.Game.Core.Components
{
    public class SpellComponent : Component
    {
        [XmlIgnoreAttribute]
        private List<Entity> spells;
        [XmlIgnoreAttribute]
        private Entity currentSpell;

        public SpellComponent()
        {
            spells = new List<Entity>();
        }
        [XmlIgnoreAttribute]
        public List<Entity> SpellList { get { return spells; } }
        public void addSpell(Entity spell)
        {
            if (spell.HasComponent<WeaponComponent>())
            {
                spells.Add(spell);
                if (spells.Count == 1)
                    currentSpell = spell;
            }
        }
        public void setCurrentSpell(Entity spell)
        {
            if (spells.Contains(spell))
                currentSpell = spell;
        }
        [XmlIgnoreAttribute]
        public Entity CurrentSpell { get { return currentSpell; } }
        public void removeSpell(Entity spell)
        {
            spells.Remove(spell);
            if (currentSpell == spell)
            {
                if (spells.Count > 0)
                    currentSpell = spells[0];
                else
                    currentSpell = null;
            }
        }
    }
}
