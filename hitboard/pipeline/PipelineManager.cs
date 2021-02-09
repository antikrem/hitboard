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
        private readonly KeyboardHook hook;
        private VController vController;

        // Keystate
        private KeyState State = new KeyState();

        // Currently active configuration
        private KeyConfiguration configuration = new KeyConfiguration();

        // Event Queue
        private readonly BlockingCollection<Event> EventQueue  
                = new BlockingCollection<Event>(new ConcurrentQueue<Event>());

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
                        KeyState eState = configuration.UpdateKeyState(State, e);
                        vController.Input(eState);
                        break;

                    // Handled in CheckLoopCondition
                    case Event.EventType.STOP:
                        break;
                }
            }
        }

        // Enter event loop for pipeline
        public void Start()
        {
            hook.StartHook();
            (new Thread(this.EventLoop)).Start();
        }

        // Clear pipeline 
        public void Stop()
        {
            hook.StopHook();
            EventQueue.Add(new Event(Event.EventType.STOP));
        }
    }
}
