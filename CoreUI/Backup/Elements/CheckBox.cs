using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CoreUI.Elements
{
    public class CheckBox : RadioCheckBase
    {

        //Textures
        private static int CheckNormCheck;
        private static int CheckNormBlank;
        private static int CheckNormInd;
        private static int CheckHoverCheck;
        private static int CheckHoverBlank;
        private static int CheckHoverInd;
        private static int CheckDsbldCheck;
        private static int CheckDsbldBlank;
        private static int CheckDsbldInd;
        private static int CheckPressCheck;
        private static int CheckPressBlank;
        private static int CheckPressInd;

        static CheckBox()
        {
            CheckNormCheck = CoreUIEngine.LoadTexture(Properties.Resources.CheckNormCheck);
            CheckNormBlank = CoreUIEngine.LoadTexture(Properties.Resources.CheckNormBlank);
            CheckNormInd = CoreUIEngine.LoadTexture(Properties.Resources.CheckNormInd);
            CheckHoverCheck = CoreUIEngine.LoadTexture(Properties.Resources.CheckHoverCheck);
            CheckHoverBlank = CoreUIEngine.LoadTexture(Properties.Resources.CheckHoverBlank);
            CheckHoverInd = CoreUIEngine.LoadTexture(Properties.Resources.CheckHoverInd);
            CheckDsbldCheck = CoreUIEngine.LoadTexture(Properties.Resources.CheckDsbldCheck);
            CheckDsbldBlank = CoreUIEngine.LoadTexture(Properties.Resources.CheckDsbldBlank);
            CheckDsbldInd = CoreUIEngine.LoadTexture(Properties.Resources.CheckDsbldInd);
            CheckPressCheck = CoreUIEngine.LoadTexture(Properties.Resources.CheckPressCheck);
            CheckPressBlank = CoreUIEngine.LoadTexture(Properties.Resources.CheckPressBlank);
            CheckPressInd = CoreUIEngine.LoadTexture(Properties.Resources.CheckPressInd);
        }

        protected internal override void Render()
        {
            base.Render();
            if (Bounds.Width > 12 && Bounds.Height > 12)
            {
                if (!IsEnabled)
                {
                    if (IsChecked == null)
                        CoreUIEngine.mScreen2D.Draw_Texture(CheckDsbldInd, Bounds.Left, Bounds.Top, Bounds.Left + 12, Bounds.Top + 12);
                    else if (IsChecked == true)
                        CoreUIEngine.mScreen2D.Draw_Texture(CheckDsbldCheck, Bounds.Left, Bounds.Top, Bounds.Left + 12, Bounds.Top + 12);
                    else
                        CoreUIEngine.mScreen2D.Draw_Texture(CheckDsbldBlank, Bounds.Left, Bounds.Top, Bounds.Left + 12, Bounds.Top + 12);
                }
                else if (mIsPressed)
                {
                    if (IsChecked == null)
                        CoreUIEngine.mScreen2D.Draw_Texture(CheckPressInd, Bounds.Left, Bounds.Top, Bounds.Left + 12, Bounds.Top + 12);
                    else if (IsChecked == true)
                        CoreUIEngine.mScreen2D.Draw_Texture(CheckPressCheck, Bounds.Left, Bounds.Top, Bounds.Left + 12, Bounds.Top + 12);
                    else
                        CoreUIEngine.mScreen2D.Draw_Texture(CheckPressBlank, Bounds.Left, Bounds.Top, Bounds.Left + 12, Bounds.Top + 12);
                }
                else if (IsMouseOver)
                {
                    if (IsChecked == null)
                        CoreUIEngine.mScreen2D.Draw_Texture(CheckHoverInd, Bounds.Left, Bounds.Top, Bounds.Left + 12, Bounds.Top + 12);
                    else if (IsChecked == true)
                        CoreUIEngine.mScreen2D.Draw_Texture(CheckHoverCheck, Bounds.Left, Bounds.Top, Bounds.Left + 12, Bounds.Top + 12);
                    else
                        CoreUIEngine.mScreen2D.Draw_Texture(CheckHoverBlank, Bounds.Left, Bounds.Top, Bounds.Left + 12, Bounds.Top + 12);
                }
                else
                {
                    if (IsChecked == null)
                        CoreUIEngine.mScreen2D.Draw_Texture(CheckNormInd, Bounds.Left, Bounds.Top, Bounds.Left + 12, Bounds.Top + 12);
                    else if (IsChecked == true)
                        CoreUIEngine.mScreen2D.Draw_Texture(CheckNormCheck, Bounds.Left, Bounds.Top, Bounds.Left + 12, Bounds.Top + 12);
                    else
                        CoreUIEngine.mScreen2D.Draw_Texture(CheckNormBlank, Bounds.Left, Bounds.Top, Bounds.Left + 12, Bounds.Top + 12);
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