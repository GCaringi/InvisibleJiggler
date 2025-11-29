using InvisibleJiggler.Services;
using InvisibleJiggler.Windows.Api;
using InvisibleJiggler.Windows.Api.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Runtime.InteropServices;
using JigglerOpts = InvisibleJiggler.Options;

namespace InvisibleJiggler.Tests;

public class InvisibleJigglerServiceTests
{
    private readonly Mock<IWindowsApiService> _mockWindowsApi;
    private readonly Mock<ILogger<MouseJigglerService>> _mockLogger;
    private readonly IOptions<JigglerOpts.JigglerOptions> _options;

    public InvisibleJigglerServiceTests()
    {
        _mockWindowsApi = new Mock<IWindowsApiService>();
        _mockLogger = new Mock<ILogger<MouseJigglerService>>();
        _options = Microsoft.Extensions.Options.Options.Create(new JigglerOpts.JigglerOptions
        {
            MinIntervalMs = 180000,
            MaxIntervalMs = 240000,
            MinDistance = 50,
            MaxDistance = 100
        });
    }

    [Fact]
    public void CreateMouseInput_DirectionUp_CreatesCorrectInput()
    {
        // Arrange
        var service = new MouseJigglerService(_mockLogger.Object, _mockWindowsApi.Object, _options);
        int distance = 75;
        int direction = 0; // Up

        // Act
        var result = service.CreateMouseInput(distance, direction);

        // Assert
        Assert.Equal(Constants.INPUT_MOUSE, result.type);
        Assert.Equal(0, result.mi.dx);
        Assert.Equal(-distance, result.mi.dy);
        Assert.Equal(Constants.MOUSEEVENTF_MOVE, result.mi.dwFlags);
    }

    [Fact]
    public void CreateMouseInput_DirectionRight_CreatesCorrectInput()
    {
        // Arrange
        var service = new MouseJigglerService(_mockLogger.Object, _mockWindowsApi.Object, _options);
        int distance = 80;
        int direction = 1; // Right

        // Act
        var result = service.CreateMouseInput(distance, direction);

        // Assert
        Assert.Equal(Constants.INPUT_MOUSE, result.type);
        Assert.Equal(distance, result.mi.dx);
        Assert.Equal(0, result.mi.dy);
        Assert.Equal(Constants.MOUSEEVENTF_MOVE, result.mi.dwFlags);
    }

    [Fact]
    public void CreateMouseInput_DirectionDown_CreatesCorrectInput()
    {
        // Arrange
        var service = new MouseJigglerService(_mockLogger.Object, _mockWindowsApi.Object, _options);
        int distance = 60;
        int direction = 2; // Down

        // Act
        var result = service.CreateMouseInput(distance, direction);

        // Assert
        Assert.Equal(Constants.INPUT_MOUSE, result.type);
        Assert.Equal(0, result.mi.dx);
        Assert.Equal(distance, result.mi.dy);
        Assert.Equal(Constants.MOUSEEVENTF_MOVE, result.mi.dwFlags);
    }

    [Fact]
    public void CreateMouseInput_DirectionLeft_CreatesCorrectInput()
    {
        // Arrange
        var service = new MouseJigglerService(_mockLogger.Object, _mockWindowsApi.Object, _options);
        int distance = 90;
        int direction = 3; // Left

        // Act
        var result = service.CreateMouseInput(distance, direction);

        // Assert
        Assert.Equal(Constants.INPUT_MOUSE, result.type);
        Assert.Equal(-distance, result.mi.dx);
        Assert.Equal(0, result.mi.dy);
        Assert.Equal(Constants.MOUSEEVENTF_MOVE, result.mi.dwFlags);
    }

    [Theory]
    [InlineData(0, 50, 0, -50)]  // Up
    [InlineData(1, 75, 75, 0)]   // Right
    [InlineData(2, 100, 0, 100)] // Down
    [InlineData(3, 65, -65, 0)]  // Left
    public void CreateMouseInput_AllDirections_CreatesCorrectMovement(
        int direction, int distance, int expectedDx, int expectedDy)
    {
        // Arrange
        var service = new MouseJigglerService(_mockLogger.Object, _mockWindowsApi.Object, _options);

        // Act
        var result = service.CreateMouseInput(distance, direction);

        // Assert
        Assert.Equal(expectedDx, result.mi.dx);
        Assert.Equal(expectedDy, result.mi.dy);
        Assert.Equal(Constants.MOUSEEVENTF_MOVE, result.mi.dwFlags);
    }

    [Theory]
    [InlineData(0, 50)]  // Up
    [InlineData(1, 75)]  // Right
    [InlineData(2, 100)] // Down
    [InlineData(3, 65)]  // Left
    public void CreateReturnInput_ReturnsOppositeMovement(int direction, int distance)
    {
        // Arrange
        var service = new MouseJigglerService(_mockLogger.Object, _mockWindowsApi.Object, _options);
        var originalInput = service.CreateMouseInput(distance, direction);

        // Act
        var returnInput = service.CreateReturnInput(originalInput, distance, direction);

        // Assert
        switch (direction)
        {
            case 0: // Up -> Down
                Assert.Equal(distance, returnInput.mi.dy);
                break;
            case 1: // Right -> Left
                Assert.Equal(-distance, returnInput.mi.dx);
                break;
            case 2: // Down -> Up
                Assert.Equal(-distance, returnInput.mi.dy);
                break;
            case 3: // Left -> Right
                Assert.Equal(distance, returnInput.mi.dx);
                break;
        }
    }

    [Fact]
    public void SendMouseInput_CallsWindowsApiWithCorrectParameters()
    {
        // Arrange
        var service = new MouseJigglerService(_mockLogger.Object, _mockWindowsApi.Object, _options);
        var input = new INPUT
        {
            type = Constants.INPUT_MOUSE,
            mi = new MOUSEINPUT { dx = 50, dy = 0, dwFlags = Constants.MOUSEEVENTF_MOVE }
        };

        _mockWindowsApi.Setup(x => x.SendInput(
            It.IsAny<uint>(),
            It.IsAny<INPUT[]>(),
            It.IsAny<int>()))
            .Returns(1u);

        // Act
        service.SendMouseInput(input);

        // Assert
        _mockWindowsApi.Verify(x => x.SendInput(
            1,
            It.Is<INPUT[]>(arr => arr.Length == 1 && arr[0].mi.dx == 50),
            Marshal.SizeOf(typeof(INPUT))),
            Times.Once);
    }
}
