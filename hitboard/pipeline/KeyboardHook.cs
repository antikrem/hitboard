using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

namespace hitboard.pipeline
{
    /**
     * An instance of this class will intercept all input from the
     * keyboard
     */
    class KeyboardHook
    {

        // Function prototype for callback
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Reference to callback to ensure it will not be GCed
        LowLevelKeyboardProc CallbackRef = HookCallback;

        // Static reference to pipeline in use
        static private BlockingCollection<Event> EventQueue;

        // Cache for inputs being listened to
        static private SortedSet<int> ValidInputs;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        private static IntPtr hookID = IntPtr.Zero;

        // Declare syscalls to externally link
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public KeyboardHook(BlockingCollection<Event> eventQueue)
        {
            EventQueue = eventQueue;
        }

        public void StartHook(KeyConfiguration configuration)
        {
            hookID = SetHook(CallbackRef);
            
            // Cache inputs
            ValidInputs = new SortedSet<int>(configuration.Configuration.Keys);

            // Also add Esc
            ValidInputs.Add(27);
        }

        public void StopHook()
        {
            UnhookWindowsHookEx(hookID);
        }


        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        /**
         * Callback on Key event
         */
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Check for correct input 
            if (nCode >= 0 
                    && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_KEYUP)
                    && ValidInputs.Contains(Marshal.ReadInt32(lParam))
                )
            {
                EventQueue.Add(
                        new Event(
                                (wParam == (IntPtr)WM_KEYDOWN) ? Event.EventType.PRESS : Event.EventType.RELEASE, 
                                Marshal.ReadInt32(lParam)
                            )
                    );

                // Suppress keyboard event
                return (IntPtr)(-1);
            }
            else
            {
                // Don't supress input
                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }
           
        }
    }
}
