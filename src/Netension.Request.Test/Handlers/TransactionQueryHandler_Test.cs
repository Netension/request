using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Infrastructure.EFCore.Abstractions;
using Netension.Request.Infrastructure.EFCore.Handlers;
using System.Threading.Tasks;
using System.Threading;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Handlers
{
    public class TransactionQueryHandler_Test
    {
        private Mock<ITransactionManager> _transactionManagerMock;
        private Mock<IQueryHandler<Query<object>, object>> _nextMock;
        private ILogger<TransactionQueryHandler<Query<object>, object>> _logger;

        public TransactionQueryHandler_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<TransactionQueryHandler<Query<object>, object>>();
        }

        private TransactionQueryHandler<Query<object>, object> CreateSUT()
        {
            _transactionManagerMock = new Mock<ITransactionManager>();
            _nextMock = new Mock<IQueryHandler<Query<object>, object>>();

            return new TransactionQueryHandler<Query<object>, object>(_transactionManagerMock.Object, _nextMock.Object, _logger);
        }

        [Fact(DisplayName = "[UNT-TCH001]: Begin Transaction (Query)")]
        [Trait("Feature", "TCH - Transactional Handler")]
        public async Task EFCore_TransactionalCommandHandler_BeginTransaction()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await sut.HandleAsync(new Query<object>(), default);

            // Assert
            _transactionManagerMock.Verify(tm => tm.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "[UNT-TCH002]: Running Transaction (Query)")]
        [Trait("Feature", "TCH - Transactional Handler")]
        public async Task EFCore_TransactionalCommandHandler_RunningTransaction()
        {
            // Arrange
            var sut = CreateSUT();

            _transactionManagerMock.SetupGet(tm => tm.IsActive).Returns(true);

            // Act
            await sut.HandleAsync(new Query<object>(), default);

            // Assert
            _transactionManagerMock.Verify(tm => tm.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "[UNT-TCH003]: Commit 'Transaction' (Query)")]
        [Trait("Feature", "TCH - Transactional Handler")]
        public async Task EFCore_TransactionalCommandHandler_CommitTransaction()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await sut.HandleAsync(new Query<object>(), default);

            // Assert
            _transactionManagerMock.Verify(tm => tm.CommitAsnyc(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "[UNT-TCH004]: Rollback 'Transaction' (Query)")]
        [Trait("Feature", "TCH - Transactional Handler")]
        public async Task EFCore_TransactionalCommandHandler_RollbackTransaction()
        {
            // Arrange
            var sut = CreateSUT();

            _nextMock.Setup(n => n.HandleAsync(It.IsAny<Query<object>>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            // Act

            // Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.HandleAsync(new Query<object>(), default));
            _transactionManagerMock.Verify(tm => tm.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
