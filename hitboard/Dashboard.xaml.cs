using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;
using hitboard.pipeline;

namespace hitboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        bool IsPipelineActive = false;

        bool configHandleFlag = true;

        // Key button cache
        private Dictionary<pipeline.Key, InputBox> InputBoxCache;

        public Dashboard()
        {
            InitializeComponent();

            InputBoxCache = new Dictionary<pipeline.Key, InputBox>
            {
                { pipeline.Key.UP, BtnUp },
                { pipeline.Key.DOWN, BtnDown },
                { pipeline.Key.LEFT, BtnLeft },
                { pipeline.Key.RIGHT, BtnRight },

                { pipeline.Key.X, BtnX },
                { pipeline.Key.Y, BtnY },
                { pipeline.Key.A, BtnA },
                { pipeline.Key.B, BtnB },

                { pipeline.Key.LEFT_SHOULDER, BtnLeftShoulder },
                { pipeline.Key.LEFT_TRIGGER, BtnLeftTrigger },
                { pipeline.Key.RIGHT_SHOULDER, BtnRightShoulder },
                { pipeline.Key.RIGHT_TRIGGER, BtnRightTrigger },

                { pipeline.Key.START, BtnStart },
                { pipeline.Key.BACK, BtnBack },
                { pipeline.Key.LEFT_JOYSTICK_DOWN, BtnLeftJoy },
                { pipeline.Key.RIGHT_JOYSTICK_DOWN, BtnRightJoy }
            };

            PopulateConfigComboBox();
        }

        private void PopulateConfigComboBox()
        {
            var configurations = KeyConfiguration.LoadConfigurations();

            foreach (KeyConfiguration config in configurations)
            {
                ConfigComboBox.Items.Add(config);
            }
        }

        private void ConfigComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            configHandleFlag = !cmb.IsDropDownOpen;
            LoadConfiguration((KeyConfiguration)ConfigComboBox.SelectedItem);
        }

        private void ConfigComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (configHandleFlag)
            {
                LoadConfiguration((KeyConfiguration)ConfigComboBox.SelectedItem);
            }
            configHandleFlag = true;
        }

        private void StartButton_Press(object sender, RoutedEventArgs e)
        {
            if (IsPipelineActive)
            {
                StopPipeline();
            }
            else
            {
                StartPipeline();
            }

            IsPipelineActive = !IsPipelineActive;
        }

        private void LoadButton_Press(object sender, RoutedEventArgs e)
        {
            var config = GenerateConfiguration();
           // LoadConfiguration(KeyConfiguration.Load());
        }

        private void SaveButton_Press(object sender, RoutedEventArgs e)
        {
            var config = GenerateConfiguration();
            config.Save("ChallyPro");
        }

        // Load a configuration into dashboard
        private void LoadConfiguration(KeyConfiguration configuration)
        {
            foreach (var i in configuration.Configuration)
            {
                InputBoxCache[i.Value].KeyCode = i.Key;
            }
        }

        // Convert into a configuration
        private KeyConfiguration GenerateConfiguration()
        {
            KeyConfiguration configuration = new KeyConfiguration();
            foreach (var i in InputBoxCache)
            {
                int keycode = i.Value.KeyCode;
                if (keycode > 0)
                {
                    configuration.Configuration[keycode] = i.Key;
                }
                
            }

            return configuration;
        }

        // Start pipeline
        public void StartPipeline()
        {
            StartButton.Content = "Stop";

            // Get configuration from setup
            var configuration = GenerateConfiguration();

            App.Instance.StartPipeline(configuration);
        }

        // Stop pipeline
        public void StopPipeline()
        {
            StartButton.Content = "Start";

            App.Instance.StopPipeline();
        }
    }
}
