using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Managers;
using EntityFramework.Systems;

namespace EntityFramework
{
    public class World
    {
        private EntityManager entityMgr;
        private SystemManager systemMgr;  

        public EntityManager EntityMgr
        {
            get
            {
                return entityMgr;
            }
        }
       
        public World()
        {
            entityMgr = new EntityManager(this);
            systemMgr = new SystemManager(this);
        }

        /// <summary>
        /// Creates a new Entity
        /// </summary>
        /// <returns>New Entity</returns>
        public Entity CreateEntity()
        {
            return entityMgr.CreateEntity();
        }

        public void AddSystem(EntitySystem system)
        {
            systemMgr.AddSystem(system);
        }

        public void RemoveSystem(EntitySystem system)
        {
            systemMgr.RemoveSystem(system);
        }

        public void RemoveEntity(Entity e)
        {
            // remove entity from systems  
            CheckSystems(e, systemMgr.GetSystems(e.SystemBits));
            entityMgr.RemoveEntity(e.Id);
        }

        public void Refresh(Entity e)
        {
            CheckSystems(e, systemMgr.Systems);
        }

        public void Update(float dt)
        {
            foreach (EntitySystem system in systemMgr.Systems)
            {
                system.Update(dt);
            }
        }

        private void CheckSystems(Entity e, IEnumerable<EntitySystem> systemsToCheck)
        {
            foreach (EntitySystem system in systemsToCheck)
            {
                system.Check(e);
            }
        }               
    }
}
