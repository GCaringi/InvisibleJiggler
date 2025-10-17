namespace InvisibleJiggler.Windows.Api.Interface
{
    public interface IWindowsApiService
    {
        uint SetThreadExecutionState(uint esFlags);
        uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
    }
}
