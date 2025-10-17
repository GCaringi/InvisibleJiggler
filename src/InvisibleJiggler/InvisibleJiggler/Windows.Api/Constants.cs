namespace InvisibleJiggler.WindowsApi
{
    public static class Constants
    {
        //Thread execution state flags
        public const uint ES_CONTINUOUS = 0x80000000;
        // Prevents the system from entering sleep or turning off the display
        public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        // Prevents the display from being turned off
        public const uint ES_DISPLAY_REQUIRED = 0x00000002;

        // SendInput flags
        public const uint INPUT_MOUSE = 0;
        // Mouse event flag for movement
        public const uint MOUSEEVENTF_MOVE = 0x0001;
    }
}