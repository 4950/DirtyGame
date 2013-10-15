using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;

namespace ShittyPrototype.src.application
{
    class GlobalLua
    {
        public static Lua lua = new Lua();

        /// <summary>
        /// Convenience method. Easier to type than using Lua object's method,
        /// and don't need to include "scripts\\" every time.
        /// </summary>
        /// <param name="fileName">Name of script file to be run.</param>
        public static void DoFile(string fileName)
        {
            lua.DoFile("scripts\\" + fileName + ".lua");
        }
        
    }
}
