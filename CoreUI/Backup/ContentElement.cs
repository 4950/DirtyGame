using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUI
{
    public abstract class ContentElement : Element
    {
        private Element mContent;

        protected internal override void CheckPositioning()
        {
            base.CheckPositioning();
            if (Content != null)
                Content.CheckPositioning();
        }
        public Element Content
        {
            get
            {
                return mContent;
            }
            set
            {
                mContent = value;
                mContent.Parent = this;
                mContent.CheckPositioning();
            }
        }
        protected internal override void Render()
        {
            if (Content != null && Content.Visibility == Visibility.Visible)
                Content.Render();
            base.Render();
        }
        protected internal override void HitTest(ref List<Element> retlist, float X, float Y)
        {
            base.HitTest(ref retlist, X, Y);
            if (Content != null)
                Content.HitTest(ref retlist, X - Bounds.X, Y - Bounds.Y);
        }
        protected internal override Element HitTest(float X, float Y)
        {
            Element e = base.HitTest(X, Y);
            if (e != null)
            {
                if (Content != null)
                {
                    e = Content.HitTest(X - Bounds.X, Y - Bounds.Y);
                    if (e != null)
                        return e;
                }
                return this;
            }
            return null;
        }
    }
}