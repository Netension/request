using Netension.Core.Exceptions;
using Netension.Request.Http.Enumerations;
using Netension.Request.Http.ValueObjects;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Http.Extensions
{
    public static class HttpContentExtensions
    {
        public static async Task<Exception> DeserializeBadRequestAsync(this HttpContent content, CancellationToken cancellationToken)
        {
            var error = await content.ReadFromJsonAsync<Error>(cancellationToken: cancellationToken).ConfigureAwait(false);

            if (error.Code == ErrorCodeEnumeration.ValidationFailed.Id) return error.SerializeToValidationException();

            return new VerificationException(error.Code, error.Message);
        }
    }
}
