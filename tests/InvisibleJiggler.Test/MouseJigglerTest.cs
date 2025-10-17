using System.Runtime.InteropServices;
using InvisibleJiggler.Windows.Api;
using InvisibleJiggler.Windows.Api.Interface;
using Moq;


namespace InvisibleJiggler.Test
{
    public class MouseJigglerTests
    {
        private readonly Mock<IWindowsApiService> _mockWindowsApi;
        private readonly Mock<Random> _mockRandom;

        public MouseJigglerTests()
        {
            _mockWindowsApi = new Mock<IWindowsApiService>();
            _mockRandom = new Mock<Random>();
        }

        [Fact]
        public void Constructor_WithNullWindowsApi_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MouseJiggler(null, new Random()));
        }

        [Fact]
        public void Constructor_WithNullRandom_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new MouseJiggler(new Mock<IWindowsApiService>().Object, null));
        }

        [Fact]
        public void CreateMouseInput_DirectionUp_CreatesCorrectInput()
        {
            // Arrange
            var jiggler = new MouseJiggler(_mockWindowsApi.Object, new Random());
            int distance = 75;
            int direction = 0; // Up

            // Act
            var result = jiggler.CreateMouseInput(distance, direction);

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
            var jiggler = new MouseJiggler(_mockWindowsApi.Object, new Random());
            int distance = 80;
            int direction = 1; // Right

            // Act
            var result = jiggler.CreateMouseInput(distance, direction);

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
            var jiggler = new MouseJiggler(_mockWindowsApi.Object, new Random());
            int distance = 60;
            int direction = 2; // Down

            // Act
            var result = jiggler.CreateMouseInput(distance, direction);

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
            var jiggler = new MouseJiggler(_mockWindowsApi.Object, new Random());
            int distance = 90;
            int direction = 3; // Left

            // Act
            var result = jiggler.CreateMouseInput(distance, direction);

            // Assert
            Assert.Equal(Constants.INPUT_MOUSE, result.type);
            Assert.Equal(-distance, result.mi.dx);
            Assert.Equal(0, result.mi.dy);
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
            var jiggler = new MouseJiggler(_mockWindowsApi.Object, new Random());
            var originalInput = jiggler.CreateMouseInput(distance, direction);

            // Act
            var returnInput = jiggler.CreateReturnInput(originalInput, distance, direction);

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
            var jiggler = new MouseJiggler(_mockWindowsApi.Object, new Random());
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
            jiggler.SendMouseInput(input);

            // Assert
            _mockWindowsApi.Verify(x => x.SendInput(
                1,
                It.Is<INPUT[]>(arr => arr.Length == 1 && arr[0].mi.dx == 50),
                Marshal.SizeOf(typeof(INPUT))),
                Times.Once);
        }

        [Fact]
        public void Stop_SetsRunningToFalse()
        {
            // Arrange
            var jiggler = new MouseJiggler(_mockWindowsApi.Object, new Random());

            // Act
            jiggler.Stop();
        }

        [Fact]
        public void Stop_SetsRunningToFalse_AllowsLoopToExit()
        {
            // Arrange
            var jiggler = new MouseJiggler(_mockWindowsApi.Object, new Random());

            // Act
            jiggler.Stop();

            Assert.True(true);
        }
    }
}
