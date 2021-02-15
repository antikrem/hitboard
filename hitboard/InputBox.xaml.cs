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

using hitboard.pipeline;
using hitboard.callback;

namespace hitboard
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : UserControl
    {
        private string _KeyName = "<unassigned>";

        private int _KeyCode = -1;

        public int KeyCode
        {
            get
            {
                return _KeyCode;
            }
            set
            {
                _KeyCode = value;
                DrawKeycodeLabel();
            }
        }

        // Transfers _KeyName to label
        private void DrawKeyLabel()
        {
            Key.Content = _KeyName;
        }

        // Transfers _KeyCode to label
        private void DrawKeycodeLabel()
        {
            if (KeyCode < 0) return;
            Button.Content = AsciiConverter.KeycodeToAscii(KeyCode);
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
            int value = prompt.Resolve();
            KeyCode = value;;
        }
    }
}
