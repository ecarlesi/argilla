using System;
using Argilla.Core.Common.Entities;

namespace Argilla.Core.Entities
{
    public class CachedResolveResponse : ResolveResponse
    {
        public CachedResolveResponse(string serviceName, ResolveResponse resolveResponse)
        {
            this.ServiceName = serviceName;
            this.Cached = DateTime.Now;
            this.Endpoints = resolveResponse.Endpoints;
            this.Properties = resolveResponse.Properties;
        }

        public DateTime Cached { get; set; }
        public string ServiceName { get; set; }
    }
}
