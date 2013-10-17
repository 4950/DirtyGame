using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MTV3D65;

namespace CoreUI
{
    public class Panel : Element
    {

        protected internal List<Visual> Visuals = new List<Visual>();

        public Panel()
        {
            Background = new TV_COLOR(0, 0, 0, 0).GetIntColor();
        }
        protected internal override void Render()
        {
            CoreUIEngine.mScreen2D.Draw_FilledBox(InnerBounds.Left, InnerBounds.Top, InnerBounds.Right, InnerBounds.Bottom, Background);
            base.Render();
            for (int i = 0; i < Visuals.Count; i++)
            {
                if (Visuals[i].Visibility == Visibility.Visible)
                    Visuals[i].Render();
            }
        }
        protected internal override void HitTest(ref List<Element> retlist, float X, float Y)
        {
            base.HitTest(ref retlist, X, Y);
            for (int i = 0; i < Visuals.Count; i++)
                if (Visuals[i] is Element)
                    ((Element)Visuals[i]).HitTest(ref retlist, X, Y);
        }
        protected internal override Element HitTest(float X, float Y)
        {
            Element e;
            for (int i = 0; i < Visuals.Count; i++)
            {
                if (Visuals[i] is Element)
                {
                    e = ((Element)Visuals[i]).HitTest(X, Y);
                    if (e != null)
                    {
                        return e;
                    }
                }
            }
            return base.HitTest(X, Y);
        }
        public void RemoveAllItems()
        {
            Visuals.Clear();
            InvalidateVisual();
        }
        public void AddElement(Element e)
        {
            if (!Visuals.Contains(e))
            {
                e.Parent = this;
                Visuals.Add(e);
                InvalidateVisual();
            }
        }
        public void AddVisual(Visual v)
        {
            if (!Visuals.Contains(v))
            {
                v.Parent = this;
                Visuals.Add(v);
                InvalidateVisual();
            }
        }
        public void RemoveElement(Element e)
        {
            Visuals.Remove(e);
            InvalidateVisual();
        }
        public void RemoveVisual(Visual v)
        {
            Visuals.Remove(v);
            InvalidateVisual();
        }
    }
}
