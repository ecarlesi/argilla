using System.Collections.Generic;
using Argilla.Core.Common.Entities;
using Argilla.Server.Entities.Settings;

namespace Argilla.Server
{
    internal interface IStorageManager
    {
        void Configure(Manager managerConfiguration);
        List<Endpoint> Resolve(ResolveRequest request);
        void Register(Endpoint endpoint);
        void Unregister(Endpoint endpoint);
    }
}