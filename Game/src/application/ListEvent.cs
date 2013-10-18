using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ShittyPrototype.src.application
{

    public delegate void ChangedEventHandler(object sender, EventArgs e, string s);

    class ListEvent<T> : List<T>
    {

    

        public event ChangedEventHandler Changed;
 
        protected virtual void OnChanged(EventArgs e, string s)
        {
            if (Changed != null)
                Changed(this, e, s);
        }

        public void Add(T type)
        {
            base.Add(type);
            OnChanged(EventArgs.Empty,"add");
        }

        public void Remove(T type)
        {
            base.Remove(type);
            OnChanged(EventArgs.Empty, "remove");
        }

        public void Clear()
        {
            base.Clear();
        }

    }
}
