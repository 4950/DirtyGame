using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUI.Visuals
{
    public class ImageBrush : Visual
    {
        private int tex = -1;

        protected internal override void Render()
        {
            if (tex != -1)
                CoreUIEngine.mScreen2D.Draw_Texture(tex, InnerBounds.Left, InnerBounds.Top, InnerBounds.Right, InnerBounds.Bottom);
        }
        public int Texture
        {
            set
            {
                if (value >= 0)
                    tex = value;
                else
                    tex = -1;
            }
        }
        public void LoadImage(String ImagePath)
        {
            if (tex != -1)
                CoreUIEngine.mTextures.DeleteTexture(tex);
            tex = CoreUIEngine.mTextures.LoadTexture(ImagePath);
        }
        public void ClearImage()
        {
            tex = -1;
        }
    }
}
