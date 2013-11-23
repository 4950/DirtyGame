using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using OpenTK.Input;

namespace DirtyGame.game.Input
{
    public class KeyHandler
    {
        public delegate void KeyHandlerDelegate(Keys key);

        public KeyHandler(Keys key, KeyHandlerDelegate onPressedDelegate, KeyHandlerDelegate onReleadedDelegate)
        {
            OnPressedHandler = onPressedDelegate;
            OnReleasedHandler = onReleadedDelegate;
            Key = key;
        }

        public Keys Key
        {
            get;
            private set;
        }

        public KeyHandlerDelegate OnPressedHandler
        {
            get;
            private set;
        }

        public KeyHandlerDelegate OnReleasedHandler
        {
            get;
            private set;
        }
    }
}
