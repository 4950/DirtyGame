using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class Aspect
    {
        #region Variables
        private BitVector bitVector;
        #endregion

        public BitVector BitVector
        {
            get
            {
                return bitVector;
            }
        }

        #region Constructors
        internal Aspect()
        {
            bitVector = new BitVector();
        }
        #endregion

        #region Functions
        #region Static
        public static Aspect CreateAspectFor(IEnumerable<Type> types)
        {
            Aspect aspect = new Aspect();
            foreach (Type type in types)
            {
                aspect.AddComponent(type);
            }
            return aspect;
        }
        #endregion

        public bool Contains(Aspect aspect)
        {
            return Contains(aspect.bitVector);
        }

        public bool Contains(BitVector bv)
        {
            return bitVector.Contains(bv);
        }

        public Aspect AddComponent(Type type)
        {
            bitVector.AddBitByOffset((int)Mappers.ComponentTypeMapper.GetValue(type));
            return this;
        }

        public Aspect AddComponent<T>() where T : Component
        {
            return AddComponent(typeof(T));
        }

        public Aspect RemoveComponent(Type type)
        {
            bitVector.RemoveBitByOffset((int)Mappers.ComponentTypeMapper.GetValue(type));
            return this;
        }  

        public Aspect RemoveComponent<T>() where T : Component
        {
            return RemoveComponent(typeof(T));
        }
        #endregion

    }
}
