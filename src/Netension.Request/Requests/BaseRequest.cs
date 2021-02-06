﻿using Netension.Request.Abstraction.Requests;
using System;

namespace Netension.Request.Requests
{
    public  class BaseRequest : IRequest
    {
        public Guid RequestId { get; }

        protected BaseRequest(Guid? requestId)
        {
            RequestId = requestId ?? Guid.NewGuid();
        }

        public bool Equals(IRequest other)
        {
            return other != null && RequestId.Equals(other.RequestId);
        }

        public override int GetHashCode()
        {
            return -2107324841 + RequestId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IRequest);
        }
    }
}
