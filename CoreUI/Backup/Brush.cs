using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUI
{
    public abstract class Brush
    {
        private Visual mParent;

        public Visual Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }
        protected internal virtual void Draw() { }

    }
}
