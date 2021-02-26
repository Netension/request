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
            var keys = sut.Resolve(new Command());

            // Assert
            Assert.Contains(rightKey, keys);
            Assert.DoesNotContain(wrongKey, keys);
        }

        [Fact(DisplayName = "RequestSenderKeyContainer - Resolve - Multiple key exists")]
        public void RequestSenderKeyContainer_Resolve_MultipleKeyExists()
        {
            // Arrange
            var sut = CreateSUT();
            var key1 = "key1";
            var key2 = "key2";
            sut.Registrate(key1, (request) => true);
            sut.Registrate(key2, (request) => true);

            // Act
            var keys = sut.Resolve(new Command());

            // Assert
            Assert.Contains(key1, keys);
            Assert.Contains(key2, keys);
        }
    }
}
