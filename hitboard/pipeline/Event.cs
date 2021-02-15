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
    public class Event
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

        // 
        private Key[] SOCD_INPUTS = new Key[]
        {
            Key.UP,
            Key.DOWN,
            Key.LEFT,
            Key.RIGHT
        };

        // Returns true if it is an SOCD effecting input
        public bool IsSOCDEffecting(KeyConfiguration config)
        {
            if (!(this.Type == EventType.PRESS || this.Type == EventType.RELEASE)) return false;

            // Check if it was either a left, right, up or down
            return (
                    SOCD_INPUTS.Contains(config.Configuration[this.ScanCode])
                );

        }
    }
}
