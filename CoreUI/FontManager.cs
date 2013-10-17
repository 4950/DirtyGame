using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CoreUI
{
    public class FontManager
    {
        private static int Fontnums = 0;
        public static int GetFontInt(Font f)
        {
            Fontnums++;
            //return CoreUIEngine.mText.NormalFont_Create("Font" + Fontnums.ToString(), f.FontFamily.Name, (int)f.Size, f.Bold, f.Underline, f.Italic);
            return 0;
        }
    }
}
