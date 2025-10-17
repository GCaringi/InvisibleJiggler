using System;
using System.Runtime.InteropServices;

namespace InvisibleJiggler.WindowsApi
{
    public static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern uint SetThreadExecutionState(uint esFlags);

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
    }
}
