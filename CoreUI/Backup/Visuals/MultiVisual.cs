using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CoreUI.Visuals
{
    public class MultiVisual : Visual
    {
        private List<Visual> mVisuals = new List<Visual>();

        public MultiVisual()
        {
            base.SizeMode = SizeMode.Fill;
        }
        public void AddVisual(Visual v)
        {
            mVisuals.Insert(0, v);
            v.Parent = this;
            InvalidateVisual();
        }
        public void RemoveVisual(Visual v)
        {
            mVisuals.Remove(v);
            InvalidateVisual();
        }
        protected internal override void Render()
        {
            base.Render();
            foreach (Visual v in mVisuals)
                v.Render();
        }
        public override SizeMode SizeMode
        {
            get
            {
                return base.SizeMode;
            }
            set
            {
            }
        }
    }
}
