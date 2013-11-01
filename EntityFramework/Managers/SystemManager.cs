using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Systems;

namespace EntityFramework.Managers
{
    internal class SystemManager
    {
        private World world;
        private TypeMapper<EntitySystem> systemMapper;
        private Dictionary<uint, EntitySystem> systems;

        public IEnumerable<EntitySystem> Systems
        {
            get
            {
                return systems.Values;  
            } 
        } 

        public SystemManager(World world)
        {
            systems = new Dictionary<uint, EntitySystem>();
            this.world = world;
            systemMapper = Mappers.SystemTypeMapper;
        }

        public void AddSystem(EntitySystem system)
        {
            if (systems.ContainsKey(system.Id))
            {
                systems.Remove(system.Id);
            }
            systems.Add(system.Id, system);
            system.EntityMgr = world.EntityMgr;
            system.World = world;
            system.Initialize();
            Sort();

        }

        public void RemoveSystem(EntitySystem system)
        {
            system.Shutdown();
            system.RemoveAllEntities();
            systems.Remove(system.Id);
            Sort();
        }

        public IEnumerable<EntitySystem> GetSystems(BitVector bitVector)
        {
            //TODO: doesnt work right now
            List<EntitySystem> selectedSystems = new List<EntitySystem>();               
            foreach (EntitySystem system in systems.Values)
            {
                if (bitVector.Contains(system.Aspect.BitVector))
                {
                    selectedSystems.Add(system);
                }
            }
            return selectedSystems;
        }

        private void Sort()
        {
            // maintain priority sorting
            systems.ToList().Sort((firstPair, nextPair) =>
            {
                return firstPair.Value.CompareTo(nextPair.Value);
            }
            );
        }
        
    }
}
