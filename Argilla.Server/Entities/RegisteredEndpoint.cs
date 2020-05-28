using System;
using Argilla.Core.Common.Entities;

namespace Argilla.Server.Entities
{
    public class RegisteredEndpoint: Endpoint
    {
        public RegisteredEndpoint(Endpoint endpoint)
        {
            this.Registered = DateTime.Now;
            this.ServiceName = endpoint.ServiceName;
            this.EndpointSync = endpoint.EndpointSync;
            this.EndpointAsync = endpoint.EndpointAsync;
        }

        public DateTime Registered { get; set; }
    }
}
