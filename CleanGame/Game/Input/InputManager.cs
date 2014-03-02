using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CleanGame.Game.Util;
using Microsoft.Xna.Framework.Input;

namespace CleanGame.Game.Input
{
    public class InputManager : Singleton<InputManager>
    {
        private KeyboardState oldState;       
        private List<InputContext> inputContexts = new List<InputContext>();
     
        public void AddInputContext(InputContext context)
        {
            inputContexts.Add(context);
        }

        public void RemoveInputContext(InputContext context)
        {
            inputContexts.Remove(context);
        }
        public void RemoveAllContexts()
        {
            inputContexts.Clear();
        }

        private void DispatchKey(Keys key, KeyState state)
        {
            foreach (InputContext context in inputContexts)
            {
                if (context.Dispatch(key, state))
                {                    
                    //return;
                }
            }
        }
              
        public void DispatchInput()
        {
            KeyboardState newState = Keyboard.GetState();

            // find onPress events
            foreach (Keys key in newState.GetPressedKeys())
            {
                if (oldState.IsKeyUp(key))
                {
                    DispatchKey(key, KeyState.Down);
                }
            }

            //find onRelease events
            foreach (Keys key in oldState.GetPressedKeys())
            {
                if (newState.IsKeyUp(key))
                {   
                    DispatchKey(key, KeyState.Up);
                }    
            }
            
            oldState = newState;
        }
    }
}
