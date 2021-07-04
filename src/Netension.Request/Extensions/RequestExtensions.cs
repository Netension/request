using FluentValidation;
using FluentValidation.Results;
using Netension.Request.Abstraction.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Extensions
{
    public static class RequestExtensions
    {
        public static async Task ValidateAsync<TRequest>(this TRequest request, IEnumerable<IValidator<TRequest>> validators, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            var validationResults = new List<ValidationResult>();
            foreach (var validator in validators)
            {
                validationResults.Add(await validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false));
            }

            if (validationResults.Any(vr => !vr.IsValid)) throw new ValidationException($"{request.RequestId} is invalid", validationResults.SelectMany(vr => vr.Errors));
        }
    }
}
