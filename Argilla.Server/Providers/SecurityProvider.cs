using System;
using Argilla.Server.Entities.Settings;
using Argilla.Server.Managers;

namespace Argilla.Server.Providers
{
    public class SecurityProvider
    {
        internal static ISecurityManager Get(Manager manager)
        {
            return InstanceHelper.Create<ISecurityManager>(manager.Type);
        }
    }
}
