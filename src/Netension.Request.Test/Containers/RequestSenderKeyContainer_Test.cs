using Netension.Request.Containers;
using Xunit;

namespace Netension.Request.Test.Containers
{
    public class RequestSenderKeyContainer_Test
    {
        private RequestSenderKeyContainer CreateSUT()
        {
            return new RequestSenderKeyContainer();
        }

        [Fact(DisplayName = "RequestSenderKeyContainer - Resolve - Key exists")]
        public void RequestSenderKeyContainer_Resolve_KeyExists()
        {
            // Arrange
            var sut = CreateSUT();
            var rightKey = "RightKey";
            var wrongKey = "WrongKey";
            sut.Registrate(rightKey, (request) => true);
            sut.Registrate(wrongKey, (request) => false);

            // Act
            var key = sut.Resolve(new Command());

            // Assert
            Assert.Equal(rightKey, key);
        }

        [Fact(DisplayName = "RequestSenderKeyContainer - Resolve - Key does not exists")]
        public void RequestSenderKeyContainer_Resolve_KeyDoesNotExists()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var key = sut.Resolve(new Command());

            // Assert
            Assert.Null(key);
        }
    }
}
