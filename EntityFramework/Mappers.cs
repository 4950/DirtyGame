using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Systems;

namespace EntityFramework
{
    internal class Mappers
    {       
        public static TypeMapper<Component> ComponentTypeMapper { get; private set; }
        public static TypeMapper<EntitySystem> SystemTypeMapper { get; private set; }
        static Mappers()
        {
            ComponentTypeMapper = new TypeMapper<Component>();
            SystemTypeMapper = new TypeMapper<EntitySystem>();           

        }
    }
}
