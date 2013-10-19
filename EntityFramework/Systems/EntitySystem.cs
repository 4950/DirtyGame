using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Managers;

namespace EntityFramework.Systems
{
    public abstract class EntitySystem : IComparable<EntitySystem>
    {
        private List<Entity> entities;
        private Aspect aspect;
        private int priorityLevel;

        public EntityManager EntityMgr
        {
            get;
            set;
        }

        public long Bit
        {
            get
            {
                return 1L << (int)Mappers.SystemTypeMapper.GetValue(GetType());
            }
        }

        public uint Id
        {
            get
            {
                return Mappers.SystemTypeMapper.GetValue(GetType());
            }    
        }

        protected EntitySystem(Aspect aspect, int priorityLevel)
        {
            this.aspect = aspect;
            this.priorityLevel = priorityLevel;
            entities = new List<Entity>();
        }

        internal void Check(Entity e)
        {
            if (!aspect.Contains(e.ComponentBits))
            {
                if (entities.Contains(e))
                {
                    // remove
                    entities.Remove(e);
                    e.SystemBits.RemoveBit(Bit);
                    OnEntityAdded(e);
                }
            }
            else
            {
                if (!entities.Contains(e))
                {
                    // add
                    entities.Add(e);
                    e.SystemBits.AddBit(Bit);
                    OnEntityAdded(e);
                }
            }                       
        }

        // I dont like this here
        internal void RemoveAllEntities()
        {
            foreach (Entity e in entities)
            {
                e.SystemBits.RemoveBit(Bit);
            }
            entities.Clear();
        }

        public void Update(float dt)
        {
            ProcessEntities(entities, dt);
        }


        public abstract void ProcessEntities(IEnumerable<Entity> entities, float dt);  
        public virtual void Initialize() {}
        public virtual void Shutdown() {}
        public abstract void OnEntityAdded(Entity e);
        public abstract void OnEntityRemoved(Entity e);
    

        public int CompareTo(EntitySystem other)
        {
            return priorityLevel - other.priorityLevel;
        }
    }
}
