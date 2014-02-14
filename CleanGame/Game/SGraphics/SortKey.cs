using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanGame.Game.SGraphics
{
    public class SortKey : IComparable<SortKey>
    {
        private const int LAYER_OFFSET = 56;
        private const ulong LAYER_MASK = 0xFF00000000000000;
        private const ulong LAYER_MASK_CLEAR = 0x00FFFFFFFFFFFFFF;

        private ulong key;

        public ulong Key
        {
            get
            {
                return key;
            }
        }

        public SortKey()
        {
            key = 0;
        }

        public void SetRenderLayer(RenderLayer layer)
        {
            key &= LAYER_MASK_CLEAR;
            key |= ((ulong)layer << LAYER_OFFSET) & LAYER_MASK;
        }

        public RenderLayer GetRenderLayer()
        {
            ulong p = key & LAYER_MASK;
            p = p >> LAYER_OFFSET;
            return (RenderLayer)p; 
        }

        public int CompareTo(SortKey other)
        {
            if (Key > other.Key)
            {
                return 1;
            } else if (Key < other.Key)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
