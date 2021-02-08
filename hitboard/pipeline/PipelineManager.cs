using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        // Event loop action
        // Will stop on EventType.STOP event
        private void EventLoop()
        {
            Event e;
            while ((e = EventQueue.Take()).Type != Event.EventType.STOP)
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
            EventLoop();
        }

        // Clear pipeline
        public void Free()
        {
            hook.StopHook();
        }
    }
}
