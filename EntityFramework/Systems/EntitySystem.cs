using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Managers;

namespace EntityFramework.Systems
{
    public abstract class EntitySystem : IComparable<EntitySystem>
    {
        #region Variables
        private List<Entity> entities;
        private Aspect aspect;
        private int priorityLevel;
        #endregion

        #region Properties
        public EntityManager EntityMgr
        {
            get;
            set;
        }

        public World World
        {
            get;
            set;
        }

        public Aspect Aspect
        {
            get
            {
                return aspect;
            }
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
        #endregion

        #region Constructors
        protected EntitySystem(Aspect aspect, int priorityLevel)
        {
            this.aspect = aspect;
            this.priorityLevel = priorityLevel;
            entities = new List<Entity>();
        }
        #endregion

        #region Functions
        internal void Check(Entity e)
        {
            if (!aspect.Contains(e.ComponentBits))
            {
                if (entities.Contains(e))
                {
                    // remove
                    entities.Remove(e);
                    e.SystemBits.RemoveBit(Bit);
                    OnEntityRemoved(e);
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

        // quick fix for "World.RemoveEntity" problems
        internal void RemoveEntity(Entity e)
        {
            if (entities.Contains(e))
            {
                OnEntityRemoved(e);
                entities.Remove(e);
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


        /// <summary>
        /// Called once per frame to allow the system to act upon all of its entities
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="dt"></param>
        public abstract void ProcessEntities(IEnumerable<Entity> entities, float dt);  

        /// <summary>
        /// Called when system is added to the world
        /// </summary>
        public virtual void Initialize() {}

        /// <summary>
        /// Called when the system is removed from the wolrd
        /// </summary>
        public virtual void Shutdown() {}

        /// <summary>
        /// Called everytime an entity is added to the system
        /// </summary>
        /// <param name="e"></param>
        public abstract void OnEntityAdded(Entity e);

        /// <summary>
        /// Called everytime an entity is removed from the system
        /// </summary>
        /// <param name="e"></param>
        public abstract void OnEntityRemoved(Entity e);
    

        public int CompareTo(EntitySystem other)
        {
            return priorityLevel - other.priorityLevel;
        }
        #endregion
    }
}
