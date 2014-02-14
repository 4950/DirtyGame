using CleanGame.Game.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanGame.Game.Core
{
    public class EventManager
    {
        // delegate signature for event listeners
        public delegate void EventCallback(Event e);

        // Keeps track of which functions are listening for which events.
        private Dictionary<string, List<EventCallback>> listenerDict;

        // EventManager is a Singleton
        private static EventManager instance;

        private  EventManager()
        {
            listenerDict = new Dictionary<string, List<EventCallback>>();
        }

        /// <summary>
        /// Return the singleton Event Manager. Creates a new EventManager if one hasn't been created yet.
        /// </summary>
        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Register a function to be called when the specified event occurs.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        public void AddListener(string eventName, EventCallback listener)
        {
            if (!listenerDict.ContainsKey(eventName))
            {
                listenerDict.Add(eventName, new List<EventCallback>());
            }
            listenerDict[eventName].Add(listener);
        }

        /// <summary>
        /// Remove a function from the list of listeners for the specified event.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        public void RemoveListener(string eventName, EventCallback listener)
        {
            if (!listenerDict[eventName].Contains(listener))
            {
                return;
            }
            listenerDict[eventName].Remove(listener);
        }

        /// <summary>
        /// Triggers the event, calling all functions that are listening for that event.
        /// </summary>
        /// <param name="e"></param>
        public void TriggerEvent(Event e)
        {
            if (listenerDict.ContainsKey(e.name))
            {
                foreach (EventCallback listener in listenerDict[e.name])
                {
                    listener(e);
                }
            }
        }


        ///////////////////////////////////
        /// For doing later
        //////////////////////////////////.
        // queue event - add an event to the queue to be fired later when update is called

        // abort event - remove an event from the queue so it doesn't get fired

        // update - processes events in the queue. would be called every game loop
    }
}
