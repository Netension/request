using Microsoft.Extensions.Logging;
using Moq;
using Netension.Extensions.Correlation;
using Netension.Request.Messages;
using Netension.Request.Wrappers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Wrappers
{
    public class TestCorrelationMutator : ICorrelationMutator
    {
        public Guid MessageId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid? CausationId { get;  set; }
    }

    public class LoopbackCorrelationUnwrapper_Test
    {
        private readonly ILogger<LoopbackCorrelationUnwrapper> _logger;
        private Mock<ILoopbackRequestUnwrapper> _loopbackRequestUnwrapperMock;
        private TestCorrelationMutator _correlationMutatorMock;

        public LoopbackCorrelationUnwrapper_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<LoopbackCorrelationUnwrapper>();
        }

        private LoopbackCorrelationUnwrapper CreateSUT()
        {
            _loopbackRequestUnwrapperMock = new Mock<ILoopbackRequestUnwrapper>();
            _correlationMutatorMock = new TestCorrelationMutator();

            return new LoopbackCorrelationUnwrapper(_correlationMutatorMock, _loopbackRequestUnwrapperMock.Object, _logger);
        }

        [Fact(DisplayName = "LoopbackCorrelationUnwrapper - UnwrapAsync - Set message id")]
        public async Task LoopbackCorrelationUnwrapper_UnwrapAsync_SetMessageId()
        {
            // Arrange
            var sut = CreateSUT();
            var requestId = Guid.NewGuid();

            // Act
            await sut.UnwrapAsync(new LoopbackMessage(new Command(requestId)), CancellationToken.None);

            // Assert
            Assert.Equal(requestId, _correlationMutatorMock.MessageId);
        }

        [Fact(DisplayName = "LoopbackCorrelationUnwrapper - UnwrapAsync - Set correlation id")]
        public async Task LoopbackCorrelationUnwrapper_UnwrapAsync_SetCorrelationId()
        {
            // Arrange
            var sut = CreateSUT();
            var correlationId = Guid.NewGuid();
            var message = new LoopbackMessage(new Command());

            message.Headers.SetCorrelationId(correlationId);

            // Act
            await sut.UnwrapAsync(message, CancellationToken.None);

            // Assert
            Assert.Equal(correlationId, _correlationMutatorMock.CorrelationId);
        }

        [Fact(DisplayName = "LoopbackCorrelationUnwrapper - UnwrapAsync - Set causation id")]
        public async Task LoopbackCorrelationUnwrapper_UnwrapAsync_SetCausationId()
        {
            // Arrange
            var sut = CreateSUT();
            var causationId = Guid.NewGuid();
            var message = new LoopbackMessage(new Command());

            message.Headers.SetCausationId(causationId);

            // Act
            await sut.UnwrapAsync(message, CancellationToken.None);

            // Assert
            Assert.Equal(causationId, _correlationMutatorMock.CausationId);
        }

        [Fact(DisplayName = "LoopbackCorrelationUnwrapper - UnwrapAsync - Call next unwrap")]
        public async Task LoopbackCorrelationUnwrapper_UnwrapAsync_CallNextUnwrap()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new LoopbackMessage(new Command());

            // Act
            await sut.UnwrapAsync(message, CancellationToken.None);

            // Assert
            _loopbackRequestUnwrapperMock.Verify(lru => lru.UnwrapAsync(It.Is<LoopbackMessage>(lm => lm.Request.Equals(message.Request)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
