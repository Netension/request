using System;
using Xunit;

namespace Netension.Request.Test.Requests
{
    public class Command_Test
    {
        [Fact(DisplayName = "Command - Set request id")]
        public void Command_SetRequestId()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var sut = new Command(id);

            // Assert
            Assert.Equal(id, sut.RequestId);
        }

        [Fact(DisplayName = "Command - Generate request id")]
        public void Command_GenerateRequestId()
        {
            // Arrange
            // Act
            var sut = new Command();

            // Assert
            Assert.NotEqual(Guid.Empty, sut.RequestId);
        }

        [Fact(DisplayName = "Command - Equal by id")]
        public void Command_EqualById()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            // Assert
            Assert.True(new Command(id).Equals(new Command(id)));
        }

        [Fact(DisplayName = "Command - Not equal by id")]
        public void Command_NotEqualById()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(new Command(Guid.NewGuid()).Equals(new Command(Guid.NewGuid())));
        }

        [Fact(DisplayName = "Command - Message type")]
        public void Command_MessageType()
        {
            // Arrange
            var command = new Command();

            // Act
            var messageType = command.MessageType;

            // Assert
            Assert.Equal($"{command.GetType().FullName}, {command.GetType().Assembly.GetName().Name}", messageType);
        }
    }
}
