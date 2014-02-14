using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;

namespace CleanGame.Game.Core.Systems.Util
{
    public class SystemParams
    {
        public SystemParams(Aspect aspect, int priority)
        {
            Aspect = aspect;
            Priority = priority;
        }

        public Aspect Aspect
        {
            get;
            set;
        }

        public int Priority
        {
            get;
            set;
        }
    }
}
