using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityFramework
{
    public class BitVector
    {
        #region Variables
        private long bits;
        #endregion

        #region Properties
        public long Bits
        {
            get
            {
                return bits;               
            }
        }
        #endregion

        #region Constructors
        public BitVector()
        {
            bits = 0;
        }
        #endregion

        #region Functions
        public void AddBits(BitVector bitVector)
        {
            bits |= bitVector.Bits;
        }

        public void RemoveBits(BitVector bitVector)
        {
            bits &= bitVector.Bits;
        }

        // Note: If we every go above 64 components/systems, cant use this anymore
        public void AddBit(long bit)
        {
            bits |= bit;
        }

        // Note: If we every go above 64 components/systems, cant use this anymore
        public void RemoveBit(long bit)
        {
            bits ^= bit;
        }

        public void AddBitByOffset(int offset)
        {
            AddBit(1L << offset);
        }

        public void RemoveBitByOffset(int offset)
        {
            RemoveBit(1L << offset);
        }

        public bool Contains(BitVector v)
        {
            return Contains(v.Bits);
        }

        /// <summary>
        /// Determine if the provided bits are all present in this bitvect. This uses a "material nonimplication" s(A&~B)
        /// </summary>
        /// <param name="bitsToCompare"></param>
        /// <returns>Returns true if all the bits in the param are present but false if any bits are not present</returns>
        public bool Contains(long bitsToCompare)
        {
            return (bits & ~bitsToCompare) == 0;
        }

        #endregion
    }
}
