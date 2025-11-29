using System;
using System.Runtime.InteropServices;

namespace InvisibleJiggler.Windows.Api
{
    internal static partial class NativeMethods
    {
        [LibraryImport("kernel32.dll")]
        public static partial uint SetThreadExecutionState(uint esFlags);

        [LibraryImport("user32.dll")]
        public static partial uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
    }
}
