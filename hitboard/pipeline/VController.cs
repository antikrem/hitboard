using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Runtime.InteropServices;

namespace hitboard.pipeline
{
    /**
     * Manages Vcontroller interface
     * Which links to 
     */
    class VController
    {
        [DllImport("VIGEMWRAPPER.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ViGEmWrapper_Initialise();

        [DllImport("VIGEMWRAPPER.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ViGEmWrapper_Input(out XInputState state);

        [DllImport("VIGEMWRAPPER.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ViGEmWrapper_Free();

        /**
         * Create ViGEm Client instance and initialise library
         */
        public static int Initialise()
        {
            int success = ViGEmWrapper_Initialise();

            // Need to wait a semi trivial amount of time
            // To ensure driver backend is fully initialised
            Thread.Sleep(1000);

            return success;
        }

        /**
         * Given an XInput, set state in vController
         */
         public int Input(KeyState keyState)
         {
            XInputState state = keyState.ToXinputState();
            return ViGEmWrapper_Input(out state);
        }

        /**
         * Free and close ViGEm 
         */
        public static int Free()
        {
            return ViGEmWrapper_Free();
        }
    }
}
