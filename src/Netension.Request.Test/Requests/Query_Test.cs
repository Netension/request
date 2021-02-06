using System;
using Xunit;

namespace Netension.Request.Test.Requests
{
    public class Query_Test
    {
        [Fact(DisplayName = "Query - Set request id")]
        public void Query_SetRequestId()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var sut = new Query<object>(id);

            // Assert
            Assert.Equal(id, sut.RequestId);
        }


        [Fact(DisplayName = "Query - Generate request id")]
        public void Query_GeneratequestId()
        {
            // Act
            var sut = new Query<object>();

            // Assert
            Assert.NotEqual(Guid.Empty, sut.RequestId);
        }

        [Fact(DisplayName = "Query - Equal by id")]
        public void Query_EqualById()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            // Assert
            Assert.True(new Query<object>(id).Equals(new Query<object>(id)));
        }

        [Fact(DisplayName = "Query - Not equal by id")]
        public void Query_NotEqualById()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(new Query<object>(Guid.NewGuid()).Equals(new Query<object>(Guid.NewGuid())));
        }
    }
}
