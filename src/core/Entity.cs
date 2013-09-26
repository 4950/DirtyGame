using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShittyPrototype.src.core
{
    class Entity
    {
        private Dictionary<Type, IComponent> components;

        public Entity()
        {
            components = new Dictionary<Type, IComponent>();
        }

        public void AddComponent(IComponent comp)
        {
            components.Add(comp.GetType(), comp);
        }

        public bool HasComponent<T>()
        {
            return components.ContainsKey(typeof(T));
        }

        public IComponent GetComponent<T>()
        {
            IComponent comp = null;
            components.TryGetValue(typeof(T), out comp);
            return comp;
        }
    }
}
