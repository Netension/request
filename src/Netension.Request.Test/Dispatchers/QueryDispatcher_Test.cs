using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Behaviors;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Dispatchers;
using System;
using System.Collections.Generic;
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
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IQueryHandler<Query<object>, object>)))
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

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IQueryHandler<Query<object>, object>)))))
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

            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IQueryHandler<Query<object>, object>)))
                .Returns(handlerMock.Object);

            handlerMock.Setup(h => h.HandleAsync(It.IsAny<Query<object>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.DispatchAsync(new Query<object>(), CancellationToken.None));
        }

        [Fact(DisplayName = "[UNT-PRHB001]: Call 'PreHandler' behavior (Query)")]
        [Trait("Feature", "PRHB - Pre Handler Behavior")]
        public async Task QueryDispacted_DispatchAsync_PreQueryHandler()
        {
            // Arrange
            var sut = CreateSUT();
            var preQueryHandlerMock = new Mock<IPreQueryHandler<Query<object>, object>>();
            var query = new Query<object>();

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IQueryHandler<Query<object>, object>))))).Returns(new Mock<IQueryHandler<Query<object>, object>>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IEnumerable<IPreQueryHandler<Query<object>, object>>))))).Returns(new List<IPreQueryHandler<Query<object>, object>> { preQueryHandlerMock.Object, preQueryHandlerMock.Object });

            // Act
            await sut.DispatchAsync(query, default);

            // Assert
            preQueryHandlerMock.Verify(pch => pch.PreHandleAsync(It.Is<Query<object>>(q => q.Equals(query)), It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "[UNT-PRHB002]: 'PreHandler' not configured (Query)")]
        [Trait("Feature", "PRHB - Pre Handler Behavior")]
        public async Task QueryDispacted_DispatchAsync_PreQueryHandlerNotFound()
        {
            // Arrange
            var sut = CreateSUT();
            var queryHandlerMock = new Mock<IQueryHandler<Query<object>, object>>();
            var query = new Query<object>();

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IQueryHandler<Query<object>, object>))))).Returns(queryHandlerMock.Object);

            // Act
            await sut.DispatchAsync(query, default);

            // Assert
            queryHandlerMock.Verify(pch => pch.HandleAsync(It.Is<Query<object>>(c => c.Equals(query)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "[UNT-POHB001]: Call 'PostHandler' behavior (Query)")]
        [Trait("Feature", "POHB - Post Handler Behavior")]
        public async Task QueryDispacted_DispatchAsync_PostQueryHandler()
        {
            // Arrange
            var sut = CreateSUT();
            var postQueryHandlerMock = new Mock<IPostQueryHandler<Query<object>, object>>();
            var query = new Query<object>();

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IQueryHandler<Query<object>, object>))))).Returns(new Mock<IQueryHandler<Query<object>, object>>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IEnumerable<IPostQueryHandler<Query<object>, object>>))))).Returns(new List<IPostQueryHandler<Query<object>, object>> { postQueryHandlerMock.Object, postQueryHandlerMock.Object });

            // Act
            await sut.DispatchAsync(query, default);

            // Assert
            postQueryHandlerMock.Verify(pch => pch.PostHandleAsync(It.Is<Query<object>>(q => q.Equals(query)), It.IsAny<object>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "[UNT-PRHB002]: 'PostHandler' not configured (Query)")]
        [Trait("Feature", "POHB - Post Handler Behavior")]
        public async Task QueryDispacted_DispatchAsync_PostQueryHandlerNotFound()
        {
            // Arrange
            var sut = CreateSUT();
            var queryHandlerMock = new Mock<IQueryHandler<Query<object>, object>>();
            var query = new Query<object>();

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IQueryHandler<Query<object>, object>))))).Returns(queryHandlerMock.Object);

            // Act
            await sut.DispatchAsync(query, default);

            // Assert
            queryHandlerMock.Verify(pch => pch.HandleAsync(It.Is<Query<object>>(c => c.Equals(query)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "[UNT-FLHB001]: Call 'FailureHandler' behavior (Query)")]
        [Trait("Feature", "FLHB - Failure Handler Behavior")]
        public async Task QueryDispacted_DispatchAsync_FailureQueryHandler()
        {
            // Arrange
            var sut = CreateSUT();
            var failureQueryHandlerMock = new Mock<IFailureQueryHandler<Query<object>, object>>();
            var query = new Query<object>();
            var queryHandlerMock = new Mock<IQueryHandler<Query<object>, object>>();

            queryHandlerMock.Setup(qh => qh.HandleAsync(It.IsAny<Query<object>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IQueryHandler<Query<object>, object>))))).Returns(queryHandlerMock.Object);
            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IEnumerable<IFailureQueryHandler<Query<object>, object>>))))).Returns(new List<IFailureQueryHandler<Query<object>, object>> { failureQueryHandlerMock.Object, failureQueryHandlerMock.Object });

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.DispatchAsync(query, default));
            failureQueryHandlerMock.Verify(pch => pch.FailHandleAsync(It.Is<Query<object>>(q => q.Equals(query)), It.IsAny<Exception>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "[UNT-FLHB002]: 'FailureHandler' not configured (Query)")]
        [Trait("Feature", "FLHB - Failure Handler Behavior")]
        public async Task QueryDispacted_DispatchAsync_FailureQueryHandlerNotFound()
        {
            // Arrange
            var sut = CreateSUT();
            var queryHandlerMock = new Mock<IQueryHandler<Query<object>, object>>();
            var query = new Query<object>();

            _serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IQueryHandler<Query<object>, object>))))).Returns(queryHandlerMock.Object);

            // Act
            await sut.DispatchAsync(query, default);

            // Assert
            queryHandlerMock.Verify(pch => pch.HandleAsync(It.Is<Query<object>>(c => c.Equals(query)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
