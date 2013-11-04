using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.Map.Generators.BSP
{
    class BSPNode<T>
    {
        public T Data
        {
            get;
            private set;
        }

        public BSPNode<T> Left
        {
            get;
            set;
        }

        public BSPNode<T> Right
        {
            get;
            set;
        }

        public bool IsLeaf
        {
            get
            {
                return Left == null && Right == null;
            }
        }

        public BSPNode(T data)
        {
            Data = data;
            Left = null;
            Right = null;
        }
    }
}
