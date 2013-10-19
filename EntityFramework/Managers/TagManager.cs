using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityFramework.Managers
{
    /// <summary>
    /// Allow unique tagged of generic objects to strings
    /// </summary>
    /// <typeparam name="T">Generic object ot uniquely tag</typeparam>
    public class TagManager<T>
    {
        private Dictionary<T, string> idToTag;
        private Dictionary<string, T> tagToid;

        public TagManager()
        {
            idToTag = new Dictionary<T, string>();
            tagToid = new Dictionary<string, T>();
        }

        public void AddTag(T id, string tag)
        {
            if (idToTag.ContainsKey(id))
            {
                // we are overwriting a tag
                RemoveTag(id);                
            }
            idToTag.Add(id, tag);
            tagToid.Add(tag, id);
        }

        public void RemoveTag(T id)
        {
            if (!idToTag.ContainsKey(id))
            {
                return;                               
            }

            string tag = idToTag[id];
            idToTag.Remove(id);
            tagToid.Remove(tag);
        }

        public T GetMemeber(string tag)
        {
            if (!tagToid.ContainsKey(tag))
            {
                // i dont like this
                throw new Exception();
            }
            return tagToid[tag];
        }

        public string GetTag(T id)
        {
            if (!idToTag.ContainsKey(id))
            {
                return "";
            }
            return idToTag[id];
        }
    }
}
