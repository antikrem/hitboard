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

namespace hitboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        bool IsPipelineActive = false;

        public Dashboard()
        {
            InitializeComponent();

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

        // Start pipeline
        public void StartPipeline()
        {
            StartButton.Content = "Stop";

            App.Instance.StartPipeline();

        }

        // Stop pipeline
        public void StopPipeline()
        {
            StartButton.Content = "Start";

            App.Instance.StopPipeline();
        }
    }
}
