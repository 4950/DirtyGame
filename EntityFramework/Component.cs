using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EntityFramework
{
    /// <summary>
    /// Purely data objects 
    /// </summary>
    public abstract class Component
    {
        // Future
        //public abstract void Deserialize(BinaryReader reader);
        //public abstract void Serialize(BinaryWriter writer);


        public uint Id
        {
            get
            {
                return Mappers.ComponentTypeMapper.GetValue(GetType());
            }
        }

        public long Bit
        {
            get
            {
                return 1L << (int)Mappers.ComponentTypeMapper.GetValue(GetType());
            }
        }
    }
}
