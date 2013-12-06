using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.Core.Components
{
    public class SpellComponent : Component
    {
        private List<Entity> spells;
        private Entity currentSpell;

        public SpellComponent()
        {
            spells = new List<Entity>();
        }
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
