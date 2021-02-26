using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Dispatchers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Dispatchers
{
    public class CommandDispatcher_Test
    {
        private Mock<IServiceProvider> _serviceProviderMock;
        private readonly ILogger<CommandDispatcher> _logger;

        public CommandDispatcher_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<CommandDispatcher>();
        }

        private CommandDispatcher CreateSUT()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();

            return new CommandDispatcher(_serviceProviderMock.Object, _logger);
        }

        [Fact(DisplayName = "CommandDispather - DispatchAsync - Get handler")]
        public async Task CommandDispatcher_DispatchAsync_GetHandler()
        {
            // Arrange
            var sut = CreateSUT();
            _serviceProviderMock.Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(new Mock<ICommandHandler<Command>>().Object);

            // Act
            await sut.DispatchAsync(new Command(), CancellationToken.None);

            // Assert
            _serviceProviderMock.Verify(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(ICommandHandler<Command>)))), Times.Once);
        }

        [Fact(DisplayName = "CommandDispather - DispatchAsync - Call handler")]
        public async Task CommandDispatcher_DispatchAsync_CallHandler()
        {
            // Arrange
            var sut = CreateSUT();
            var handlerMock = new Mock<ICommandHandler<Command>>();
            var command = new Command();

            _serviceProviderMock.Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(handlerMock.Object);

            // Act
            await sut.DispatchAsync(command, CancellationToken.None);

            // Assert
            handlerMock.Verify(h => h.HandleAsync(It.Is<Command>(c => c.Equals(command)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "CommandDispather - DispatchAsync - Handler not found")]
        public async Task CommandDispatcher_DispatchAsync_HandlerNotFound()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.DispatchAsync(new Command(), CancellationToken.None));
        }

        [Fact(DisplayName = "CommandDispatcher - DispatchAsnyc - Handler throws exception")]
        public async Task CommandDispatcher_DispatchAsync_HandlerThrowsException()
        {
            // Arrange
            var sut = CreateSUT();

            var handlerMock = new Mock<ICommandHandler<Command>>();

            _serviceProviderMock.Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(handlerMock.Object);

            handlerMock.Setup(h => h.HandleAsync(It.IsAny<Command>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.DispatchAsync(new Command(), CancellationToken.None));
        }
    }
}
