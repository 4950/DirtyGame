using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityFramework.Managers
{
    public class GroupManager<T>
    {        
        private Dictionary<string, HashSet<T>> groupToIds;
        private Dictionary<T, HashSet<string>> idToGroups;

        public GroupManager()
        {
            groupToIds = new Dictionary<string, HashSet<T>>();
            idToGroups = new Dictionary<T, HashSet<string>>();
        }

        public void AddToGroup(T id, string group)
        {
            if (!idToGroups.ContainsKey(id))
            {
                idToGroups.Add(id, new HashSet<string>());                            
            }
            if (!groupToIds.ContainsKey(group))
            {
                groupToIds.Add(group, new HashSet<T>());
            }                              
            idToGroups[id].Add(group);
            groupToIds[group].Add(id);                                              
        }

        public void RemoveFromGroup(T id, string group)
        {
            if (idToGroups.ContainsKey(id) && idToGroups[id].Contains(group))
            {
                idToGroups[id].Remove(group);
                if (idToGroups[id].Count == 0)
                {
                    idToGroups.Remove(id);
                }
                groupToIds[group].Remove(id);
                if (groupToIds[group].Count == 0)
                {
                    groupToIds.Remove(group);
                }
            }
        }

        public void RemoveFromAllGroups(T id)
        {
            if (idToGroups.ContainsKey(id))
            {
                HashSet<string> groups = idToGroups[id];
                idToGroups.Remove(id);
                foreach (string s in groups)
                {                    
                    RemoveFromGroup(id, s);
                }
            }
        }

        public IEnumerable<T> GetGroupMembers(string group)
        {
            if (!groupToIds.ContainsKey(group))
            {
                return new List<T>();
            }
            return groupToIds[group];
        }

        public IEnumerable<string> GetGroups(T id)
        {
            if (!idToGroups.ContainsKey(id))
            {
                return new List<string>();
            }
            return idToGroups[id];
        } 
    }
}
