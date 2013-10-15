using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CoreUI.Elements
{
    public class RadioButton : RadioCheckBase
    {
        //Textures
        private static int RadioNormRadio;
        private static int RadioNormBlank;
        private static int RadioNormInd;
        private static int RadioHoverRadio;
        private static int RadioHoverBlank;
        private static int RadioHoverInd;
        private static int RadioDsbldRadio;
        private static int RadioDsbldBlank;
        private static int RadioDsbldInd;
        private static int RadioPressRadio;
        private static int RadioPressBlank;
        private static int RadioPressInd;

        static RadioButton()
        {
            RadioNormRadio = CoreUIEngine.LoadTexture(Properties.Resources.RadioNormRadio);
            RadioNormBlank = CoreUIEngine.LoadTexture(Properties.Resources.RadioNormBlank);
            RadioNormInd = CoreUIEngine.LoadTexture(Properties.Resources.RadioNormInd);
            RadioHoverRadio = CoreUIEngine.LoadTexture(Properties.Resources.RadioHoverRadio);
            RadioHoverBlank = CoreUIEngine.LoadTexture(Properties.Resources.RadioHoverBlank);
            RadioHoverInd = CoreUIEngine.LoadTexture(Properties.Resources.RadioHoverInd);
            RadioDsbldRadio = CoreUIEngine.LoadTexture(Properties.Resources.RadioDsbldRadio);
            RadioDsbldBlank = CoreUIEngine.LoadTexture(Properties.Resources.RadioDsbldBlank);
            RadioDsbldInd = CoreUIEngine.LoadTexture(Properties.Resources.RadioDsbldInd);
            RadioPressRadio = CoreUIEngine.LoadTexture(Properties.Resources.RadioPressRadio);
            RadioPressBlank = CoreUIEngine.LoadTexture(Properties.Resources.RadioPressBlank);
            RadioPressInd = CoreUIEngine.LoadTexture(Properties.Resources.RadioPressInd);
        }

        protected internal override void Render()
        {
            base.Render();
            if (Bounds.Width > 11 && Bounds.Height > 11)
            {
                if (!IsEnabled)
                {
                    if (IsChecked == null)
                        CoreUIEngine.mScreen2D.Draw_Texture(RadioDsbldInd, Bounds.Left, Bounds.Top, Bounds.Left + 11, Bounds.Top + 11);
                    else if (IsChecked == true)
                        CoreUIEngine.mScreen2D.Draw_Texture(RadioDsbldRadio, Bounds.Left, Bounds.Top, Bounds.Left + 11, Bounds.Top + 11);
                    else
                        CoreUIEngine.mScreen2D.Draw_Texture(RadioDsbldBlank, Bounds.Left, Bounds.Top, Bounds.Left + 11, Bounds.Top + 11);
                }
                else if (mIsPressed)
                {
                    if (IsChecked == null)
                        CoreUIEngine.mScreen2D.Draw_Texture(RadioPressInd, Bounds.Left, Bounds.Top, Bounds.Left + 11, Bounds.Top + 11);
                    else if (IsChecked == true)
                        CoreUIEngine.mScreen2D.Draw_Texture(RadioPressRadio, Bounds.Left, Bounds.Top, Bounds.Left + 11, Bounds.Top + 11);
                    else
                        CoreUIEngine.mScreen2D.Draw_Texture(RadioPressBlank, Bounds.Left, Bounds.Top, Bounds.Left + 11, Bounds.Top + 11);
                }
                else if (IsMouseOver)
                {
                    if (IsChecked == null)
                        CoreUIEngine.mScreen2D.Draw_Texture(RadioHoverInd, Bounds.Left, Bounds.Top, Bounds.Left + 11, Bounds.Top + 11);
                    else if (IsChecked == true)
                        CoreUIEngine.mScreen2D.Draw_Texture(RadioHoverRadio, Bounds.Left, Bounds.Top, Bounds.Left + 11, Bounds.Top + 11);
                    else
                        CoreUIEngine.mScreen2D.Draw_Texture(RadioHoverBlank, Bounds.Left, Bounds.Top, Bounds.Left + 11, Bounds.Top + 11);
                }
                else
                {
                    if (IsChecked == null)
                        CoreUIEngine.mScreen2D.Draw_Texture(RadioNormInd, Bounds.Left, Bounds.Top, Bounds.Left + 11, Bounds.Top + 11);
                    else if (IsChecked == true)
                        CoreUIEngine.mScreen2D.Draw_Texture(RadioNormRadio, Bounds.Left, Bounds.Top, Bounds.Left + 11, Bounds.Top + 11);
                    else
                        CoreUIEngine.mScreen2D.Draw_Texture(RadioNormBlank, Bounds.Left, Bounds.Top, Bounds.Left + 11, Bounds.Top + 11);
                }
            }
            CoreUIEngine.mScreen2D.Action_End2D();
            CoreUIEngine.mText.Action_BeginText();
            CoreUIEngine.mText.NormalFont_DrawText(DispText, TextPos.X, TextPos.Y, Foreground, mFontInt);
            CoreUIEngine.mText.Action_EndText();
            CoreUIEngine.mScreen2D.Action_Begin2D();
        }

    }
}
