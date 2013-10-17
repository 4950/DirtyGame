using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShittyPrototype
{
    class Singleton<T> where T : class, new()
    {
        private static T _instance = new T();
        public static T GetSingleton()
        {
            return _instance;
        }
    }
}
