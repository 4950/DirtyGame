using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShittyPrototype.src.core
{
    class Entity
    {
        private ICollection<IComponent> components;

        public Entity()
        {
            components = new List<IComponent>();
        }

        public void AddComponent(IComponent comp)
        {
            components.Add(comp);
        }

        public IComponent GetComponent<T>()
        {
            foreach (IComponent comp in components)
            {
                if (comp is T)
                {
                    return comp;
                }
            }
            return null;
        }
    }
}
