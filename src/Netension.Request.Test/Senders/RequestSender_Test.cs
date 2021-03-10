using AutoFixture;
using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Resolvers;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Senders;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Senders
{
    public class RequestSender_Test
    {
        private readonly ILogger<RequestSender> _logger;
        private ServiceContainer _serviceContainer;
        private Mock<IRequestSenderKeyResolver> _requestSenderKeyResolverMock;

        public RequestSender_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RequestSender>();
        }

        private RequestSender CreateSUT()
        {
            _serviceContainer = new ServiceContainer();
            _requestSenderKeyResolverMock = new Mock<IRequestSenderKeyResolver>();

            return new RequestSender(_serviceContainer.CreateServiceProvider(new ServiceCollection()), _requestSenderKeyResolverMock.Object, _logger);
        }

        [Fact(DisplayName = "RequestSender - QueryAsync - Send query")]
        public async Task RequestSender_QueryAsync_SendQuery()
        {
            // Arrange
            var sut = CreateSUT();
            var querySenderMock = new Mock<IQuerySender>();
            var key = new Fixture().Create<string>();
            var query = new Query<object>();

            _serviceContainer.RegisterInstance(querySenderMock.Object, key);
            _serviceContainer.RegisterInstance(new Mock<IQuerySender>().Object, new Fixture().Create<string>());

            _requestSenderKeyResolverMock.Setup(rskr => rskr.Resolve(It.IsAny<IRequest>()))
                .Returns(key);

            // Act
            await ((IQuerySender)sut).QueryAsync(query, CancellationToken.None);

            // Assert
            querySenderMock.Verify(qs => qs.QueryAsync(It.Is<Query<object>>(q => q.Equals(query)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "RequestSender - QueryAsync - Sender not found")]
        public async Task RequestSender_QueryAsync_SenderNotFound()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await ((IQuerySender)sut).QueryAsync(new Query<object>(), CancellationToken.None));
        }

        [Fact(DisplayName = "RequestSender - SendAsync - Send command")]
        public async Task RequestSender_SendAsync_SendCommand()
        {
            // Arrange
            var sut = CreateSUT();
            var commandSenderMock = new Mock<ICommandSender>();
            var key = new Fixture().Create<string>();
            var command = new Command();

            _serviceContainer.RegisterInstance(commandSenderMock.Object, key);
            _serviceContainer.RegisterInstance(new Mock<ICommandSender>(), new Fixture().Create<string>());

            _requestSenderKeyResolverMock.Setup(rskr => rskr.Resolve(It.IsAny<IRequest>()))
                .Returns(key);

            // Act
            await ((ICommandSender)sut).SendAsync(command, CancellationToken.None);

            // Assert
            commandSenderMock.Verify(cs => cs.SendAsync(It.Is<Command>(c => c.Equals(command)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "RequestSender - SendAsync - Sender not found")]
        public async Task RequestSender_SendAsync_SenderNotFound()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await ((ICommandSender)sut).SendAsync(new Command(), CancellationToken.None));
        }
    }
}
