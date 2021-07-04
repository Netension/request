using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<object> ReadFromJsonAsync(this HttpRequest request, Type returnType, CancellationToken cancellationToken)
        {
            var buffer = new Memory<byte>();
            await request.Body.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            return JsonSerializer.Deserialize(buffer.Span, returnType);
        }
    }
}
