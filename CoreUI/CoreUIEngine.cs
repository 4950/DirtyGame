using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace CoreUI
{
    public sealed class CoreUIEngine
    {
        public Panel Children;
        internal static List<Window> Windows = new List<Window>();
        internal static List<Window> ModalQueue = new List<Window>();
        private static Window mModalWindow;
        IUIRenderSurface rs;
        /*TVRenderSurface rs;
        internal static TVScene mScene;
        internal static TVScreen2DImmediate mScreen2D;
        internal static TVScreen2DText mText;
        internal static TVTextureFactory mTextures;
        internal static TVInputEngine mInput;*/
        internal static IDrawEngine mDrawEngine;
        internal static int mWidth, mHeight;
        /*internal static TVEngine mTV;
        internal static TVGlobals mGlobals;*/

        private static Window FocusedWindow;
        private static Element FocusedElement;
        private static Element MouseCaptureElement;
        private static List<Element> MouseElements = new List<Element>();
        private bool mMouseB1, mMouseB2, mMouseB3, tmpMouseB1, tmpMouseB2, tmpMouseB3;
        private int mMouseX, mMouseY, tmpMouseX, tmpMouseY;
        internal static bool ForceRedraw = true;
        internal static float TimeElapsed;


        public CoreUIEngine(IDrawEngine draw, int width, int height/*TVEngine TV, TVScene Scene*/)
        {
            mDrawEngine = draw;
            mDrawEngine.setSize(width, height);
            mWidth = width;
            mHeight = height;
            /*mTV = TV;
            mScene = Scene;
            mWidth = mTV.GetViewport().GetWidth();
            mHeight = mTV.GetViewport().GetHeight();*/
            Children = new Panel();
            Children.Position = new Point(0, 0);
            Children.Size = new Point(mWidth, mHeight);
            rs = mDrawEngine.CreateRenderSurface(mWidth, mHeight);
            /*rs = Scene.CreateAlphaRenderSurface(mWidth, mHeight);
            rs.SetBackgroundColor(new TV_COLOR(0, 0, 0, 0));
            mScreen2D = new TVScreen2DImmediate();
            mInput = new TVInputEngine();
            mTextures = new TVTextureFactory();
            mGlobals = new TVGlobals();
            mText = new TVScreen2DText();*/
            mMouseB1 = mMouseB2 = mMouseB3 = false;
            mMouseX = mMouseY = 0;
        }
        public static void AddWindow(Window w)
        {
            if (mModalWindow == w || ModalQueue.Contains(w))
                return;
            else if (Windows.Contains(w))
            {
                Windows.Remove(w);
            }
            if (mModalWindow == null)
            {
                Windows.Insert(0, w);
                if (FocusedElement != null)
                    FocusedElement.OnLostFocus(FocusedElement);
                FocusedElement = null;
                if (FocusedWindow != null)
                    FocusedWindow.OnLostFocus(FocusedWindow);
                FocusedWindow = w;
                FocusedWindow.OnGotFocus(FocusedWindow);
            }
            else
                Windows.Insert(1, w);
        }
        public static void AddModal(Window w)
        {
            if (mModalWindow == w || ModalQueue.Contains(w))
                return;
            else if (Windows.Contains(w))
            {
                Windows.Remove(w);
            }
            if (mModalWindow == null)
            {
                mModalWindow = w;
                if (FocusedElement != null)
                    FocusedElement.OnLostFocus(FocusedElement);
                FocusedElement = null;
                if (FocusedWindow != null)
                    FocusedWindow.OnLostFocus(FocusedWindow);
                FocusedWindow = mModalWindow;
                FocusedWindow.OnGotFocus(FocusedWindow);
            }
            else
                ModalQueue.Add(w);
            Windows.Insert(ModalQueue.Count, w);
        }
        public static IUITexture LoadTexture(byte[] data)
        {
            /*
            unsafe
            {
                fixed (byte* pSrc = data)
                {
                    string handle = mGlobals.GetDataSourceFromMemory((int)pSrc, data.Length);
                    return mTextures.LoadTexture(handle, Guid.NewGuid().ToString(), -1, -1, CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL);
                }
            }*/
            return mDrawEngine.CreateTexture(data);
        }
        public static void RemoveWindow(Window w)
        {
            if (mModalWindow == w)
            {
                mModalWindow = null;
                Windows.Remove(w);
                if (ModalQueue.Count > 0)
                {
                    mModalWindow = ModalQueue[0];
                    ModalQueue.Remove(mModalWindow);
                    if (FocusedElement != null)
                        FocusedElement.OnLostFocus(FocusedElement);
                    FocusedElement = null;
                    if (FocusedWindow != null)
                        FocusedWindow.OnLostFocus(FocusedWindow);
                    FocusedWindow = mModalWindow;
                    FocusedWindow.OnGotFocus(FocusedWindow);
                }
            }
            else if (ModalQueue.Contains(w))
            {
                ModalQueue.Remove(w);
                Windows.Remove(w);
            }
            else if (Windows.Contains(w))
            {
                Windows.Remove(w);
            }
        }
        private Element SingleHitTest()
        {
            int tmp = 0;
            return SingleHitTest(ref tmp);
        }
        private List<Element> MultiHitTest()
        {
            List<Element> elems = new List<Element>();
            foreach (Window w in Windows)
            {
                if (w == FocusedWindow)
                    FocusedWindow.HitTest(ref elems, mMouseX, mMouseY);
                else if (w.HitTestUnFocused)
                    w.HitTest(ref elems, mMouseX, mMouseY);
            }
            return elems;
        }
        private Element SingleHitTest(ref int Window)
        {
            if (mModalWindow != null)
            {
                Element e = mModalWindow.HitTest(mMouseX, mMouseY);
                Window = 0;
                return e;
            }
            else
            {
                Window = -1;
                Element e;
                if (Windows.Count > 0)
                {
                    for (int i = 0; i < Windows.Count; i++)
                    {
                        e = Windows[i].HitTest(mMouseX, mMouseY);
                        if (e != null)
                        {
                            Window = i;
                            return e;
                        }
                    }
                }
                return Children.HitTest(mMouseX, mMouseY);
            }
        }
        internal static Point mSize()
        {
            return new Point(mWidth, mHeight);
        }
        public Point Size()
        {
            return new Point(mWidth, mHeight);
        }
        public void Resize(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            Children.Size = new Point(mWidth, mHeight);
            rs.resize(mWidth, mHeight);
            /*
            mWidth = mTV.GetViewport().GetWidth();
            mHeight = mTV.GetViewport().GetHeight();
            Children.Size = new Point(mWidth, mHeight);
            rs.Destroy();
            rs = mScene.CreateAlphaRenderSurface(mWidth, mHeight);
            rs.SetBackgroundColor(new UIColor(0, 0, 0, 0));*/
            for (int i = 0; i < Windows.Count; i++)
                Windows[i].InvalidateVisual();
        }
        private void LeftMouseDown()
        {
            MouseCaptureElement = null;
            int win = -1;
            Element e = SingleHitTest(ref win);
            if (mModalWindow != null && e == null)//If there is a modal window it wasnt clicked
            {//Then make it flash
                mModalWindow.Flash();
                if (FocusedElement != null)
                    FocusedElement.OnLostFocus(FocusedElement);
                FocusedElement = null;
                if (FocusedWindow != null)
                    FocusedWindow.OnLostFocus(FocusedWindow);
                FocusedWindow = null;
            }
            if (win != -1)//Window was selected
            {
                if (win != 0)//Move selected window to top
                {
                    Window w = Windows[win];
                    Windows.Remove(w);
                    Windows.Insert(0, w);
                    w.InvalidateVisual();
                    win = 0;
                }
                if (Windows[win] != FocusedWindow)//If selected window wasn't before, give it focus and unfocus old window
                {
                    if (FocusedElement != null)
                        FocusedElement.OnLostFocus(FocusedElement);
                    FocusedElement = null;
                    if (FocusedWindow != null)
                        FocusedWindow.OnLostFocus(FocusedWindow);
                    FocusedWindow = Windows[win];
                    FocusedWindow.OnGotFocus(FocusedWindow);
                }
            }
            else//If mainpanel or nothing was selected, lose focus on old selected window
            {
                if (FocusedWindow != null)
                    FocusedWindow.OnLostFocus(FocusedWindow);
                FocusedWindow = null;
            }
            if (e != null)//If something was selected
            {
                Point p;
                if (win != -1 && Windows[win] != e)
                    p = Windows[win].ConvertMouse(mMouseX, mMouseY);
                else
                    p = new Point(mMouseX, mMouseY);
                if (e != FocusedElement)//Give new focused element focus and lose focus on old element
                {//Unless it was a window, because window focus was already handled
                    if (FocusedElement != null && !FocusedElement.GetType().IsSubclassOf(typeof(Window)))
                        FocusedElement.OnLostFocus(FocusedElement);
                    FocusedElement = e;
                    if (!e.GetType().IsSubclassOf(typeof(Window)))
                        e.OnGotFocus(e);
                }
                MouseCaptureElement = e;
                e.OnMouseDown(e, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 0, p.X, p.Y, 0));
            }
            else//If nothing was selected, lose focus on current focused element
            {
                if (FocusedElement != null && !FocusedElement.GetType().IsSubclassOf(typeof(Window)))
                    FocusedElement.OnLostFocus(FocusedElement);
                FocusedElement = null;
                FocusedWindow = null;
            }
        }
        private void LeftMouseUp()
        {
            if (MouseCaptureElement != null)
            {
                if (MouseCaptureElement == SingleHitTest())
                    MouseCaptureElement.OnMouseUp(MouseCaptureElement, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, mMouseX, mMouseY, 0));
                else
                    MouseCaptureElement.OnMouseUp(MouseCaptureElement, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 0, mMouseX, mMouseY, 0));
            }
        }
        private void RightMouseDown()
        {
        }
        private void RightMouseUp()
        {
        }
        private void MouseMove()
        {
            List<Element> elems = MultiHitTest();
            foreach (Element e in elems)
                if (!MouseElements.Contains(e))
                    e.OnMouseEnter(e, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
            foreach (Element e in MouseElements)
                if (!elems.Contains(e))
                    e.OnMouseLeave(e, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
            MouseElements.Clear();
            MouseElements.AddRange(elems);
            elems.Clear();
            if (MouseCaptureElement != null)
            {
                MouseCaptureElement.OnMouseMove(MouseCaptureElement, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, tmpMouseX, tmpMouseY, 0));
            }
        }
        public void GetInput(int tmpMouseXs, int tmpMouseYs, bool tmpMouseB1s, bool tmpMouseB2s, bool tmpMouseB3s)
        {
            tmpMouseX = tmpMouseXs;
            tmpMouseY = tmpMouseYs;
            tmpMouseB1 = tmpMouseB1s;
            tmpMouseB2 = tmpMouseB2s;
            tmpMouseB3 = tmpMouseB3s;
            if (mMouseB1 != tmpMouseB1)
            {
                mMouseB1 = tmpMouseB1;
                if (mMouseB1)
                    LeftMouseDown();
                else
                    LeftMouseUp();
            }
            if (mMouseB2 != tmpMouseB2)
            {
                mMouseB2 = tmpMouseB2;
                if (mMouseB2)
                    RightMouseDown();
                else
                    RightMouseUp();
            }
            if (mMouseX != tmpMouseX || mMouseY != tmpMouseY)
            {
                tmpMouseX -= mMouseX;
                tmpMouseY -= mMouseY;
                mMouseX += tmpMouseX;
                mMouseY += tmpMouseY;
                MouseMove();
            }
        }
        public void Update(float timeElapsed)
        {
            CoreUIEngine.TimeElapsed = timeElapsed;
            bool Redraw = false;
            for (int i = 0; i < Windows.Count; i++)
            {
                if (Windows[i].Invalidated)
                    Redraw = true;
                Windows[i].Render();
            }
            if (Children.Invalidated || Redraw || ForceRedraw)
            {
                ForceRedraw = false;
                mDrawEngine.BeginDraw(rs);
                //rs.StartRender();
               // mScreen2D.Action_Begin2D();
                Children.Render();
                for (int i = Windows.Count - 1; i > -1; i--)
                {
                    Windows[i].TexRender();
                }
               // mScreen2D.Action_End2D();
                //rs.EndRender();
                mDrawEngine.EndDraw();
            }

        }
        public void Render()
        {
            mDrawEngine.BeginDraw();
            mDrawEngine.Draw_RS(rs,  0, 0, mWidth, mHeight);
            //mDrawEngine.Draw_FilledBox(0, 0, 100, 100, mDrawEngine.CreateColor(1, 0, 0), mDrawEngine.CreateColor(1, 0, 0), mDrawEngine.CreateColor(1, 1, 0), mDrawEngine.CreateColor(1, 1, 0));
            //mDrawEngine.Draw_FilledBox(0, 0, 100, 100, Color.Red.ToArgb(), Color.Green.ToArgb(), Color.Yellow.ToArgb(), Color.White.ToArgb());
            mDrawEngine.EndDraw();
            //mScreen2D.Action_Begin2D();
            //mScreen2D.Draw_Texture(rs.GetTexture(), 0, 0, mWidth - 1, mHeight - 1);
            //mScreen2D.Draw_FilledBox(0, 0, 100, 100, Color.Red.ToArgb(), Color.Green.ToArgb(), Color.Yellow.ToArgb(), Color.White.ToArgb());
            //mScreen2D.Action_End2D();
        }

    }
}
