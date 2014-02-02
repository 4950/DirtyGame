using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Managers;
using System.Xml.Serialization;

namespace EntityFramework
{
    public class EntityRef
    {
        private uint id;
        [XmlIgnoreAttribute]
        private EntityManager manager;

        public Entity entity
        {
            get
            {
                return manager.GetEntity(id);
            }
        }
        public EntityRef(Entity e)
        {
            id = e.Id;
            manager = e.entityManager;
        }
    }
    /// <summary>
    /// Lightweight object for grouping sets on components together
    /// </summary>
    public class Entity
    {
        #region Variables
        internal EntityManager entityManager;
        private Guid guid;
        private string name = null;
        #endregion
        internal void setName(string name)
        {
            this.name = name;
        }
        #region Properties
        public BitVector ComponentBits
        {
            get
            {
                return entityManager.GetComponentBitVector(Id);
            }
        }

        public BitVector SystemBits
        {
            get
            {
                return entityManager.GetSystemBitVector(Id);
            }
        }
        /// <summary>
        /// Data entities are not visible to systems
        /// </summary>
        public bool DataEntity { get; set; }
        public Guid GUID
        {
            get { return guid; }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                entityManager.SetEntityName(value, Id);
            }
        }
        public uint Id
        {
            get;
            set;
        }
        public EntityRef reference
        {
            get
            {
                return new EntityRef(this);
            }
        }
        public string Tag
        {
            get
            {
                return entityManager.TagManager.GetTag(Id);
            }
            set
            {
                if (value == "")
                {
                    entityManager.TagManager.RemoveTag(Id);
                }
                else
                {
                    entityManager.TagManager.AddTag(Id, value);
                }

            }
        }

        public IEnumerable<Component> Components
        {
            get
            {
                return entityManager.GetComponents(Id);
            }
        }

        public IEnumerable<string> Groups
        {
            get
            {
                return entityManager.GroupManager.GetGroups(Id);
            }
        }
        #endregion

        #region Constructors
        internal Entity()
        {
        }
        internal Entity(EntityManager em)
        {
            guid = Guid.NewGuid();
            Id = (uint)guid.GetHashCode();
            entityManager = em;
        }
        internal Entity(EntityManager em, Guid old_id)
        {
            entityManager = em;
            guid = old_id;
            Id = (uint)old_id.GetHashCode();
        }
        #endregion

        #region Functions
        public bool HasComponent(Type type)
        {
            return entityManager.HasComponent(Id, type);
        }

        public bool HasComponent<T>()
        {
            return HasComponent(typeof(T));
        }

        public T GetComponent<T>() where T : Component
        {
            return (T)entityManager.GetComponent(Id, typeof(T));
        }
        public T GetComponent<T>(string name) where T : Component
        {
            return (T)entityManager.GetComponent(Id, name);
        }
        public void AddComponent(Component comp)
        {
            entityManager.AddComponent(Id, comp);
        }

        public void RemoveComponent(Component comp)
        {
            entityManager.RemoveComponent(Id, comp);
        }

        public void AddToGroup(string group)
        {
            entityManager.GroupManager.AddToGroup(Id, group);
        }

        public void RemoveFromGroup(string group)
        {
            entityManager.GroupManager.RemoveFromGroup(Id, group);
        }

        public void RemoveTag()
        {
            Tag = "";
        }

        public void Destroy()
        {
            entityManager.world.DestroyEntity(this);
        }

        /// <summary>
        /// Updates the entity in the world it belongs to.
        /// This should be called after adding components to the 
        /// entity so that it can be distributed to its proper processing systems.
        /// </summary>
        public void Refresh()
        {
            entityManager.Refresh(Id);
        }
        #endregion
    }
}
