using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dirtygame.game.Util
{
    public class Singleton<T> where T : class, new()
    {
        private static T instance = new T();
        public static T Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
