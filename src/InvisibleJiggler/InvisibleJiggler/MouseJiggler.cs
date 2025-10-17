using System;
using System.Runtime.InteropServices;
using System.Threading;
using InvisibleJiggler.WindowsApi;

namespace InvisibleJiggler
{
    internal class MouseJiggler
    {
        private volatile bool _running = true;
        private readonly Random _random = new Random();

        public void Start()
        {
            Console.CancelKeyPress += OnCancelKeyPress;

            // Attiva protezione sonno
            NativeMethods.SetThreadExecutionState(
                Constants.ES_CONTINUOUS |
                Constants.ES_SYSTEM_REQUIRED |
                Constants.ES_DISPLAY_REQUIRED);

            Console.WriteLine("Jiggling is activer. Ctrl+C for exit.");

            RunJigglerLoop();

            // Ripristina stato normale
            NativeMethods.SetThreadExecutionState(Constants.ES_CONTINUOUS);
            Console.WriteLine("Jiggling stopped.");
        }

        private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _running = false;
        }

        private void RunJigglerLoop()
        {
            while (_running)
            {
                PerformMouseMovement();
                Thread.Sleep(1000);
            }
        }

        private void PerformMouseMovement()
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

        private INPUT CreateMouseInput(int distance, int direction)
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

        private INPUT CreateReturnInput(INPUT original, int distance, int direction)
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

        private void SendMouseInput(INPUT input)
        {
            NativeMethods.SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
