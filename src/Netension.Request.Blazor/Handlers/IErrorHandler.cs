using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Blazor.Handlers
{
    public interface IErrorHandler
    {
        Task HandleServerErrorAsync(CancellationToken cancellationToken);
        Task HandleVerificationErrorAsync(int code, string message, CancellationToken cancellationToken);
        Task HandleValidationErrorAsync(IEnumerable<ValidationFailure> failures, CancellationToken cancellationToken);
    }
}
