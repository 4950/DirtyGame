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
        private string name;
        // Future
        //public abstract void Deserialize(BinaryReader reader);
        //public abstract void Serialize(BinaryWriter writer);

        public Component()
        {
            name = this.GetType().Name;
            if (!ComponentTypes.Contains(this.GetType()))
                ComponentTypes.Add(this.GetType());
        }

        public virtual void DidDeserialize()
        {
        }

        public int Id
        {
            get
            {
                return name.GetHashCode();
                //return Mappers.ComponentTypeMapper.GetValue(GetType());
            }
        }
        /// <summary>
        /// Do NOT change name, except before adding component. You have been warned
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public long Bit
        {
            get
            {
                return 1L << (int)Mappers.ComponentTypeMapper.GetValue(GetType()) | 1L << (int)Mappers.ComponentTypeMapper.GetValue(GetType().BaseType);
            }
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
