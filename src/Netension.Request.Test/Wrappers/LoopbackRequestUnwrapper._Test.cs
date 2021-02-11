﻿using Microsoft.Extensions.Logging;
using Netension.Request.Messages;
using Netension.Request.Wrappers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Wrappers
{
    public class LoopbackRequestUnwrapper_Test
    {
        private readonly ILogger<LoopbackRequestUnwrapper> _logger;

        public LoopbackRequestUnwrapper_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<LoopbackRequestUnwrapper>();
        }

        private LoopbackRequestUnwrapper CreateSUT()
        {
            return new LoopbackRequestUnwrapper(_logger);
        }

        [Fact(DisplayName = "LoopbackRequestUnwrapper - UnwrapAsync - Unwrap request")]
        public async Task LoopbackRequestUnwrapper_UnwrapAsync_UnwrapRequest()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();

            // Act
            var request = await sut.UnwrapAsync(new LoopbackMessage(command), CancellationToken.None);

            // Assert
            Assert.Equal(command, request);
        }
    }
}
