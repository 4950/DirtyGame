using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using CleanGame.Game.SGraphics;
using CleanGame.Game.Util;

namespace CleanGame.Game.Core
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        private ContentManager content;
        private Dictionary<Type, Dictionary<string, object>> resources;

        public ResourceManager()
        {
        }
        public ResourceManager(ContentManager content)
        {
            instance = this;
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
            if (!resources.ContainsKey(typeof(T)) || !resources[typeof(T)].ContainsKey(name))
            {
                if (typeof(SpriteSheet) != typeof(T))
                    return Load<T>(name);
                else
                    return default(T);
            }
            return (T)resources[typeof(T)][name];
        }
        public void AddResource<T>(T resource, string name)
        {
            if (!resources.ContainsKey(typeof(T)))
            {
                resources.Add(typeof(T), new Dictionary<string, object>());
            }
            if (!resources[typeof(T)].ContainsKey(name))
                resources[typeof(T)].Add(name, resource);
        }
        private T Load<T>(string name)
        {
            T resource = content.Load<T>(name);
            if (!resources.ContainsKey(typeof(T)))
            {
                resources.Add(typeof(T), new Dictionary<string, object>());
            }

            resources[typeof(T)].Add(name, resource);
            return resource;
        }
    }
}
