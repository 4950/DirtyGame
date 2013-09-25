using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ShittyPrototype
{
    class InputManager : Singleton<InputManager>
    {
        private LinkedList<IInputContext> contextStack = new LinkedList<IInputContext>();

        public void PushContext(IInputContext context)
        {
            contextStack.AddLast(context);
        }
        public bool RemoveContext(IInputContext context)
        {
            return contextStack.Remove(context);  
        }

        public void Update()
        {
            KeyboardState state = Keyboard.GetState();
            Keys[] pressedKeys = state.GetPressedKeys();
            foreach(IInputContext context in contextStack) {
                foreach(Keys key in pressedKeys) {
                    //context.dispatch(key);
                }

                
            } 
        }
    }
}
