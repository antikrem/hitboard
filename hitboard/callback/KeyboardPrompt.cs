using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace hitboard.callback
{
    class KeyboardPrompt
    {
        // Function prototype for callback
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Reference to callback to ensure it will not be GCed
        LowLevelKeyboardProc CallbackRef = HookCallback;


        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        private static IntPtr hookID = IntPtr.Zero;

        // Main thread blocker
        private static ManualResetEvent PromptBlock;

        // Prompted key
        private static int PromptedKey = -1;

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


        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        // Sets up hook
        public KeyboardPrompt()
        {
            hookID = SetHook(CallbackRef);
            PromptBlock = new ManualResetEvent(false);
        }

        // Wait until input
        public int Resolve()
        {
            PromptBlock.WaitOne();

            UnhookWindowsHookEx(hookID);

            return PromptedKey;
        }

        /**
        * Callback on Key event, sets Prompted Key
        */
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                PromptedKey = Marshal.ReadInt32(lParam);
            }

            // Signal main thread to continue
            PromptBlock.Set();

            // Suppress keyboard event
            return (IntPtr)(-1);
        }
    }
}
