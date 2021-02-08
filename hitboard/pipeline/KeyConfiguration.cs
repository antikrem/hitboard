using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hitboard.pipeline
{
    /**
     * Represents an input setup
     * Which translates a scancode to Key
     */
    class KeyConfiguration
    {
        public SortedDictionary<int, Key> Configuration = new SortedDictionary<int, Key>()
            {
                { 87, Key.UP },
                { 83, Key.DOWN },
                { 65, Key.LEFT },
                { 68, Key.RIGHT },
                { 73, Key.X },
                { 79, Key.Y },
                { 75, Key.A },
                { 76, Key.B },
                { 74, Key.START },
                { 70, Key.BACK }
            };
    }
}
