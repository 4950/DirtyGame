using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanGame.Game.Core.Events
{
    /// <summary>
    /// Stores all the data of an event.
    /// Currently only has name, could be extended to have more data carried with it (such as a timestamp).
    /// </summary>
    public class Event
    {
        public string name { get; set; }
    }
}
