﻿using System;
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
        // Central pipeline
        // This manages taking player input
        // And converting it to a virtual controller
        // output
        PipelineManager pipeline; 

        public App()
        {

            pipeline = new PipelineManager();
            pipeline.SpinUpEventLoop();


        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            pipeline.Free();
        }
    }
}
