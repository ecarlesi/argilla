using System;
using System.Collections.Generic;
using System.Linq;
using Argilla.Core.Common.Entities;
using Argilla.Server.Entities;
using Argilla.Server.Entities.Settings;

namespace Argilla.Server.Managers
{
    public class DummyStorageManager : IStorageManager
    {
        private static List<RegisteredEndpoint> endpoints = new List<RegisteredEndpoint>();

        public void Configure(Manager managerConfiguration)
        {
        }

        public void Register(Endpoint endpoint)
        {
            if (endpoints.Where(x =>
                        x.ServiceName == endpoint.ServiceName &&
                        x.EndpointSync == endpoint.EndpointSync &&
                        x.EndpointAsync == endpoint.EndpointAsync).FirstOrDefault() != null)
            {
                return;
            }

            endpoints.Add(new RegisteredEndpoint(endpoint));
        }

        public List<Endpoint> Resolve(ResolveRequest request)
        {
            return endpoints.Where(x => x.ServiceName == request.ServiceName).ToList<Endpoint>();
        }

        public void Unregister(Endpoint endpoint)
        {
            RegisteredEndpoint a = endpoints.Where(x =>
                        x.ServiceName == endpoint.ServiceName &&
                        x.EndpointSync == endpoint.EndpointSync &&
                        x.EndpointAsync == endpoint.EndpointAsync).FirstOrDefault();

            if (a == null)
            {
                return;
            }

            endpoints.Remove(a);
        }
    }
}
