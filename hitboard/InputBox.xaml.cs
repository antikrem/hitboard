using System;
using System.Collections.Generic;
using System.Diagnostics;
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

using hitboard.callback;

namespace hitboard
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : UserControl
    {
        private string _KeyName = "<unassigned>";

        // Transfers _KeyName to label
        private void DrawKeyLabel()
        {
            Key.Content = _KeyName;
        }

        public string KeyName
        {
            get
            {
                return _KeyName;
            }
            set
            {
                _KeyName = value;
                DrawKeyLabel();
            }
        }

        public InputBox()
        {
            InitializeComponent();
            DrawKeyLabel();
        }

        void Key_Click(object sender, RoutedEventArgs e)
        {
            KeyboardPrompt prompt = new KeyboardPrompt();
            Button.Content = AsciiConverter.KeycodeToAscii(prompt.Resolve());
        }
    }
}
