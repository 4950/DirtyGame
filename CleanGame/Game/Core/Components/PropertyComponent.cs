using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanGame.Game.Core.Components
{
    public class PropertyComponent : Component
    {
        private bool modified;
        public bool IsModified
        {
            get
            {
                return modified;
            }
            set
            {
                modified = value;
            }
        }
    }
    public class PropertyComponent<T> : PropertyComponent
    {
        private T val;

        private PropertyComponent()
        {
        }
        public PropertyComponent(string Name, T value)
        {
            this.Name = Name;
            val = value;
            IsModified = true;
        }
        public T value
        {
            get
            {
                return val;
            }
            set
            {
                val = value;
                IsModified = true;
            }
        }
        

    }
}
