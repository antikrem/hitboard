using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        // Different rules to resolve SOCD's
        enum SOCDResolution
        {
            Both,
            Neutral,
            Low,
            High
        }

        // Solution to an SOCD given input
        static private (bool, bool) ResolveSOCD(bool lowInput, bool highInput, SOCDResolution resolution)
        {
            switch (resolution)
            {
                case SOCDResolution.Both:
                    return (lowInput, highInput);

                case SOCDResolution.Neutral:
                    return lowInput && highInput ? (false, false) : (lowInput, highInput);

                case SOCDResolution.Low:
                    return lowInput && highInput ? (true, false) : (lowInput, highInput);

                case SOCDResolution.High:
                    return lowInput && highInput ? (false, true) : (lowInput, highInput);

                default:
                    return (lowInput, highInput);
            }

        }

        // SOCD Resolution for given input
        SOCDResolution UpDownResolution = SOCDResolution.Both;
        SOCDResolution LeftRightResolution = SOCDResolution.Neutral;

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
                { 72, Key.START },
                { 70, Key.BACK },
                { 85, Key.LEFT_SHOULDER },
                { 74, Key.LEFT_TRIGGER },
                { 80, Key.RIGHT_SHOULDER },
                { 186, Key.RIGHT_TRIGGER }
            };

        // Given a keystate and event, update keystate
        // Returns a new state that is presented to controller
        public KeyState UpdateKeyState(KeyState state, Event e)
        {
            // Check this was the correct input
            if (!Configuration.ContainsKey(e.ScanCode)) return state;

            // Update keyboard state
            state.Buttons[(int)Configuration[e.ScanCode]] = (e.Type == Event.EventType.PRESS);

            KeyState eState = (KeyState)state.Clone();

            // Resolve SOCD
            (eState.Buttons[(int)Key.UP], eState.Buttons[(int)Key.DOWN]) 
                    = ResolveSOCD(eState.Buttons[(int)Key.UP], eState.Buttons[(int)Key.DOWN], UpDownResolution);
            (eState.Buttons[(int)Key.LEFT], eState.Buttons[(int)Key.RIGHT])
                    = ResolveSOCD(eState.Buttons[(int)Key.LEFT], eState.Buttons[(int)Key.RIGHT], LeftRightResolution);

            return eState;
        }
    }
}
