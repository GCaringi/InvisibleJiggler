using InvisibleJiggler.Windows.Api.Interface;

namespace InvisibleJiggler.Windows.Api
{
    public class WindowsApiService : IWindowsApiService
    {
        public uint SetThreadExecutionState(uint esFlags)
        {
            return NativeMethods.SetThreadExecutionState(esFlags);
        }

        public uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize)
        {
            return NativeMethods.SendInput(nInputs, pInputs, cbSize);
        }
    }
}
