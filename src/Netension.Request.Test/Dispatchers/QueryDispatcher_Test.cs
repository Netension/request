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
    public class QueryDispatcher_Test
    {

        private readonly ILogger<QueryDispatcher> _logger;
        private Mock<IServiceProvider> _serviceProviderMock;

        public QueryDispatcher_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<QueryDispatcher>();
        }

        private QueryDispatcher CreateSUT()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();

            return new QueryDispatcher(_serviceProviderMock.Object, _logger);
        }

        [Fact(DisplayName = "QueryDispatcher - DispatchAsnyc - Resolve handler")]
        public async Task QueryDispatcher_DispatchAsync_ResolveHandler()
        {
            // Arrange
            var sut = CreateSUT();
            _serviceProviderMock.Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(new Mock<IQueryHandler<Query<object>, object>>().Object);

            // Act
            await sut.DispatchAsync(new Query<object>(), CancellationToken.None);

            // Assert
            _serviceProviderMock.Verify(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IQueryHandler<Query<object>, object>)))), Times.Once);
        }

        [Fact(DisplayName = "QueryDispatcher - DispatchAsnyc - Call handler")]
        public async Task QueryDispatcher_DispatchAsync_CallHandler()
        {
            // Arrange
            var sut = CreateSUT();
            var handlerMock = new Mock<IQueryHandler<Query<object>, object>>();
            var query = new Query<object>();

            _serviceProviderMock.Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(handlerMock.Object);

            // Act
            await sut.DispatchAsync(query, CancellationToken.None);

            // Assert
            handlerMock.Verify(handlerMock => handlerMock.HandleAsync(It.Is<Query<object>>(q => q.Equals(query)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "QueryDispather - DispatchAsync - Handler not found")]
        public async Task QueryDispatcher_DispatchAsync_HandlerNotFound()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.DispatchAsync(new Query<object>(), CancellationToken.None));
        }

        [Fact(DisplayName = "QueryDispatcher - DispatchAsnyc - Handler throws exception")]
        public async Task QueryDispatcher_DispatchAsync_HandlerThrowsException()
        {
            // Arrange
            var sut = CreateSUT();

            var handlerMock = new Mock<IQueryHandler<Query<object>, object>>();

            _serviceProviderMock.Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(handlerMock.Object);

            handlerMock.Setup(h => h.HandleAsync(It.IsAny<Query<object>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.DispatchAsync(new Query<object>(), CancellationToken.None));
        }
    }
}
