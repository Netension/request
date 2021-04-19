using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Infrastructure.EFCore.Abstractions;
using Netension.Request.Infrastructure.EFCore.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Handlers
{
    public class TransactionalCommandHandler_Test
    {
        private readonly ILogger<TransactionalCommandHandler<Command>> _logger;
        private Mock<ITransactionManager> _transactionManagerMock;
        private Mock<ICommandHandler<Command>> _nextMock;

        public TransactionalCommandHandler_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<TransactionalCommandHandler<Command>>();
        }

        private TransactionalCommandHandler<Command> CreateSUT()
        {
            _transactionManagerMock = new Mock<ITransactionManager>();
            _nextMock = new Mock<ICommandHandler<Command>>();

            return new TransactionalCommandHandler<Command>(_transactionManagerMock.Object, _nextMock.Object, _logger);
        }

        [Fact(DisplayName = "[UNT-TCH001]: Begin Transaction (command)")]
        [Trait("Feature", "TCH - Transactional Handler")]
        public async Task EFCore_TransactionalCommandHandler_BeginTransaction()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await sut.HandleAsync(new Command(), default);

            // Assert
            _transactionManagerMock.Verify(tm => tm.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "[UNT-TCH002]: Running Transaction (Command)")]
        [Trait("Feature", "TCH - Transactional Handler")]
        public async Task EFCore_TransactionalCommandHandler_RunningTransaction()
        {
            // Arrange
            var sut = CreateSUT();

            _transactionManagerMock.SetupGet(tm => tm.IsActive).Returns(true);

            // Act
            await sut.HandleAsync(new Command(), default);

            // Assert
            _transactionManagerMock.Verify(tm => tm.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "[UNT-TCH003]: Commit 'Transaction' (Command)")]
        [Trait("Feature", "TCH - Transactional Handler")]
        public async Task EFCore_TransactionalCommandHandler_CommitTransaction()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await sut.HandleAsync(new Command(), default);

            // Assert
            _transactionManagerMock.Verify(tm => tm.CommitAsnyc(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "[UNT-TCH004]: Rollback 'Transaction' (Command)")]
        [Trait("Feature", "TCH - Transactional Handler")]
        public async Task EFCore_TransactionalCommandHandler_RollbackTransaction()
        {
            // Arrange
            var sut = CreateSUT();

            _nextMock.Setup(n => n.HandleAsync(It.IsAny<Command>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.HandleAsync(new Command(), default));
            _transactionManagerMock.Verify(tm => tm.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
