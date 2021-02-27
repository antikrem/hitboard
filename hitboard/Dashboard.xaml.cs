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

            // Build cache for buttons
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

            PopulateComboBoxes();

            // Cache dashboard
            App.Instance.Dashboard = this;
        }

        // Locks all controls not StartButton
        private void Lock()
        { 
            foreach (var control in RootGrid.Children.OfType<GroupBox>())
            {
                control.IsEnabled = false;
            }
        }

        // Unlocks all controls
        private void Unlock()
        {
            foreach (var control in RootGrid.Children.OfType<GroupBox>())
            {
                control.IsEnabled = true;
            }
        }

        // Set up combo boxes
        private void PopulateComboBoxes()
        {
            var configurations = KeyConfiguration.LoadConfigurations();

            foreach (KeyConfiguration config in configurations)
            {
                ConfigComboBox.Items.Add(config);
            }

            foreach (KeyConfiguration.SOCDResolution resolution in Enum.GetValues(typeof(KeyConfiguration.SOCDResolution)))
            {
                if (resolution != KeyConfiguration.SOCDResolution.Up && resolution != KeyConfiguration.SOCDResolution.Down)
                {
                    LeftRightSOCDComboBox.Items.Add(resolution);
                }
                if (resolution != KeyConfiguration.SOCDResolution.Left && resolution != KeyConfiguration.SOCDResolution.Right)
                {
                    UpDownSOCDComboBox.Items.Add(resolution);
                }
            }

            foreach (KeyConfiguration.KeyActivation activation in Enum.GetValues(typeof(KeyConfiguration.KeyActivation)))
            {
                DirectionalComboBox.Items.Add(activation);
                FaceComboBox.Items.Add(activation);
            }

            // Set default SOCD
            LeftRightSOCDComboBox.SelectedItem = KeyConfiguration.SOCDResolution.Both;
            UpDownSOCDComboBox.SelectedItem = KeyConfiguration.SOCDResolution.Both;

            // Set default Activation
            DirectionalComboBox.SelectedItem = KeyConfiguration.KeyActivation.Default;
            FaceComboBox.SelectedItem = KeyConfiguration.KeyActivation.Default;
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
        }

        private void LoadButton_Press(object sender, RoutedEventArgs e)
        {
            LoadConfiguration((KeyConfiguration)ConfigComboBox.SelectedItem);
        }

        private void SaveButton_Press(object sender, RoutedEventArgs e)
        {
            var config = GenerateConfiguration();

            // Start dialogue
            SaveAsDialogue dialogue = new SaveAsDialogue((KeyConfiguration)ConfigComboBox.SelectedItem);
            dialogue.ShowDialog();

            // If dialogue was successful, save
            if (dialogue.Location.Length > 0)
            {
                try
                {
                    config.Save(dialogue.Location);
                }
                catch (Exception)
                {
                    MessageBox.Show(
                            "Error saving file, possibly due to lack of permissions or storage. Save operation aborted.", 
                            "Error",
                            MessageBoxButton.OK, 
                            MessageBoxImage.Error
                        );
                }
            }

        }

        // Load a configuration into dashboard
        private void LoadConfiguration(KeyConfiguration configuration)
        {
            if (configuration is null)
            {
                return;
            }

            foreach (var i in InputBoxCache)
            {
                i.Value.KeyCode = -1;
            }

            foreach (var i in configuration.Configuration)
            {
                InputBoxCache[i.Value].KeyCode = i.Key;
            }

            LeftRightSOCDComboBox.SelectedItem = configuration.LeftRightResolution;
            UpDownSOCDComboBox.SelectedItem = configuration.UpDownResolution;

            DirectionalComboBox.SelectedItem = configuration.DirectionActivation;
            FaceComboBox.SelectedItem = configuration.FaceActivation;
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

            configuration.LeftRightResolution = (KeyConfiguration.SOCDResolution)LeftRightSOCDComboBox.SelectedItem;
            configuration.UpDownResolution = (KeyConfiguration.SOCDResolution)UpDownSOCDComboBox.SelectedItem;

            configuration.DirectionActivation = (KeyConfiguration.KeyActivation)DirectionalComboBox.SelectedItem;
            configuration.FaceActivation = (KeyConfiguration.KeyActivation)FaceComboBox.SelectedItem;

            return configuration;
        }

        // Start pipeline
        public void StartPipeline()
        {
            StartButton.Content = "Stop (or press ESC)";
            IsPipelineActive = !IsPipelineActive;
            Lock();

            // Get configuration from setup
            var configuration = GenerateConfiguration();

            App.Instance.StartPipeline(configuration);
        }

        // Stop pipeline
        public void StopPipeline()
        {
            App.Instance.StopPipeline();
        }

        // Change front end to be in start set
        // This may be called from the pipeline manager
        public void ResetFrontEnd()
        {
            StartButton.Content = "Start";
            IsPipelineActive = !IsPipelineActive;
            Unlock();
        }
    }
}
