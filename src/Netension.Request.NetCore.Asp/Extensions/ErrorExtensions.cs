using FluentValidation;
using FluentValidation.Results;
using Netension.Request.NetCore.Asp.ValueObjects;
using System;
using System.Collections.Generic;

namespace Netension.Request.NetCore.Asp.Extensions
{
    public static class ErrorExtensions
    {
        public static ValidationException SerializeToValidationException(this Error error)
        {
            var failures = new List<ValidationFailure>();
            foreach (var line in error.Message.Split(Environment.NewLine))
            {
                if (line.StartsWith("--"))
                {
                    var failure = line.TrimStart('-').Split(':');
                    failures.Add(new ValidationFailure(failure[0].Trim(), failure[1].Trim()));
                }
            }

            return new ValidationException(failures);
        }
    }
}
