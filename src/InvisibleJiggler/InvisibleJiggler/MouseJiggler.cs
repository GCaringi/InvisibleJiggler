using InvisibleJiggler.Windows.Api;
using InvisibleJiggler.Windows.Api.Interface;
using System.Runtime.InteropServices;

namespace InvisibleJiggler
{
    public class MouseJiggler
    {
        private readonly IWindowsApiService _windowsApi;
        private volatile bool _running = true;
        private readonly Random _random = new Random();

        public MouseJiggler() : this(new WindowsApiService(), new Random())
        {
        }
        public MouseJiggler(IWindowsApiService windowsApi, Random random)
        {
            _windowsApi = windowsApi ?? throw new ArgumentNullException(nameof(windowsApi));
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        internal void Start()
        {
            Console.CancelKeyPress += OnCancelKeyPress;

            // Attiva protezione sonno
            _windowsApi.SetThreadExecutionState(
                Constants.ES_CONTINUOUS |
                Constants.ES_SYSTEM_REQUIRED |
                Constants.ES_DISPLAY_REQUIRED);

            Console.WriteLine("Jiggling is activer. Ctrl+C for exit.");

            RunJigglerLoop();

            // Ripristina stato normale
            _windowsApi.SetThreadExecutionState(Constants.ES_CONTINUOUS);
            Console.WriteLine("Jiggling stopped.");
        }

        internal void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _running = false;
        }

        internal void RunJigglerLoop()
        {
            while (_running)
            {
                PerformMouseMovement();
                Thread.Sleep(1000);
            }
        }

        internal void PerformMouseMovement()
        {
            int distance = _random.Next(0, 1);
            int direction = _random.Next(0, 4);

            var input = CreateMouseInput(distance, direction);
            SendMouseInput(input);
#if DEBUG
            Console.WriteLine($"Jiggling set");
#endif
            Thread.Sleep(500);
    
            var returnInput = CreateReturnInput(input, distance, direction);
            SendMouseInput(returnInput);
#if DEBUG
            Console.WriteLine("Jiggling reset");
#endif
        }

        internal INPUT CreateMouseInput(int distance, int direction)
        {
            var input = new INPUT { type = Constants.INPUT_MOUSE };

            switch (direction)
            {
                case 0: // Up
                    input.mi = new MOUSEINPUT { dx = 0, dy = -distance, dwFlags = Constants.MOUSEEVENTF_MOVE };
                    break;
                case 1: // Right
                    input.mi = new MOUSEINPUT { dx = distance, dy = 0, dwFlags = Constants.MOUSEEVENTF_MOVE };
                    break;
                case 2: // Down
                    input.mi = new MOUSEINPUT { dx = 0, dy = distance, dwFlags = Constants.MOUSEEVENTF_MOVE };
                    break;
                case 3: // Left
                    input.mi = new MOUSEINPUT { dx = -distance, dy = 0, dwFlags = Constants.MOUSEEVENTF_MOVE };
                    break;
            }

            return input;
        }

        internal INPUT CreateReturnInput(INPUT original, int distance, int direction)
        {
            var input = original;
            switch (direction)
            {
                case 0: input.mi.dy = distance; break;
                case 1: input.mi.dx = -distance; break;
                case 2: input.mi.dy = -distance; break;
                case 3: input.mi.dx = distance; break;
            }
            return input;
        }

        internal void SendMouseInput(INPUT input)
        {
            _windowsApi.SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        public void Stop()
        {
            _running = false;
        }
    }
}
