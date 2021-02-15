using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace hitboard.callback
{
    class AsciiConverter
    {
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKeyW(
          uint uCode,
          uint uMapType
        );

        [DllImport("user32.dll")]
        private static extern int GetKeyNameTextW(
            int lParam,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder receivingBuffer,
            int cchSize
        );

        public static string KeycodeToAscii(int key)
        {
            var buffer = new StringBuilder(1024);

            uint scanCode = MapVirtualKeyW((uint)key, 0);
            GetKeyNameTextW((int)(scanCode << 16), buffer, 1024);

            return buffer.ToString();
        }
    }
}
