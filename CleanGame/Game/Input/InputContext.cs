using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace CleanGame.Game.Input
{
    public class InputContext
    {      
        private Dictionary<Keys, KeyHandler> keyHandlers = new Dictionary<Keys, KeyHandler>();

        public void RegisterHandler(Keys key, KeyHandler.KeyHandlerDelegate onPressedDelegate, KeyHandler.KeyHandlerDelegate onReleadedDelegate)
        {
            keyHandlers.Add(key, new KeyHandler(key, onPressedDelegate, onReleadedDelegate)); 
        }
        public void UnregisterHandlers(Keys key)
        {
            keyHandlers.Remove(key);
        }
        public void RemoveAllHandlers()
        {
            keyHandlers.Clear();
        }
        public bool Dispatch(Keys key, KeyState state)
        {
            KeyHandler.KeyHandlerDelegate handler;

            if (!keyHandlers.ContainsKey(key))
            {
                return false;
            }

            if (state == KeyState.Down)
            {
                 handler = keyHandlers[key].OnPressedHandler;
            }
            else if (state == KeyState.Up)
            {
                handler = keyHandlers[key].OnReleasedHandler;
            }
            else
            {
                // problem -- should probably log this
                return false;
            }

            // check if the handler had a delegate
            if (handler == null)
            {
                return false;
            }
            else
            {
                handler(key);
                return true;
            }
            
        }
    }
}
