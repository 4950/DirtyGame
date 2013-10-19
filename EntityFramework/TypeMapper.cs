using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityFramework
{    
    public class TypeMapper<T>
    {
        private uint key;
        private Dictionary<Type, uint> typeMap;

        public TypeMapper()
        {
            key = 0;
            typeMap = new Dictionary<Type, uint>();
        }

        public uint GetValue(T type)
        {
            return GetValue(typeof (T));
        }

        public uint GetValue<T2>() where T2 : T
        {
            return GetValue(typeof(T2));
        }

        public uint GetValue(Type type)
        {
            if (!typeof (T).IsAssignableFrom(type))
            {
                throw new Exception();
            }

            if (typeMap.ContainsKey(type))
            {
                return typeMap[type];
            }
            else
            {
                RegisterType(type);
                return GetValue(type);
            }
        }
       
        private void RegisterType(Type type)
        {
            typeMap.Add(type, key++);
        }        
    }
}
