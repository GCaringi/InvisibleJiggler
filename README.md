# Mouse Jiggler .NET

A lightweight, portable .NET console application that simulates mouse movements to prevent system sleep and screen lock. Perfect for environments where you cannot install external programs or need a minimal footprint solution.

## ✨ Features

- **No installation required**: Runs as a simple console application without admin privileges
- **Ultra-lightweight**: No external dependencies, only native Windows API calls via P/Invoke
- **Minimal memory footprint**: Console-based architecture keeps resource usage extremely low
- **Natural movements**: Random movements in 4 directions with variable distances
- **Prevents sleep**: Uses `SetThreadExecutionState` to prevent system standby and screen timeout
- **Portable**: Single executable that can run from any folder without installation
- **Clean code**: Modular structure with separation of concerns

## 🎯 Use Case

This project was born from the need to have a mouse jiggler on computers where:
- Installing external programs is restricted or prohibited
- Administrative privileges are not available
- A portable solution is required
- Minimal resource consumption is essential

## 🚀 Requirements

- Windows 10/11
- .NET 10.0 Runtime

## 📦 Installation

1. Clone the repository:
git clone https://github.com/yourusername/mouse-jiggler-dotnet.git
cd mouse-jiggler-dotnet

2. Build the project:
dotnet build -c Release

3. Run the application:
dotnet run

## 🔧 How It Works

1. **SetThreadExecutionState**: Prevents the system from entering standby and the screen from turning off using `ES_CONTINUOUS`, `ES_SYSTEM_REQUIRED`, and `ES_DISPLAY_REQUIRED` flags

2. **SendInput**: Simulates real mouse movements that are recognized by Windows and applications as user activity

3. **Smart loop**: Random movements every 3-4 minutes with variable directions and distances

## ⚙️ Customization

You can easily modify:

- **Movement interval**: Change `Thread.Sleep(rnd.Next(180000, 240000))` in `MouseJiggler.cs`
- **Movement distance**: Modify `_random.Next(50, 101)` for different pixel ranges
- **Sleep behavior**: Remove flags in `Constants.cs`

## 📝 Notes

- The application does not require administrator privileges
- Movement always returns to original position
- Does not interfere with normal PC usage
- On exit (Ctrl+C) automatically restores power saving settings

## 🤝 Contributing

Contributions are welcome! Feel free to open issues or pull requests.

## 📄 License

This project is released under the MIT License. See LICENSE file for details.

## ⚠️ Disclaimer

This software is provided "as is" for educational and personal purposes. Use at your own risk and responsibility.