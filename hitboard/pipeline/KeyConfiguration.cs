using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace hitboard.pipeline
{
    /**
     * Represents an input setup
     * Which translates a scancode to Key
     */
     [Serializable]
    public class KeyConfiguration
    {
        private const string CONFIG_FOLDER = "configs/";
        private const string CONFIG_SUFFIX = ".json";

        // Different rules to resolve SOCD's
        public enum SOCDResolution
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

        // Associated to string name
        public string Name;

        // SOCD Resolution for given input
        public SOCDResolution UpDownResolution { get; set; } = SOCDResolution.Both;
        public SOCDResolution LeftRightResolution { get; set; } = SOCDResolution.Neutral;

        public SortedDictionary<int, Key> Configuration { get; set; } = new SortedDictionary<int, Key>();

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

        // Save this configuration as a json
        public void Save(string name)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(this, options);

            File.WriteAllText(CONFIG_FOLDER + name + CONFIG_SUFFIX, json);

        }

        // Load a configuration given a name
        static public KeyConfiguration Load(string configFile)
        {
            string json = File.ReadAllText(configFile);

            var configuration = JsonSerializer.Deserialize<KeyConfiguration>(json);
            configuration.Name = configFile;
            return configuration;
        }

        // Loads all possible key configs
        static public KeyConfiguration[] LoadConfigurations()
        {
            string[] filePaths = Directory.GetFiles(CONFIG_FOLDER, "*" + CONFIG_SUFFIX,
                                         SearchOption.TopDirectoryOnly);

            KeyConfiguration[] configs = filePaths.ToList()
                                                .Select(x => KeyConfiguration.Load(x))
                                                .ToArray();
            return configs;
        }

        public override string ToString() {
            return Name;
        }
    }
}
