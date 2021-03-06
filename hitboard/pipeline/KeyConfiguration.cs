﻿using System;
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
     * 
     * In terms of saving, save files are in config
     * and saved in c:/ProgramData/hitboard/config
     */
     [Serializable]
    public class KeyConfiguration
    {
        public static string CONFIG_FOLDER
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/hitboard/configs/";
            }
        }
        private const string CONFIG_SUFFIX = ".json";

        // Different rules to resolve SOCD's
        public enum SOCDResolution
        {
            Both,
            Neutral,
            Up,
            Down,
            Left,
            Right
        }

        // Different rules for updating key state
        public enum KeyActivation
        {
            Default,
            RisingEdge,
            FallingEdge
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

                case SOCDResolution.Up:
                case SOCDResolution.Left:
                    return lowInput && highInput ? (true, false) : (lowInput, highInput);

                case SOCDResolution.Down:
                case SOCDResolution.Right:
                    return lowInput && highInput ? (false, true) : (lowInput, highInput);

                default:
                    return (lowInput, highInput);
            }

        }

        // Updates state given input and key configuration
        private void UpdateFromInput(KeyState state, Event e)
        {
            switch (e.IsSOCDEffecting(this) ? FaceActivation : DirectionActivation)
            {

                case KeyActivation.Default:
                default:
                    state.Buttons[(int)Configuration[e.ScanCode]] = (e.Type == Event.EventType.PRESS);
                    break;

            }
        }

        // Associated to string name
        public string Name;

        // SOCD Resolution for given input
        public SOCDResolution UpDownResolution { get; set; } = SOCDResolution.Both;
        public SOCDResolution LeftRightResolution { get; set; } = SOCDResolution.Neutral;

        // Activation for buttons
        public KeyActivation DirectionActivation { get; set; } = KeyActivation.Default;
        public KeyActivation FaceActivation { get; set; } = KeyActivation.Default;

        public SortedDictionary<int, Key> Configuration { get; set; } = new SortedDictionary<int, Key>();

        // Given a keystate and event, update keystate
        // Returns a new state that is presented to controller
        public KeyState UpdateKeyState(KeyState state, Event e)
        {
            // Check this was the correct input
            if (!Configuration.ContainsKey(e.ScanCode)) return state;

            // Update keyboard state
            UpdateFromInput(state, e);

            // Copy state for SOCD
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

            File.WriteAllText(CONFIG_FOLDER + name, json);

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
            // Create a folder if it doesn't exist
            Directory.CreateDirectory(CONFIG_FOLDER);

            // Load all files contained
            string[] filePaths = Directory.GetFiles(CONFIG_FOLDER, "*" + CONFIG_SUFFIX,
                                         SearchOption.TopDirectoryOnly);

            // Load individual configs
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
