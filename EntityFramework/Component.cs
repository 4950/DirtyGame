using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace EntityFramework
{
    /// <summary>
    /// Purely data objects 
    /// </summary>
    [Serializable]
    public abstract class Component : ICloneable
    {
        public static List<Type> ComponentTypes = new List<Type>();
        // Future
        //public abstract void Deserialize(BinaryReader reader);
        //public abstract void Serialize(BinaryWriter writer);

        public Component()
        {
            if (!ComponentTypes.Contains(this.GetType()))
                ComponentTypes.Add(this.GetType());
        }

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
                return 1L << (int)Id | 1L << (int)Mappers.ComponentTypeMapper.GetValue(GetType().BaseType);
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
