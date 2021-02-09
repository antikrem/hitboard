using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hitboard.pipeline
{


    /**
     * Represents an event within the pipeline
     * Evoked from KeyboardHook and others
     */
    class Event
    {
        public enum EventType
        {
            NONE,
            STOP,
            PRESS,
            RELEASE,
            
        }

        public readonly EventType Type;
        public readonly int ScanCode;


        // Constructors for Keyboard events
        public Event(EventType type, int scancode)
        {
            Type = type;
            ScanCode = scancode;
        }

        // Constructors for General event
        public Event(EventType type)
        {
            Type = type;
        }
    }
}
