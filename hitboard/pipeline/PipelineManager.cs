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
            hook.StartHook();

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
                // Check for a stop hardcode
                if (e.Type == Event.EventType.PRESS && e.ScanCode == 27) break;

                // Given new event, update keystate
                if (!configuration.Configuration.ContainsKey(e.ScanCode)) continue;
                State.Buttons[(int)configuration.Configuration[e.ScanCode]] = (e.Type == Event.EventType.PRESS);

                int value = vController.Input(State);
            }
        }

        // Enter event loop for pipeline
        public void SpinUpEventLoop()
        {
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
