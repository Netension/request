using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Resolvers;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Senders;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Senders
{
    public class RequestSender_Test
    {
        private readonly ILogger<RequestSender> _logger;
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<IRequestSenderKeyResolver> _requestSenderKeyResolverMock;

        public RequestSender_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RequestSender>();
        }

        private RequestSender CreateSUT()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _requestSenderKeyResolverMock = new Mock<IRequestSenderKeyResolver>();

            return new RequestSender(_serviceProviderMock.Object, _requestSenderKeyResolverMock.Object, _logger);
        }

        [Fact(DisplayName = "RequestSender - SendAsync - Resolve key")]
        public async Task RequestSender_SendAsync_ResolveKey()
        {
            // Arrange
            var sut = (ICommandSender)CreateSUT();
            var command = new Command();

            _requestSenderKeyResolverMock.Setup(rskr => rskr.Resolve(It.IsAny<IRequest>()))
                .Returns(Enumerable.Empty<string>());

            // Act
            await sut.SendAsync(command, CancellationToken.None);

            // Assert
            _requestSenderKeyResolverMock.Verify(rskr => rskr.Resolve(It.Is<IRequest>(r => r.Equals(command))), Times.Once);
        }

        //[Fact(DisplayName = "RequestSender - SendAsync - Resolve sender")]
        //public async Task RequestSender_SendAsync_ResolveSender()
        //{
        //    // Arrange
        //    var sut = CreateSUT();
        //    var key = "TestKey";

        //    _requestSenderKeyResolverMock.Setup(rskr => rskr.Resolve(It.IsAny<IRequest>()))
        //        .Returns(new List<string> { key, key });

        //    // Act
        //    await sut.SendAsync(new Command(), CancellationToken.None);

        //    // Assert
        //    _serviceProviderMock.Verify(sp => sp.GetService(typeof(Func<string, ICommandSender>)), Times.Once);
        //}

        //[Fact(DisplayName = "RequestSender - SendAsync - Send command")]
        //public async Task RequestSender_SendAsync_SendCommand()
        //{
        //    // Arrange
        //    var sut = CreateSUT();
        //    var key = "TestKey";
        //    var commandSenderMock = new Mock<ICommandSender>();
        //    var command = new Command();

        //    _requestSenderKeyResolverMock.Setup(rskr => rskr.Resolve(It.IsAny<IRequest>()))
        //        .Returns(new List<string> { key, key });
        //    _serviceProviderMock.Setup(sp => sp.GetService(It.IsAny<Type>()))
        //        .Returns((string key) => { return commandSenderMock.Object; });

        //    // Act
        //    await sut.SendAsync(command, CancellationToken.None);

        //    // Assert
        //    commandSenderMock.Verify(cs => cs.SendAsync(It.Is<ICommand>(c => c.Equals(command)), It.IsAny<CancellationToken>()), Times.Once);
        //}
    }
}
