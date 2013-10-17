using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ShittyPrototype
{
    class InputManager : Singleton<InputManager>
    {
        private KeyboardState _currentState;
        private LinkedList<IInputContext> contextStack = new LinkedList<IInputContext>();
        public MouseState mouseState;

//         public void PushContext(IInputContext context)
//         {
//             contextStack.AddLast(context);
//         }
// 
//         public bool RemoveContext(IInputContext context)
//         {
//             return contextStack.Remove(context);  
//         }

        public bool IsKeyDown(Keys key)
        {
            return _currentState.IsKeyDown(key);
        }

        public void Update()
        {
            _currentState = Keyboard.GetState();
            mouseState = Mouse.GetState();
        }
    }
}
