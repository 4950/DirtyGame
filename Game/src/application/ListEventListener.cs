using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ShittyPrototype.src.application.core;
using ShittyPrototype.src;
using ShittyPrototype.src.graphics;
using ShittyPrototype.src.util;

namespace ShittyPrototype.src.application
{
    class ListEventListener
    {
        private ListEvent<Monster> List;
        private bool gamestarted = false;
        int numOfMonsters = 0;


        public ListEventListener(ListEvent<Monster> list)
        {
            List = list;
            List.Changed += new ChangedEventHandler(ListChanged);
        }

        private void ListChanged(object sender, EventArgs e, string s)
        {
            if (String.Equals(s, "add"))
            {
                if (!gamestarted)
                    gamestarted = true;
                numOfMonsters++;
            }

            if (String.Equals(s, "remove"))
            {
                numOfMonsters--;
            }
            if (numOfMonsters == 0 && gamestarted)
            {

                Singleton<Gamestate>.GetSingleton().GameIsOver();
            }
        }

        public void Detach()
        {
            List.Changed -= new ChangedEventHandler(ListChanged);
            List = null;
        }
    }
}
