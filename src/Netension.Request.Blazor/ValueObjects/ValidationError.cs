using FluentValidation.Results;
using Netension.Request.Http.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netension.Request.Blazor.ValueObjects
{
    public class ValidationError : Error
    {
        public IEnumerable<ValidationFailure> Failures { get; }

        public ValidationError(IEnumerable<ValidationFailure> failures)
            : base(ErrorCodeEnumeration.ValidationFailed.Id, ErrorCodeEnumeration.ValidationFailed.Message)
        {
            Failures = failures;
        }
    }
}
