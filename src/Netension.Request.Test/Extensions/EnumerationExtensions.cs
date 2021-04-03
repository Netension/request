using FluentValidation.Results;
using System.Collections.Generic;
using Xunit;

namespace Netension.Request.Test.Extensions
{
    public static class EnumerationExtensions
    {
        public static void Validate(this IEnumerable<ValidationFailure> failures, ValidationFailure failure)
        {
            Assert.Contains(failures, f => f.PropertyName.Equals(failure.PropertyName) && f.ErrorMessage.Equals(failure.ErrorMessage));
        }
    }
}
