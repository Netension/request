using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Validators;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Validators
{
    public class QueryValidator_Test
    {
        private Mock<IQueryHandler<Query<object>, object>> _queryHandlerMock;
        private List<IValidator<Query<object>>> _validators;
        private readonly ILogger<QueryValidator<Query<object>, object>> _logger;

        public QueryValidator_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<QueryValidator<Query<object>, object>>();
        }

        private QueryValidator<Query<object>, object> CreateSUT()
        {
            _queryHandlerMock = new Mock<IQueryHandler<Query<object>, object>>();
            _validators = new List<IValidator<Query<object>>>();

            return new QueryValidator<Query<object>, object>(_validators, _queryHandlerMock.Object, _logger);
        }

        [Fact(DisplayName = "QueryValidator - HandleAsync - Validate query")]
        public async Task QueryValidator_HandleAsnyc_ValidateQuery()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Query<object>();
            var validatorMock = new Mock<IValidator<Query<object>>>();

            _validators.Add(validatorMock.Object);
            _validators.Add(validatorMock.Object);

            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Query<object>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Act
            await sut.HandleAsync(query, default).ConfigureAwait(false);

            // Assert
            validatorMock.Verify(v => v.ValidateAsync(It.Is<Query<object>>(c => c.Equals(query)), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "QueryValidator - HandleAsync - Throw ValidationException")]
        public async Task QueryValidator_HandleAsnyc_ThrowValidationException()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Query<object>();
            var validatorMock = new Mock<IValidator<Query<object>>>();

            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Query<object>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new Fixture().CreateMany<ValidationFailure>(2)));

            _validators.Add(validatorMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<ValidationException>(async () => await sut.HandleAsync(query, default).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "QueryValidator - HandleAsync - Validator not found")]
        public async Task QueryValidator_HandleAsnyc_ValidatorNotFound()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Query<object>();

            // Act
            await sut.HandleAsync(query, default).ConfigureAwait(false);

            // Assert
            _queryHandlerMock.Verify(ch => ch.HandleAsync(It.Is<Query<object>>(c => c.Equals(query)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
