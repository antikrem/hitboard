using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hitboard.pipeline
{
    class PipelineManager
    {
        private KeyboardHook hook;
        private VController vController;

        // Keystate
        private KeyState State = new KeyState();

        // Currently active configuration
        private KeyConfiguration Configuration;

        // Event Queue
        private readonly BlockingCollection<Event> EventQueue  
                = new BlockingCollection<Event>(new ConcurrentQueue<Event>());

        // Handle thread
        private Thread PipelineHandler = null;

        // Initialise all required components for pipeline
        public PipelineManager()
        {
            // Create virtual controller
            VController.Initialise();
            vController = new VController();

            // Create hook
            hook = new KeyboardHook(EventQueue);
        }

        // Checks if Event is a stop sequence
        private static bool CheckLoopCondition(Event e)
        {
            return
                (e.Type != Event.EventType.STOP) &&
                !(e.Type == Event.EventType.PRESS && e.ScanCode == 27);
        }


        // Post event loop clean up
        private void PostCycleCleanup()
        {
            // Disable hook 
            hook.StopHook();
        }

        // Event loop action
        // Will stop on EventType.STOP event
        private void EventLoop()
        {
            Event e;

            // Keep looping until Loop Conditions are not met
            while (CheckLoopCondition(e = EventQueue.Take()))
            {
                switch (e.Type)
                {
                    // Ignore
                    case Event.EventType.NONE:
                        break;

                    // Given new event, update keystate
                    case Event.EventType.PRESS:
                    case Event.EventType.RELEASE:
                        KeyState eState = Configuration.UpdateKeyState(State, e);
                        vController.Input(eState);
                        break;

                    // Handled in CheckLoopCondition
                    case Event.EventType.STOP:
                        break;
                }
            }

            // Event loop over
            PostCycleCleanup();
        }

        // Enter event loop for pipeline
        public void Start(KeyConfiguration configuration)
        {
            this.Configuration = configuration;
            hook.StartHook(configuration);
            PipelineHandler = new Thread(this.EventLoop);
            PipelineHandler.Start();
        }

        // Clear pipeline 
        public void Stop()
        {
            if (PipelineHandler is null)
            {
                return;
            }

            EventQueue.Add(new Event(Event.EventType.STOP));
            PipelineHandler.Join();
            PipelineHandler = null;
        }
    }
}
