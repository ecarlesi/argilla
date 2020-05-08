using System;
using Argilla.Server.Entities.Settings;
using Argilla.Server.Managers;

namespace Argilla.Server.Providers
{
    public class StorageProvider
    {
        internal static IStorageManager Get(Manager manager)
        {
            return InstanceHelper.Create<IStorageManager>(manager.Type);

        }
    }
}
