using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Behaviors;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Dispatchers;
using System;
using System.Collections.Generic;
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
            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(ICommandHandler<Command>)))))
                .Returns(new Mock<ICommandHandler<Command>>().Object);

            // Act
            await sut.DispatchAsync(new Command(), CancellationToken.None).ConfigureAwait(false);

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

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(ICommandHandler<Command>)))))
                .Returns(handlerMock.Object);

            // Act
            await sut.DispatchAsync(command, CancellationToken.None).ConfigureAwait(false);

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
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.DispatchAsync(new Command(), CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "CommandDispatcher - DispatchAsnyc - Handler throws exception")]
        public async Task CommandDispatcher_DispatchAsync_HandlerThrowsException()
        {
            // Arrange
            var sut = CreateSUT();

            var handlerMock = new Mock<ICommandHandler<Command>>();

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(ICommandHandler<Command>)))))
                .Returns(handlerMock.Object);

            handlerMock.Setup(h => h.HandleAsync(It.IsAny<Command>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.DispatchAsync(new Command(), CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "[UNT-PRHB001]: Call 'PreHandler' behavior (Command)")]
        [Trait("Feature", "PRHB - Pre Handler Behavior")]
        public async Task CommandDispacted_DispatchAsync_PreCommandHandler()
        {
            // Arrange
            var sut = CreateSUT();
            var preCommandHandlerMock = new Mock<IPreCommandHandler<Command>>();
            var command = new Command();

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(ICommandHandler<Command>))))).Returns(new Mock<ICommandHandler<Command>>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IEnumerable<IPreCommandHandler<Command>>))))).Returns(new List<IPreCommandHandler<Command>> { preCommandHandlerMock.Object, preCommandHandlerMock.Object });

            // Act
            await sut.DispatchAsync(command, default).ConfigureAwait(false);

            // Assert
            preCommandHandlerMock.Verify(pch => pch.PreHandleAsync(It.Is<Command>(c => c.Equals(command)), It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "[UNT-PRHB002]: 'PreHandler' not configured (Command)")]
        [Trait("Feature", "PRHB - Pre Handler Behavior")]
        public async Task CommandDispacted_DispatchAsync_PreCommandHandlerNotFound()
        {
            // Arrange
            var sut = CreateSUT();
            var commandHandlerMock = new Mock<ICommandHandler<Command>>();
            var command = new Command();

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(ICommandHandler<Command>))))).Returns(commandHandlerMock.Object);

            // Act
            await sut.DispatchAsync(command, default).ConfigureAwait(false);

            // Assert
            commandHandlerMock.Verify(pch => pch.HandleAsync(It.Is<Command>(c => c.Equals(command)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "[UNT-POHB001]: Call 'PostHandler' behavior (Command)")]
        [Trait("Feature", "POHB - Post Handler Behavior")]
        public async Task CommandDispacted_DispatchAsync_PostCommandHandler()
        {
            // Arrange
            var sut = CreateSUT();
            var postCommandHandlerMock = new Mock<IPostCommandHandler<Command>>();
            var command = new Command();

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(ICommandHandler<Command>))))).Returns(new Mock<ICommandHandler<Command>>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IEnumerable<IPostCommandHandler<Command>>))))).Returns(new List<IPostCommandHandler<Command>> { postCommandHandlerMock.Object, postCommandHandlerMock.Object });

            // Act
            await sut.DispatchAsync(command, default).ConfigureAwait(false);

            // Assert
            postCommandHandlerMock.Verify(pch => pch.PostHandleAsync(It.Is<Command>(c => c.Equals(command)), It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "[UNT-POHB002]: 'PostHandler' not configured (Command)")]
        [Trait("Feature", "POHB - Post Handler Behavior")]
        public async Task CommandDispacted_DispatchAsync_PostCommandHandlerNotFound()
        {
            // Arrange
            var sut = CreateSUT();
            var commandHandlerMock = new Mock<ICommandHandler<Command>>();
            var command = new Command();

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(ICommandHandler<Command>))))).Returns(commandHandlerMock.Object);

            // Act
            await sut.DispatchAsync(command, default).ConfigureAwait(false);

            // Assert
            commandHandlerMock.Verify(pch => pch.HandleAsync(It.Is<Command>(c => c.Equals(command)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "[UNT-FLHB001]: Call 'FailureHandler' behavior (Command)")]
        [Trait("Feature", "FLHB - Failure Handler Behavior")]
        public async Task CommandDispacted_DispatchAsync_FailureCommandHandler()
        {
            // Arrange
            var sut = CreateSUT();
            var failureCommandHandlerMock = new Mock<IFailureCommandHandler<Command>>();
            var commandHandlerMock = new Mock<ICommandHandler<Command>>();
            var command = new Command();

            commandHandlerMock.Setup(ch => ch.HandleAsync(It.IsAny<Command>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(ICommandHandler<Command>))))).Returns(commandHandlerMock.Object);
            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IEnumerable<IFailureCommandHandler<Command>>))))).Returns(new List<IFailureCommandHandler<Command>> { failureCommandHandlerMock.Object, failureCommandHandlerMock.Object });

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.DispatchAsync(command, default).ConfigureAwait(false)).ConfigureAwait(false);
            failureCommandHandlerMock.Verify(pch => pch.FailHandleAsync(It.Is<Command>(c => c.Equals(command)), It.IsAny<Exception>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "[UNT-FLHB002]: 'FailureHandler' not configured (Command)")]
        [Trait("Feature", "FLHB - Failure Handler Behavior")]
        public async Task CommandDispacted_DispatchAsync_FailureCommandHandlerNotFound()
        {
            // Arrange
            var sut = CreateSUT();
            var commandHandlerMock = new Mock<ICommandHandler<Command>>();
            var command = new Command();

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(ICommandHandler<Command>))))).Returns(commandHandlerMock.Object);

            // Act
            await sut.DispatchAsync(command, default).ConfigureAwait(false);

            // Assert
            commandHandlerMock.Verify(pch => pch.HandleAsync(It.Is<Command>(c => c.Equals(command)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
