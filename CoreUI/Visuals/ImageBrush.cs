using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUI.Visuals
{
    public class ImageBrush : Visual
    {
        private IUITexture tex;

        protected internal override void Render()
        {
            if (tex != null)
                CoreUIEngine.mDrawEngine.Draw_Texture(tex, InnerBounds.Left, InnerBounds.Top, InnerBounds.Right, InnerBounds.Bottom);
        }
        public IUITexture Texture
        {
            set
            {
                tex = value;
                InvalidateVisual();
            }
        }
        public void LoadImage(String ImagePath)
        {
            if (tex != null)
                tex.Delete();
            tex = CoreUIEngine.mDrawEngine.CreateTexture(ImagePath);
            InvalidateVisual();
        }
        public void ClearImage()
        {
            tex = null;
            InvalidateVisual();
        }
    }
}
