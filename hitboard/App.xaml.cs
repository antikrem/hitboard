using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using System.Diagnostics;

using hitboard.pipeline;

namespace hitboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Central hooking interface
        KeyboardHook hook = new KeyboardHook();

        public App()
        {
            hook.StartHook();
            Trace.WriteLine("teasdasdxt");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            hook.StopHook();
        }
    }
}
