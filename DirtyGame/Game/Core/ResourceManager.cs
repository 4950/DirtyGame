using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace DirtyGame.game.Core
{
    public class ResourceManager
    {
        private ContentManager content;
        private Dictionary<Type, Dictionary<string, object>> resources; 

        public ResourceManager(ContentManager content)
        {
            this.content = content;
            content.RootDirectory = "Content";
            resources = new Dictionary<Type, Dictionary<string, object>>();
        }

        /// <summary>
        /// Retrieves a resource from the resource manager. If it hasnt already been loaded, load it into the resource manager
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetResource<T>(string name)
        {
            if (!resources.ContainsKey(typeof (T)) || !resources[typeof(T)].ContainsKey(name))
            {
                return Load<T>(name);
            }
            return (T)resources[typeof (T)][name];
        }

        private T Load<T>(string name)
        {
            T resource = content.Load<T>(name);
            if (!resources.ContainsKey(typeof (T)))
            {
                resources.Add(typeof(T), new Dictionary<string, object>());
            }
              
            resources[typeof(T)].Add(name, resource);
            return resource;
        }
    }
}
