using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace CoreUI.Elements
{
    public class ProgressBar : Element
    {

        int min = 0;	// Minimum value for progress range
        int max = 100;	// Maximum value for progress range
        int val = 0;		// Current progress
        bool Vert = false;

        public ProgressBar()
        {
            Background = CoreUIEngine.mGlobals.RGBA(1, 0, 0, 1);
            Foreground = CoreUIEngine.mGlobals.RGBA(0, 1, 0, 1);
        }
        protected internal override void Render()
        {
            base.Render();
            if (Vert == false)
            {
                float percent = (float)(val - min) / (float)(max - min);
                Rectangle rect = InnerBounds;

                if (val != min)
                {
                    //Draw the background
                    CoreUIEngine.mScreen2D.Draw_FilledBox(rect.Left, rect.Top, rect.Right, rect.Bottom, Background);

                    // Calculate area for drawing the progress.
                    rect.Width = (int)((float)rect.Width * percent);

                    // Draw the progress meter.
                    CoreUIEngine.mScreen2D.Draw_FilledBox(rect.Left, rect.Top, rect.Right, rect.Bottom, Foreground);
                }
                else
                    CoreUIEngine.mScreen2D.Draw_FilledBox(rect.Left, rect.Top, rect.Right, rect.Bottom, Background);
                // Draw a three-dimensional border around the control.
                Draw3DBorder();
            }
            else
            {
                float percent = (float)(val - min) / (float)(max - min);
                Rectangle rect = InnerBounds;
                if (val != min)
                {
                    //Draw the background
                    CoreUIEngine.mScreen2D.Draw_FilledBox(rect.Left, rect.Top, rect.Right, rect.Bottom, Background);

                    // Calculate area for drawing the progress.
                    rect.Height = (int)((float)rect.Height * percent);
                    rect.Offset(0, InnerBounds.Height - rect.Height);
                    // Draw the progress meter.
                    CoreUIEngine.mScreen2D.Draw_FilledBox(rect.Left, rect.Top, rect.Right, rect.Bottom, Foreground);
                }
                else
                    CoreUIEngine.mScreen2D.Draw_FilledBox(rect.Left, rect.Top, rect.Right, rect.Bottom, Background);
                // Draw a three-dimensional border around the control.
                Draw3DBorder();
            }
        }
        private void Draw3DBorder()
        {
            int PenWidth = (int)Pens.White.Width;

            CoreUIEngine.mScreen2D.Draw_Line(InnerBounds.Left, InnerBounds.Top, InnerBounds.Right, InnerBounds.Top, Color.DarkGray.ToArgb());
            CoreUIEngine.mScreen2D.Draw_Line(InnerBounds.Left, InnerBounds.Top, InnerBounds.Left, InnerBounds.Bottom, Color.DarkGray.ToArgb());
            CoreUIEngine.mScreen2D.Draw_Line(InnerBounds.Left, InnerBounds.Bottom, InnerBounds.Right, InnerBounds.Bottom, Color.White.ToArgb());
            CoreUIEngine.mScreen2D.Draw_Line(InnerBounds.Right, InnerBounds.Top, InnerBounds.Right, InnerBounds.Bottom, Color.White.ToArgb());
        } 
        public int Minimum
        {
            get
            {
                return min;
            }

            set
            {
                // Prevent a negative value.
                if (value < 0)
                {
                    min = 0;
                }

                // Make sure that the minimum value is never set higher than the maximum value.
                if (value > max)
                {
                    min = value;
                    min = value;
                }

                // Ensure value is still in range
                if (val < min)
                {
                    val = min;
                }

                // Invalidate the control to get a repaint.
                InvalidateVisual();
            }
        }

        public bool Vertical
        {
            get
            {
                return Vert;
            }
            set
            {
                Vert = value;
                InvalidateVisual();
            }
        }

        public int Maximum
        {
            get
            {
                return max;
            }

            set
            {
                // Make sure that the maximum value is never set lower than the minimum value.
                if (value < min)
                {
                    min = value;
                }

                max = value;

                // Make sure that value is still in range.
                if (val > max)
                {
                    val = max;
                }

                // Invalidate the control to get a repaint.
                InvalidateVisual();
            }
        }

        public int Value
        {
            get
            {
                return val;
            }

            set
            {
                int oldValue = val;

                // Make sure that the value does not stray outside the valid range.
                if (value < min)
                {
                    val = min;
                }
                else if (value > max)
                {
                    val = max;
                }
                else
                {
                    val = value;
                }

                InvalidateVisual();
            }
        }

    }
}
