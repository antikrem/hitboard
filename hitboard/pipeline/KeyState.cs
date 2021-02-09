using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hitboard.pipeline
{
    // Enumeration of different inputs
    public enum Key
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        START,
        BACK,
        LEFT_THUMB,
        RIGHT_THUMB,
        LEFT_SHOULDER,
        RIGHT_SHOULDER,
        LEFT_TRIGGER,
        RIGHT_TRIGGER,
        X,
        Y,
        A,
        B,
        KEYCOUNT
        
    }



    // Represents the current configuration of keys
    class KeyState : ICloneable
    {

        // Look up for Key to XINPUT bitmask
        public static SortedDictionary<Key, ushort> KEY_TO_MASK =  new  SortedDictionary<Key, ushort>()  {
                { Key.UP, 0x0001 },
                { Key.DOWN, 0x0002 },
                { Key.LEFT, 0x0004 },
                { Key.RIGHT, 0x0008 },
                { Key.START, 0x0010 },
                { Key.BACK, 0x0020 },
                { Key.LEFT_THUMB, 0x0040 },
                { Key.RIGHT_THUMB, 0x0080 },
                { Key.LEFT_SHOULDER, 0x0100 },
                { Key.RIGHT_SHOULDER, 0x0200 },
                { Key.X, 0x4000 },
                { Key.Y, 0x8000 },
                { Key.A, 0x1000 },
                { Key.B, 0x2000 },
            };


        // Current configuration of this KeyState
        public bool[] Buttons = new bool[(int)Key.KEYCOUNT];

        // Create a new XInputState given this KeyState
        public XInputState ToXinputState()
        {
            XInputState state = new XInputState();
            foreach (KeyValuePair<Key, ushort> entry in KEY_TO_MASK)
            {
                state.Gamepad.Buttons |= (ushort)((Buttons[(int)entry.Key] ? 1 : 0) * KEY_TO_MASK[entry.Key]); 
            }

            return state;
        }

        // Clone this object
        public object Clone()
        {
            KeyState state = new KeyState();
            for (int i = 0; i < (int)Key.KEYCOUNT; i++)
            {
                state.Buttons[i] = this.Buttons[i];
            }

            return state;

        }
    }


}
