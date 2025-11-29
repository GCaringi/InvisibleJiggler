using InvisibleJiggler.Options;
using InvisibleJiggler.Windows.Api;
using InvisibleJiggler.Windows.Api.Interface;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;

namespace InvisibleJiggler.Services
{
    public class MouseJigglerService : BackgroundService
    {
        private readonly ILogger<MouseJigglerService> _logger;

        private readonly IWindowsApiService _windowsApi;
        private readonly JigglerOptions _options;
        private readonly Random _random = new();

        public MouseJigglerService(
            ILogger<MouseJigglerService> logger,
            IWindowsApiService windowsApi,
            IOptions<JigglerOptions> options)
        {
            _logger = logger;
            
            _windowsApi = windowsApi;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Mouse Jiggler Service is starting.");

            _logger.LogInformation("Writing PID file...");
            WritePidFile();

            _windowsApi.SetThreadExecutionState(
                Constants.ES_CONTINUOUS |
                Constants.ES_SYSTEM_REQUIRED |
                Constants.ES_DISPLAY_REQUIRED);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    PerformMouseMovement();

                    var delay = _random.Next(_options.MaxIntervalMs, _options.MaxIntervalMs);
                    
                    await Task.Delay(delay, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Mouse Jiggler Service is stopping due to cancellation.");
            }
            finally
            {
                _windowsApi.SetThreadExecutionState(Constants.ES_CONTINUOUS);
                DeletePidFile();
            }
        }

        /// <summary>
        /// Creates a process ID (PID) file in the application's base directory containing the current process ID.
        /// </summary>
        /// <remarks>The PID file is named "mousejiggler.pid" and is overwritten if it already exists.
        /// This file can be used by external tools or scripts to identify or manage the running process.</remarks>
        private void WritePidFile()
        {
            var pidFile = Path.Combine(AppContext.BaseDirectory, 
                "mousejiggler.pid");

            var pid = Environment.ProcessId;
            File.WriteAllText(pidFile, pid.ToString());
            _logger.LogInformation("PID file created for pid {0}", pid);
        }

        /// <summary>
        /// Deletes the PID file from the application's base directory if it exists.
        /// </summary>
        /// <remarks>This method is typically used to remove the process identifier (PID) file created by
        /// the application, which may be used to track the running instance. If the PID file does not exist, no action
        /// is taken.</remarks>
        private void DeletePidFile()
        {
            var pidFile = Path.Combine(AppContext.BaseDirectory, "mouseJiggler.pid");

            if (File.Exists(pidFile))
            {
                File.Delete(pidFile);
                _logger.LogInformation("PID file deleted");
            }
        }

        /// <summary>
        /// Simulates a mouse movement in a random direction for a random distance, then returns the mouse to its
        /// original position.
        /// </summary>
        /// <remarks>This method generates a random movement within the configured minimum and maximum
        /// distance, moves the mouse accordingly, and then reverses the movement. The operation is synchronous and
        /// includes a short delay between the outbound and return movements. Intended for internal use to automate
        /// mouse activity.</remarks>
        private void PerformMouseMovement()
        {
            int distance = _random.Next(_options.MinDistance, _options.MaxDistance + 1);
            int direction = _random.Next(0, 4);

            var input = CreateMouseInput(distance, direction);
            SendMouseInput(input);

            Task.Delay(200).Wait();

            var returnInput = CreateReturnInput(input, distance, direction);
            SendMouseInput(returnInput);

            _logger.LogInformation("Mouse moved {0} units in direction {1} and returned", distance, direction);
        }

        /// <summary>
        /// Creates an INPUT structure representing a mouse movement in a specified direction and distance.
        /// </summary>
        /// <remarks>The method does not perform any validation on the direction parameter. Supplying a
        /// value outside the range 0–3 will result in an INPUT structure with uninitialized mouse movement
        /// data.</remarks>
        /// <param name="distance">The number of units to move the mouse cursor. Must be a non-negative integer.</param>
        /// <param name="direction">The direction in which to move the mouse cursor. Valid values are: 0 (up), 1 (right), 2 (down), and 3
        /// (left).</param>
        /// <returns>An INPUT structure configured to simulate a mouse movement in the specified direction and distance.</returns>
        internal INPUT CreateMouseInput(int distance, int direction)
        {
            var input = new INPUT
            {
                type = Constants.INPUT_MOUSE,
            };

            switch (direction)
            {
                case 0:
                    input.mi = new MOUSEINPUT
                    {
                        dx = 0,
                        dy = -distance,
                        dwFlags = Constants.MOUSEEVENTF_MOVE
                    };
                    break;
                case 1:
                    input.mi = new MOUSEINPUT
                    {
                        dx = distance,
                        dy = 0,
                        dwFlags = Constants.MOUSEEVENTF_MOVE
                    };
                    break;
                case 2:
                    input.mi = new MOUSEINPUT
                    {
                        dx = 0,
                        dy = distance,
                        dwFlags = Constants.MOUSEEVENTF_MOVE
                    };
                    break;
                case 3:
                    input.mi = new MOUSEINPUT
                    {
                        dx = -distance,
                        dy = 0,
                        dwFlags = Constants.MOUSEEVENTF_MOVE
                    };
                    break;
            }

            return input;
        }

        /// <summary>
        /// Creates a new INPUT structure representing a mouse movement relative to the original input, based on the
        /// specified distance and direction.
        /// </summary>
        /// <remarks>The direction parameter determines which axis and direction the mouse movement will
        /// occur: 0 for down, 1 for left, 2 for up, and 3 for right. The original input is not modified.</remarks>
        /// <param name="original">The original INPUT structure to use as the basis for the new input.</param>
        /// <param name="distance">The distance, in pixels, to move the mouse. Must be a non-negative integer.</param>
        /// <param name="direction">The direction of the mouse movement. Valid values are 0 (down), 1 (left), 2 (up), and 3 (right).</param>
        /// <returns>A new INPUT structure with the mouse movement adjusted according to the specified distance and direction.</returns>
        internal INPUT CreateReturnInput(INPUT original, int distance, int direction)
        {
            var input = original;
            switch(direction)
            {
                case 0:
                    input.mi.dy = distance;
                    break;
                case 1:
                    input.mi.dx = -distance;
                    break;
                case 2:
                    input.mi.dy = -distance;
                    break;
                case 3:
                    input.mi.dx = distance;
                    break;
            }

            return input;
        }

        /// <summary>
        /// Sends a single mouse input event to the operating system using the specified input structure.
        /// </summary>
        /// <remarks>This method is intended for internal use when simulating mouse actions at a low
        /// level. The caller is responsible for ensuring that the input structure is valid and represents a supported
        /// mouse event.</remarks>
        /// <param name="input">The mouse input data to send. The structure must be properly initialized to represent the desired mouse
        /// action.</param>
        internal void SendMouseInput(INPUT input)
        {
            _windowsApi.SendInput(1, [input], Marshal.SizeOf<INPUT>());
        }
    }
}
