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
using System.Windows.Shapes;

using hitboard.pipeline;

namespace hitboard
{
    /// <summary>
    /// Interaction logic for SaveAsDialogue.xaml
    /// </summary>
    public partial class SaveAsDialogue : Window
    {
        public string Location = "";

        public SaveAsDialogue(KeyConfiguration config)
        {
            InitializeComponent();

            if (config is object)
            {
                SaveLocationTextBox.Text = config.Name.Substring(KeyConfiguration.CONFIG_FOLDER.Length);
            }
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Location = SaveLocationTextBox.Text;
            this.Close();
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
